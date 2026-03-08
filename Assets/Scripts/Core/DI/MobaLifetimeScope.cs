using Core.Events;
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

             // ── Config Instances
            builder.RegisterInstance(_waveConfig);
            builder.RegisterComponent(_laneRegistry);
            builder.RegisterInstance(_teamColourConfig);
             
            // ── Creep System 
            builder.Register<ICreepFactory, CreepFactory>(Lifetime.Singleton);
            builder.Register<ICreepPool, CreepPool>(Lifetime.Singleton);
            
            // ── Lane System 
            builder.Register<IWaypointProvider, WaypointProvider>(Lifetime.Singleton);

            // ── EntryPoint
            builder.RegisterEntryPoint<CreepSpawner>();
        }
    }
}

