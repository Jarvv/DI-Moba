using UnityEngine;

namespace Structures
{
    public abstract class StructureDefinitionSO : ScriptableObject
    {
        public float Health;
        public int GoldReward = 1;
    }
}