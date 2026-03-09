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
                var collider = _hitBuffer[i];

                if (!collider.TryGetComponent<DamageableHitbox>(out var hitbox))
                    continue;

                var damageable = hitbox.Owner;

                if (damageable == null) continue;
                if (!damageable.IsAlive) continue;
                if (damageable.Team != targetTeam) continue;

                // Use collider geometry for distance-based targeting so capsule/box behave consistently.
                float nearestSurfaceSqrDistance = (collider.ClosestPoint(origin) - origin).sqrMagnitude;

                float score = priority switch
                {
                    TargetPriority.Nearest => nearestSurfaceSqrDistance,
                    TargetPriority.LowestHealth => damageable.CurrentHealth,
                    TargetPriority.HighestHealth => -damageable.CurrentHealth,
                    _ => nearestSurfaceSqrDistance
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
