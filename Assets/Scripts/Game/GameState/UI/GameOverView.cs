using TMPro;
using UnityEngine;
using VContainer;

namespace Game.GameState.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _resultText;

        private IGameOverViewModel _viewModel;

        [Inject]
        public void Construct(IGameOverViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            _viewModel.GameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            if (_viewModel == null) return;
            _viewModel.GameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            _resultText.text = _viewModel.IsVictory ? "VICTORY" : "DEFEAT";
        }
    }
}
