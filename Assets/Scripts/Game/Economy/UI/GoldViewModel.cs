using System;
using Core.Events;
using VContainer.Unity;

namespace Game.UI
{
    public interface IGoldViewModel
    {
        int Gold { get; }
        event Action<int> GoldChanged;
    }

    public class GoldViewModel : IGoldViewModel, IStartable, IDisposable
    {
        private readonly IPlayerContext _playerContext;
        private readonly ITeamEconomy _teamEconomy;
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        public int Gold { get; private set; }
        public event Action<int> GoldChanged;

        public GoldViewModel(IPlayerContext playerContext, ITeamEconomy teamEconomy, IEventBus eventBus)
        {
            _playerContext = playerContext;
            _teamEconomy = teamEconomy;
            _eventBus = eventBus;
        }

        public void Start()
        {
            Gold = _teamEconomy.GetGold(_playerContext.LocalTeam);
            GoldChanged?.Invoke(Gold);
            _subscription = _eventBus.Subscribe<TeamGoldChangedEvent>(OnGoldChanged);
        }

        private void OnGoldChanged(TeamGoldChangedEvent e)
        {
            if (e.Team != _playerContext.LocalTeam) return;

            Gold = e.Gold;
            GoldChanged?.Invoke(Gold);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
