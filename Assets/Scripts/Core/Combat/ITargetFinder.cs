using Core.Teams;
using UnityEngine;

namespace Core.Combat
{
  public enum TargetPriority
  {
    Nearest,
    LowestHealth,
    HighestHealth,
  }

  public interface ITargetFinder
  {
    IDamageable FindTarget(Vector3 origin, float range, Team targetTeam, TargetPriority priority);
  }
}