using UnityEngine;
using TMPro;
using VContainer;

namespace Game.UI
{
    public class GoldView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _goldText;
        [SerializeField] private string _format = "Gold: {0}";
        private IGoldViewModel _viewModel;

        [Inject]
        public void Construct(IGoldViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            SetGold(_viewModel.Gold);
            _viewModel.GoldChanged += SetGold;
        }

        private void OnDestroy()
        {
            if (_viewModel == null) return;
            _viewModel.GoldChanged -= SetGold;
        }

        public void SetGold(int gold)
        {
            if (_goldText == null) return;
            _goldText.text = string.Format(_format, gold);
        }
    }
}
