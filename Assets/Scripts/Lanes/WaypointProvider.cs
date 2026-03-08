using UnityEngine;

namespace Lanes
{
    public class WaypointProvider : IWaypointProvider
    {
        private readonly LaneRegistry _laneRegistry;

        public WaypointProvider(LaneRegistry laneRegistry)
        {
            _laneRegistry = laneRegistry;
        }

        public Vector3[] GetWaypoints(int laneIndex)
        {
            Transform[] waypointTransforms = _laneRegistry.Lanes[laneIndex].Waypoints;
            Vector3[] waypoints = new Vector3[waypointTransforms.Length];
            
            for (int i = 0; i < waypointTransforms.Length; i++)
            {
                waypoints[i] = waypointTransforms[i].position;
            }

            return waypoints;
        }
    }

    public interface IWaypointProvider
    {
        public Vector3[] GetWaypoints(int laneIndex);
    }
}
