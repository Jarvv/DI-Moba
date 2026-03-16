using Core.Events;
using Core.Teams;
using Game;
using Game.GameState;
using Game.GameState.UI;
using NUnit.Framework;
using Structures;
using Structures.Nexus;
using UnityEngine;

namespace Tests.EditMode
{
    public class GameStateManagerTests
    {
        private GameStateManager _manager;
        private EventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            _manager = new GameStateManager(_eventBus);
            _manager.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _manager.Dispose();
        }

        [Test]
        public void IsGameOver_False_Initially()
        {
            Assert.IsFalse(_manager.IsGameOver);
        }

        [Test]
        public void Non_Nexus_Destroyed_Does_Not_End_Game()
        {
            var go = new GameObject();
            var structure = go.AddComponent<TestStructure>();
            structure.Init(_eventBus, 100f);

            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = structure,
                KillerTeam = Team.Blue,
                GoldReward = 5
            });

            Assert.IsFalse(_manager.IsGameOver);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Nexus_Destroyed_Ends_Game()
        {
            var go = CreateNexus(Team.Red);

            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = go.GetComponent<Nexus>(),
                KillerTeam = Team.Blue,
                GoldReward = 0
            });

            Assert.IsTrue(_manager.IsGameOver);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Nexus_Destroyed_Publishes_GameOverEvent_With_Winner()
        {
            var go = CreateNexus(Team.Red);

            GameOverEvent received = default;
            bool fired = false;
            _eventBus.Subscribe<GameOverEvent>(e => { received = e; fired = true; });

            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = go.GetComponent<Nexus>(),
                KillerTeam = Team.Blue,
                GoldReward = 0
            });

            Assert.IsTrue(fired);
            Assert.AreEqual(Team.Blue, received.WinnerTeam);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void Second_Nexus_Destroyed_Does_Not_Fire_Again()
        {
            var go1 = CreateNexus(Team.Red);
            var go2 = CreateNexus(Team.Blue);

            int fireCount = 0;
            _eventBus.Subscribe<GameOverEvent>(_ => fireCount++);

            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = go1.GetComponent<Nexus>(),
                KillerTeam = Team.Blue,
                GoldReward = 0
            });

            _eventBus.Publish(new StructureDestroyedEvent
            {
                Structure = go2.GetComponent<Nexus>(),
                KillerTeam = Team.Red,
                GoldReward = 0
            });

            Assert.AreEqual(1, fireCount);
            Object.DestroyImmediate(go1);
            Object.DestroyImmediate(go2);
        }

        private GameObject CreateNexus(Team team)
        {
            var go = new GameObject($"Nexus_{team}");
            go.AddComponent<Nexus>();
            return go;
        }
    }

    public class GameOverViewModelTests
    {
        private GameOverViewModel _viewModel;
        private EventBus _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBus();
            var playerContext = new PlayerContext(Team.Blue);
            _viewModel = new GameOverViewModel(_eventBus, playerContext);
            _viewModel.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel.Dispose();
        }

        [Test]
        public void IsGameOver_False_Initially()
        {
            Assert.IsFalse(_viewModel.IsGameOver);
        }

        [Test]
        public void IsVictory_True_When_Local_Team_Wins()
        {
            _eventBus.Publish(new GameOverEvent
            {
                DestroyedNexusTeam = Team.Red,
                WinnerTeam = Team.Blue
            });

            Assert.IsTrue(_viewModel.IsGameOver);
            Assert.IsTrue(_viewModel.IsVictory);
        }

        [Test]
        public void IsVictory_False_When_Local_Team_Loses()
        {
            _eventBus.Publish(new GameOverEvent
            {
                DestroyedNexusTeam = Team.Blue,
                WinnerTeam = Team.Red
            });

            Assert.IsTrue(_viewModel.IsGameOver);
            Assert.IsFalse(_viewModel.IsVictory);
        }

        [Test]
        public void GameOver_Event_Fires()
        {
            bool fired = false;
            _viewModel.GameOver += () => fired = true;

            _eventBus.Publish(new GameOverEvent
            {
                DestroyedNexusTeam = Team.Red,
                WinnerTeam = Team.Blue
            });

            Assert.IsTrue(fired);
        }
    }
}
