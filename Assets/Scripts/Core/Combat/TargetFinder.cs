using Core.Teams;
using UnityEngine;

namespace Core.Combat
{
  public class TargetFinder : ITargetFinder
  {
    private readonly Collider[] _hitBuffer = new Collider[64];

    public IDamageable FindTarget(Vector3 origin, float range, Team targetTeam, TargetPriority priority)
    {
      int count = Physics.OverlapSphereNonAlloc(origin, range, _hitBuffer);

      IDamageable best = null;
      float bestScore = float.MaxValue;

      for (int i = 0; i < count; i++)
      {
        if (!_hitBuffer[i].TryGetComponent<DamageableHitbox>(out var hitbox))
          continue;

        var damageable = hitbox.Owner;

        if (damageable == null) continue;
        if (!damageable.IsAlive) continue;
        if (damageable.Team != targetTeam) continue;

        float score = priority switch
        {
          TargetPriority.Nearest => Vector3.SqrMagnitude(damageable.Position - origin),
          TargetPriority.LowestHealth => damageable.CurrentHealth,
          TargetPriority.HighestHealth => -damageable.CurrentHealth,
          _ => Vector3.SqrMagnitude(damageable.Position - origin)
        };

        if (score < bestScore)
        {
          bestScore = score;
          best = damageable;
        }
      }

      return best;
    }
  }
}