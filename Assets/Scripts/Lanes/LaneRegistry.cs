using System;
using UnityEngine;

namespace Lanes
{
    public class LaneRegistry : MonoBehaviour
    {
        [Serializable]
        public struct Lane
        {
            public string Name;
            public Transform[] Waypoints;
        }

        public Lane[] Lanes;
        
        public Vector3 GetSpawnPoint(int laneIndex)
        {
            return Lanes[laneIndex].Waypoints[0].position;
        }
    }
}
