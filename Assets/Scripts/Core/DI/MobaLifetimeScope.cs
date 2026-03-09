using Core.Combat;
using Core.Events;
using Core.Pooling;
using Core.Teams;
using Creeps;
using Game;
using Lanes;
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

            // ── Lane System 
            builder.Register<IWaypointProvider, WaypointProvider>(Lifetime.Singleton);

            // ── Combat System
            builder.Register<ITargetFinder, TargetFinder>(Lifetime.Singleton);

            // ── EntryPoint
            builder.RegisterEntryPoint<CreepSpawner>();
        }
    }
}

