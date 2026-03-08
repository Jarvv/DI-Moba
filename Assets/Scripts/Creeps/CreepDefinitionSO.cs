using Creeps.Behaviour;
using UnityEngine;

namespace Creeps
{
    [CreateAssetMenu(fileName = "CreepDefinitionSO", menuName = "MOBA/CreepDefinitionSO")]
    public class CreepDefinitionSO : ScriptableObject
    {
        public string Name;
        public GameObject Prefab;
        public float Health;
        public float Speed;

        [Header("Attack")]
        public AttackBehaviourFactory AttackFactory;
    }
}