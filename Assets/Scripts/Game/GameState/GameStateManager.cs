using System;
using Core.Events;
using Core.State;
using Structures;
using Structures.Nexus;
using VContainer.Unity;

namespace Game.GameState
{
    public class GameStateManager : IStartable, IDisposable, IGameState
    {
        private readonly IEventBus _eventBus;
        private IDisposable _subscription;

        public bool IsGameOver { get; private set; }

        public GameStateManager(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Start()
        {
            _subscription = _eventBus.Subscribe<StructureDestroyedEvent>(OnStructureDestroyed);
        }

        private void OnStructureDestroyed(StructureDestroyedEvent e)
        {
            if (e.Structure is not Nexus) return;
            if (IsGameOver) return;

            IsGameOver = true;
            _eventBus.Publish(new GameOverEvent
            {
                DestroyedNexusTeam = e.Structure.Team,
                WinnerTeam = e.KillerTeam
            });
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
