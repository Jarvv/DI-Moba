using System;
using Creeps;
using UnityEngine;

namespace Waves
{
   [Serializable]
    public struct CreepGroup
    {
        public CreepDefinitionSO Definition;
        public int Count;
        public float StaggerDelay;
    }
    
    [CreateAssetMenu(fileName = "NewWave", menuName = "MOBA/Wave Definition")]
    public class WaveDefinitionSO : ScriptableObject
    {
        public string WaveName;
        public CreepGroup[] CreepGroups;
    }
}
