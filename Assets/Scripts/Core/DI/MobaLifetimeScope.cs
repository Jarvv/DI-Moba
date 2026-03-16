using Core.Combat;
using Core.Events;
using Core.Pooling;
using Core.State;
using Core.Teams;
using Creeps;
using Game;
using Game.GameState;
using Game.GameState.UI;
using Game.UI;
using Lanes;
using Structures;
using Structures.Towers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Waves;

namespace Core.DI
{
    public class MobaLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private WaveConfigSO _waveConfig;

        [SerializeField]
        private LaneRegistry _laneRegistry;

        [SerializeField]
        private TeamColourConfigSO _teamColourConfig;

        [SerializeField]
        private Team _localPlayerTeam = Team.Blue;

        protected override void Configure(IContainerBuilder builder)
        {
            // ── Core Services
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<GameObjectPool>(Lifetime.Singleton);

            // ── Config Instances
            builder.RegisterInstance(_waveConfig);
            builder.RegisterComponent(_laneRegistry);
            builder.RegisterInstance(_teamColourConfig);

            // ── Creep System
            builder.Register<ICreepFactory, CreepFactory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<CreepLifecycle>();

            // ── Player/Economy
            builder.RegisterInstance<IPlayerContext>(new PlayerContext(_localPlayerTeam));
            builder.Register<ITeamEconomy, TeamEconomy>(Lifetime.Singleton);
            builder.RegisterEntryPoint<CreepGoldRewardSystem>();
            builder.RegisterEntryPoint<GoldViewModel>();

            // ── Views
            builder.RegisterComponentInHierarchy<GoldView>();
            builder.RegisterComponentInHierarchy<GameOverView>();
            builder.RegisterComponentInHierarchy<ScreenManager>();

            // ── Lane System 
            builder.Register<IWaypointProvider, WaypointProvider>(Lifetime.Singleton);

            // ── Combat System
            builder.Register<ITargetFinder, TargetFinder>(Lifetime.Singleton);

            // ── Structure System
            builder.RegisterEntryPoint<StructureLifecycle>();

            // ── Game State
            builder.RegisterEntryPoint<GameStateManager>().As<IGameState>();
            builder.RegisterEntryPoint<GameOverViewModel>().As<IGameOverViewModel>();

            // ── EntryPoint
            builder.RegisterEntryPoint<CreepSpawner>();
        }
    }
}
