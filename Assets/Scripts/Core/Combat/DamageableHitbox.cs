using UnityEngine;

namespace Core.Combat
{
  [RequireComponent(typeof(Collider))]
  public class DamageableHitbox : MonoBehaviour
  {
    public IDamageable Owner { get; private set; }

    private void Awake()
    {
      Owner = GetComponentInParent<IDamageable>();
    }
  }
}