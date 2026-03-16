using System.Collections.Generic;
using Core.Events;
using Core.State;
using Core.Teams;
using Creeps;
using Game;
using Game.GameState;
using Lanes;
using NUnit.Framework;
using UnityEngine;
using Waves;

namespace Tests.EditMode
{
    public class CreepSpawnerTests
    {
        private SpawnRecorder _recorder;
        private WaveConfigSO _waveConfig;
        private LaneRegistry _laneRegistry;
        private StubWaypointProvider _waypointProvider;
        private IGameState _gameState;

        private readonly List<Object> _cleanup = new();

        [SetUp]
        public void SetUp()
        {
            _recorder = new SpawnRecorder();
            _waypointProvider = new StubWaypointProvider();

            _waveConfig = ScriptableObject.CreateInstance<WaveConfigSO>();
            _cleanup.Add(_waveConfig);

            var laneGO = new GameObject("LaneRegistry");
            _laneRegistry = laneGO.AddComponent<LaneRegistry>();
            _cleanup.Add(laneGO);

            // Set up one lane with dummy waypoints
            var blueWP = new GameObject("BlueWP").transform;
            var redWP = new GameObject("RedWP").transform;
            _cleanup.Add(blueWP.gameObject);
            _cleanup.Add(redWP.gameObject);

            _laneRegistry.Lanes = new[]
            {
                new LaneRegistry.Lane
                {
                    Name = "Mid",
                    BlueWaypoints = new[] { blueWP },
                    RedWaypoints = new[] { redWP }
                }
            };

            _gameState = new GameStateManager(new EventBus());
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var obj in _cleanup)
                Object.DestroyImmediate(obj);
            _cleanup.Clear();
        }

        private CreepSpawner CreateSpawner()
        {
            return new CreepSpawner(_recorder, _waveConfig, _laneRegistry, _waypointProvider, _gameState);
        }

        private WaveDefinitionSO CreateWave(int groupCount, int creepsPerGroup, float staggerDelay = 0f)
        {
            var def = ScriptableObject.CreateInstance<CreepDefinitionSO>();
            _cleanup.Add(def);

            var wave = ScriptableObject.CreateInstance<WaveDefinitionSO>();
            _cleanup.Add(wave);

            var groups = new CreepGroup[groupCount];
            for (int i = 0; i < groupCount; i++)
            {
                groups[i] = new CreepGroup
                {
                    Definition = def,
                    Count = creepsPerGroup,
                    StaggerDelay = staggerDelay
                };
            }

            wave.CreepGroups = groups;
            return wave;
        }

        [Test]
        public void Start_Sets_SpawnTimer_To_InitialDelay()
        {
            _waveConfig.InitialDelay = 3f;
            _waveConfig.Waves = new WaveDefinitionSO[0];
            var spawner = CreateSpawner();

            spawner.Start();

            // Ticking less than 3s should not trigger a wave
            spawner.Tick(2.9f);
            Assert.AreEqual(0, _recorder.SpawnCount);
        }

        [Test]
        public void First_Wave_Begins_After_InitialDelay()
        {
            _waveConfig.InitialDelay = 1f;
            _waveConfig.Waves = new[] { CreateWave(1, 1) };
            var spawner = CreateSpawner();

            spawner.Start();

            spawner.Tick(1.1f); // timer expires, BeginWave called
            spawner.Tick(0.1f); // StaggerSpawn runs

            Assert.Greater(_recorder.SpawnCount, 0);
        }

        [Test]
        public void Spawns_For_Both_Teams_Per_Creep()
        {
            _waveConfig.InitialDelay = 0f;
            _waveConfig.TimeBetweenWaves = 99f;
            _waveConfig.Waves = new[] { CreateWave(1, 1) };
            var spawner = CreateSpawner();

            spawner.Start();

            spawner.Tick(0.1f); // timer expires, BeginWave
            spawner.Tick(0.1f); // StaggerSpawn — spawns 1 creep x 2 teams

            Assert.AreEqual(2, _recorder.SpawnCount);
            Assert.IsTrue(_recorder.SpawnedTeams.Contains(Team.Blue));
            Assert.IsTrue(_recorder.SpawnedTeams.Contains(Team.Red));
        }

        [Test]
        public void Spawns_Correct_Total_For_Group()
        {
            _waveConfig.InitialDelay = 0f;
            _waveConfig.TimeBetweenWaves = 99f;
            _waveConfig.Waves = new[] { CreateWave(1, 3) }; // 1 group, 3 creeps
            var spawner = CreateSpawner();

            spawner.Start();

            // Tick once to begin wave, then 3 more for each creep in the group
            for (int i = 0; i < 4; i++)
                spawner.Tick(0.1f);

            // 3 creeps x 2 teams = 6 total spawns
            Assert.AreEqual(6, _recorder.SpawnCount);
        }

        [Test]
        public void Stagger_Delay_Pauses_Between_Spawns()
        {
            _waveConfig.InitialDelay = 0f;
            _waveConfig.TimeBetweenWaves = 99f;
            _waveConfig.Waves = new[] { CreateWave(1, 2, staggerDelay: 1f) };
            var spawner = CreateSpawner();

            spawner.Start();

            spawner.Tick(0.1f); // BeginWave
            spawner.Tick(0.1f); // first creep spawned, stagger timer set to 1f

            Assert.AreEqual(2, _recorder.SpawnCount); // 1 creep x 2 teams

            // Tick with small dt — stagger timer hasn't elapsed
            spawner.Tick(0.5f);
            Assert.AreEqual(2, _recorder.SpawnCount); // still 2

            // Tick past the stagger delay
            spawner.Tick(0.6f);
            Assert.AreEqual(4, _recorder.SpawnCount); // second creep spawned
        }

        [Test]
        public void Multiple_Groups_In_Wave()
        {
            _waveConfig.InitialDelay = 0f;
            _waveConfig.TimeBetweenWaves = 99f;
            _waveConfig.Waves = new[] { CreateWave(2, 1) }; // 2 groups, 1 creep each
            var spawner = CreateSpawner();

            spawner.Start();

            spawner.Tick(0.1f); // BeginWave
            spawner.Tick(0.1f); // group 0, creep 0
            spawner.Tick(0.1f); // group 1, creep 0

            // 2 groups x 1 creep x 2 teams = 4
            Assert.AreEqual(4, _recorder.SpawnCount);
        }

        /// <summary>
        /// Records all creep factory create calls without creating real GameObjects.
        /// </summary>
        private class SpawnRecorder : ICreepFactory
        {
            public int SpawnCount { get; private set; }
            public List<Team> SpawnedTeams { get; } = new();

            public Creep Create(CreepDefinitionSO definition, Vector3 position, Vector3[] waypoints, Team team)
            {
                SpawnCount++;
                SpawnedTeams.Add(team);
                return null;
            }
        }

        private class StubWaypointProvider : IWaypointProvider
        {
            public Vector3[] GetWaypoints(int laneIndex, Team team)
            {
                return new[] { Vector3.zero, Vector3.forward * 10f };
            }
        }
    }
}
