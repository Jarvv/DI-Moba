using System;
using Creeps;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(fileName = "NewWaveConfig", menuName = "MOBA/Wave Config")]
    public class WaveConfigSO : ScriptableObject
    {
        [Header("Timing")]
        public float InitialDelay = 5f;
        public float TimeBetweenWaves = 30f;

        [Header("Wave Sequence")]
        public WaveDefinitionSO[] Waves;
    }
}