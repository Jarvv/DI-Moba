using Core.Teams;
using Creeps;
using Lanes;
using UnityEngine;
using VContainer.Unity;
using Waves;

namespace Game
{
    public class CreepSpawner : IStartable, ITickable
    {
        private ICreepPool _creepPool;
        private WaveConfigSO _waveConfig;
        private LaneRegistry _laneConfig;
        private IWaypointProvider _waypointProvider;

        private int _currentWaveIndex = 0;
        private int _currentGroupIndex = 0;
        private int _currentCreepIndex = 0;
        private int _currentLaneIndex = 0;
        private bool _isSpawning = false;
        private float _spawnTimer = 0f;
        private float _staggerTimer = 0f;
        private readonly Team[] _teams = { Team.Blue, Team.Red };

        public CreepSpawner(ICreepPool creepPool, WaveConfigSO waveConfig, LaneRegistry laneConfig, IWaypointProvider waypointProvider)
        {
            _creepPool = creepPool;
            _waveConfig = waveConfig;
            _laneConfig = laneConfig;
            _waypointProvider = waypointProvider;
        }

        public void Start()
        {
            _spawnTimer = _waveConfig.InitialDelay;
        }

        public void Tick()
        {
            Tick(Time.deltaTime);
        }

        public void Tick(float deltaTime)
        {
            if (_isSpawning)
            {
                StaggerSpawn(deltaTime);
                return;
            }

            _spawnTimer -= deltaTime;

            if (_spawnTimer <= 0f)
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

        private void StaggerSpawn(float deltaTime)
        {
            _staggerTimer -= deltaTime;
            if (_staggerTimer > 0f) return;

            WaveDefinitionSO wave = _waveConfig.Waves[_currentWaveIndex];
            CreepGroup group = wave.CreepGroups[_currentGroupIndex];

            foreach (Team team in _teams)
            {
                Vector3 spawnPoint = _laneConfig.GetSpawnPoint(_currentLaneIndex, team);
                Vector3[] waypoints = _waypointProvider.GetWaypoints(_currentLaneIndex, team);
                _creepPool.SpawnCreep(group.Definition, spawnPoint, waypoints, team);
            }

            _currentCreepIndex++;
            _staggerTimer = group.StaggerDelay;

            if (_currentCreepIndex >= group.Count)
            {
                _currentCreepIndex = 0;
                _currentGroupIndex++;
            }

            if (_currentGroupIndex >= wave.CreepGroups.Length)
            {
                _isSpawning = false;
                _currentWaveIndex++;
            }
        }
    }
}
