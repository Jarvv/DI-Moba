using Creeps.Behaviour.Attack;
using Creeps.Behaviour.Movement;
using UnityEngine;

namespace Creeps
{
    [CreateAssetMenu(fileName = "CreepDefinitionSO", menuName = "MOBA/CreepDefinitionSO")]
    public class CreepDefinitionSO : ScriptableObject
    {
        public string Name;
        public GameObject Prefab;
        public float Health;

        [Header("Movement")]
        public MovementBehaviourFactory MovementFactory;

        [Header("Attack")]
        public AttackBehaviourFactory AttackFactory;
    }
}
