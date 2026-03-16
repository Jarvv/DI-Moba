using System;
using System.Collections.Generic;
using Core.Events;
using Game.GameState;
using UnityEngine;
using VContainer;

namespace Game.UI
{
    public enum Screen
    {
        Game,
        GameOver
    }

    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _gameScreen;
        [SerializeField] private CanvasGroup _gameOverScreen;

        private Dictionary<Screen, CanvasGroup> _screens;
        private Screen _currentScreen;
        private IEventBus _eventBus;
        private IDisposable _subscription;

        [Inject]
        public void Construct(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            _screens = new Dictionary<Screen, CanvasGroup>
            {
                { Screen.Game, _gameScreen },
                { Screen.GameOver, _gameOverScreen }
            };

            SetScreen(Screen.Game);
            _subscription = _eventBus.Subscribe<GameOverEvent>(OnGameOver);
        }

        private void OnGameOver(GameOverEvent e)
        {
            SetScreen(Screen.GameOver);
        }

        public void SetScreen(Screen screen)
        {
            HideScreen(_screens[_currentScreen]);
            _currentScreen = screen;
            ShowScreen(_screens[_currentScreen]);
        }

        public void ShowOverlay(Screen screen)
        {
            ShowScreen(_screens[screen]);
        }

        public void HideOverlay(Screen screen)
        {
            HideScreen(_screens[screen]);
        }

        private void ShowScreen(CanvasGroup screen)
        {
            screen.alpha = 1f;
            screen.interactable = true;
            screen.blocksRaycasts = true;
        }

        private void HideScreen(CanvasGroup screen)
        {
            screen.alpha = 0f;
            screen.interactable = false;
            screen.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
