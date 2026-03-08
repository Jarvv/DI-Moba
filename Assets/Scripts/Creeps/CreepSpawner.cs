using Lanes;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Waves;

namespace Creeps
{
    public class CreepSpawner : IStartable, ITickable
    {
        private ICreepPool _creepPool;
        private WaveConfigSO _waveConfig;
        private LaneRegistry _laneConfig;
        
        private int _currentWaveIndex = 0;
        private int _currentGroupIndex = 0;
        private int _currentCreepIndex = 0;
        private bool _isSpawning = false;
        private float _spawnTimer = 0f;
        private float _staggerTimer = 0f;
    
       public CreepSpawner(ICreepPool creepPool, WaveConfigSO waveConfig, LaneRegistry laneConfig)
       {
            _creepPool = creepPool;
            _waveConfig = waveConfig;
            _laneConfig = laneConfig;
       }

        public void Start()
        {
            _spawnTimer = _waveConfig.InitialDelay;
        }

        public void Tick()
        {
            if(_isSpawning)
            {
                StaggerSpawn();
                return;
            }
            
            _spawnTimer -= Time.deltaTime;
            
            if(_spawnTimer <= 0f)
            {
                BeginWave();
                _spawnTimer = _waveConfig.TimeBetweenWaves;
            }
        }
        
        private void BeginWave()
        {
            _isSpawning = true;
            _currentGroupIndex = 0;
            _staggerTimer = 0f;
        }
        
        private void StaggerSpawn()
        {
            _staggerTimer -= Time.deltaTime;
            if(_staggerTimer > 0f) return;
            
            WaveDefinitionSO wave = _waveConfig.Waves[_currentWaveIndex];
            CreepGroup group = wave.CreepGroups[_currentGroupIndex];
            
            _creepPool.SpawnCreep(group.Definition, _laneConfig.GetSpawnPoint(_currentGroupIndex));    
            
            _staggerTimer = group.StaggerDelay;
            
            if(_currentCreepIndex >= group.Count)
            {
                _currentCreepIndex = 0;
                _currentGroupIndex++;
            }
            
            if(_currentGroupIndex >= wave.CreepGroups.Length)
            {
                _isSpawning = false;
                _currentWaveIndex++;
            }
        }
    }
}