using System;
using Core.Events;
using VContainer.Unity;

namespace Game.GameState.UI
{
    public interface IGameOverViewModel
    {
        bool IsGameOver { get; }
        bool IsVictory { get; }
        event Action GameOver;
    }

    public class GameOverViewModel : IGameOverViewModel, IStartable, IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly IPlayerContext _playerContext;
        private IDisposable _subscription;

        public bool IsGameOver { get; private set; }
        public bool IsVictory { get; private set; }
        public event Action GameOver;

        public GameOverViewModel(IEventBus eventBus, IPlayerContext playerContext)
        {
            _eventBus = eventBus;
            _playerContext = playerContext;
        }

        public void Start()
        {
            _subscription = _eventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameOver(GameOverEvent e)
        {
            IsGameOver = true;
            IsVictory = e.WinnerTeam == _playerContext.LocalTeam;
            GameOver?.Invoke();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
