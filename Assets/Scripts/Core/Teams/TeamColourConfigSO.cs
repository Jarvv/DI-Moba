using System.Collections.Generic;
using UnityEngine;

namespace Core.Teams
{
    [CreateAssetMenu(fileName = "NewTeamColourConfig", menuName = "MOBA/Team Colour Config")]
    public class TeamColourConfigSO : ScriptableObject
    {
        private static readonly int BaseColorProp = Shader.PropertyToID("_BaseColor");

        public Color RedTeamColour = Color.red;
        public Color BlueTeamColour = Color.blue;

        private readonly Dictionary<(Material, Team), Material> _cache = new();

        public Material GetTeamMaterial(Material baseMaterial, Team team)
        {
            var key = (baseMaterial, team);

            if (_cache.TryGetValue(key, out var cached))
                return cached;

            var instance = new Material(baseMaterial);
            instance.SetColor(BaseColorProp, GetColourForTeam(team));
            _cache[key] = instance;
            return instance;
        }

        private Color GetColourForTeam(Team team)
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
