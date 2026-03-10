using System;

namespace Core.Combat.UI
{
    public class HealthBarViewModel : IDisposable
    {
        private readonly IDamageable _target;

        public float FillAmount => _target.MaxHealth > 0 ? _target.CurrentHealth / _target.MaxHealth : 0f;
        public bool IsVisible => _target.IsAlive && FillAmount < 1f;

        public event Action HealthChanged;

        public HealthBarViewModel(IDamageable target)
        {
            _target = target;
            _target.DamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken(float amount)
        {
            HealthChanged?.Invoke();
        }

        public void Dispose()
        {
            _target.DamageTaken -= OnDamageTaken;
        }
    }
}
