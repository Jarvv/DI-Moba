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

        private void OnDrawGizmos()
        {
            if (Lanes == null) return;

            foreach (Lane lane in Lanes)
            {
                DrawWaypointPath(lane.BlueWaypoints, Color.blue);
                DrawWaypointPath(lane.RedWaypoints, Color.red);
            }
        }

        private static void DrawWaypointPath(Transform[] waypoints, Color colour)
        {
            if (waypoints == null || waypoints.Length == 0) return;

            Gizmos.color = colour;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                if (i > 0 && waypoints[i - 1] != null)
                    Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
