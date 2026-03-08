using Core.Teams;
using UnityEngine;
using VContainer;

public class TeamVisual : MonoBehaviour
{
    private static readonly int ColorProp = Shader.PropertyToID("_BaseColor");
    
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private TeamColourConfigSO _teamColourConfig;
    
    [Inject]
    private void Construct(TeamColourConfigSO teamColourConfig)
    {
        _teamColourConfig = teamColourConfig;
    }
    
    private void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void SetTeam(Team team)
    {
        _propBlock.SetColor(ColorProp, _teamColourConfig.GetColourForTeam(team));
        
        _renderer.SetPropertyBlock(_propBlock);
    }
    
    public void Reset()
    {
        _renderer.SetPropertyBlock(null);
    }
}