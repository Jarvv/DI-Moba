using Core.Teams;
using UnityEngine;

namespace Core.Combat
{
    public interface IDamageSource
    {
        float Damage { get; }
        Team Team { get; }
    }
}
