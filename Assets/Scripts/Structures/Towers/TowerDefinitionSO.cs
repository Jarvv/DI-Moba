using Core.Combat;
using UnityEngine;

namespace Structures.Towers
{
    [CreateAssetMenu(fileName = "TowerDefinition", menuName = "MOBA/Tower Definition")]
    public class TowerDefinitionSO : StructureDefinitionSO
    {
        public string Name;

        [Header("Attack")]
        public AttackBehaviourFactory AttackFactory;
    }
}
