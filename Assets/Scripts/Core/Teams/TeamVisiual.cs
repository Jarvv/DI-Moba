using Core.Teams;
using UnityEngine;
using VContainer;

public class TeamVisual : MonoBehaviour
{
    private Renderer _renderer;
    private Material _baseMaterial;
    private TeamColourConfigSO _teamColourConfig;

    [Inject]
    private void Construct(TeamColourConfigSO teamColourConfig)
    {
        _teamColourConfig = teamColourConfig;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _baseMaterial = _renderer.sharedMaterial;
    }

    public void SetTeam(Team team)
    {
        _renderer.sharedMaterial = _teamColourConfig.GetTeamMaterial(_baseMaterial, team);
    }
}
