using Core.Combat;
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
        public int GoldReward = 1;

        [Header("Movement")]
        public MovementBehaviourFactory MovementFactory;

        [Header("Attack")]
        public float AcquisitionRange = 8f;
        public AttackBehaviourFactory AttackFactory;
    }
}
