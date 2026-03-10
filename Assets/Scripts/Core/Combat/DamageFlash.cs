using System.Collections;
using UnityEngine;

namespace Core.Combat
{
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private float _flashDuration = 0.1f;
        [SerializeField] private Material _flashMaterial;

        private Renderer _renderer;
        private Material _originalMaterial;
        private IDamageable _damageable;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            _damageable = GetComponentInParent<IDamageable>();
            _damageable.DamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken(float amount)
        {
            if (_flashRoutine != null)
                StopCoroutine(_flashRoutine);

            _flashRoutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            _originalMaterial = _renderer.sharedMaterial;
            _renderer.sharedMaterial = _flashMaterial;

            yield return new WaitForSeconds(_flashDuration);

            _renderer.sharedMaterial = _originalMaterial;
            _flashRoutine = null;
        }

        private void OnDestroy()
        {
            if (_damageable != null)
                _damageable.DamageTaken -= OnDamageTaken;
        }
    }
}
