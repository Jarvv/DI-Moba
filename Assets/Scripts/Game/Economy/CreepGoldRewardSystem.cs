using System;
using Core.Events;
using Creeps;
using VContainer.Unity;

namespace Game
{
    public class CreepGoldRewardSystem : IStartable, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly ITeamEconomy _teamEconomy;
        private IDisposable _subscription;

        public CreepGoldRewardSystem(IEventBus eventBus, ITeamEconomy teamEconomy)
        {
            _eventBus = eventBus;
            _teamEconomy = teamEconomy;
        }

        public void Start()
        {
            _subscription = _eventBus.Subscribe<CreepDiedEvent>(OnCreepDied);
        }

        private void OnCreepDied(CreepDiedEvent e)
        {
            if (!e.KillerTeam.HasValue) return;
            if (e.KillerTeam.Value == e.Creep.Team) return;

            _teamEconomy.AddGold(e.KillerTeam.Value, e.GoldReward);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
