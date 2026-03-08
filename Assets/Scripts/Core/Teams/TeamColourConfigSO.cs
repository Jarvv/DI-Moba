using UnityEngine;

namespace Core.Teams
{
    [CreateAssetMenu(fileName = "NewTeamColourConfig", menuName = "MOBA/Team Colour Config")]
    public class TeamColourConfigSO : ScriptableObject
    {
        public Color RedTeamColour = Color.red;
        public Color BlueTeamColour = Color.blue;
        
        public Color GetColourForTeam(Team team)
        {
            return team switch
            {
                Team.Red => RedTeamColour,
                Team.Blue => BlueTeamColour,
                _ => Color.white
            };
        }
    }
}