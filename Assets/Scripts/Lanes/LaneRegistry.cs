using System;
using Core.Teams;
using UnityEngine;

namespace Lanes
{
    public class LaneRegistry : MonoBehaviour
    {
        [Serializable]
        public struct Lane
        {
            public string Name;
            public Transform[] BlueWaypoints;
            public Transform[] RedWaypoints;
        }

        public Lane[] Lanes;
        
        public Vector3 GetSpawnPoint(int laneIndex, Team team)
        {
            Lane lane = Lanes[laneIndex];
            return team switch
            {
                Team.Red => lane.RedWaypoints[0].position,
                Team.Blue => lane.BlueWaypoints[0].position,
                _ => Vector3.zero
            };
        }
    }
}
