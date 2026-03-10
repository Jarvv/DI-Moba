using UnityEngine;

namespace Core.Combat.UI
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private Transform _fillBar;
        [SerializeField] private Transform _pivot;

        private HealthBarViewModel _viewModel;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;

            IDamageable damageable = GetComponentInParent<IDamageable>();

            _viewModel = new HealthBarViewModel(damageable);
            _viewModel.HealthChanged += Refresh;
            Refresh();
        }

        private void LateUpdate()
        {
            _pivot.forward = _camera.transform.forward;

            bool visible = _viewModel.IsVisible;
            if (_pivot.gameObject.activeSelf != visible)
                _pivot.gameObject.SetActive(visible);
        }

        private void Refresh()
        {
            Vector3 scale = _fillBar.localScale;
            scale.x = _viewModel.FillAmount;
            _fillBar.localScale = scale;
        }

        private void OnDestroy()
        {
            _viewModel.HealthChanged -= Refresh;
            _viewModel.Dispose();
        }
    }
}
