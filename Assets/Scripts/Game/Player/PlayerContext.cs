using Core.Teams;

namespace Game
{
    public interface IPlayerContext
    {
        Team LocalTeam { get; }
    }

    public class PlayerContext : IPlayerContext
    {
        public Team LocalTeam { get; }

        public PlayerContext(Team localTeam)
        {
            LocalTeam = localTeam;
        }
    }
}
