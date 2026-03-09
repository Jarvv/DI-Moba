using System;
using System.Collections.Generic;
using Core.Events;
using Core.Teams;

namespace Game
{
    public struct TeamGoldChangedEvent
    {
        public Team Team;
        public int Gold;
    }

    public interface ITeamEconomy
    {
        int GetGold(Team team);
        void AddGold(Team team, int amount);
    }

    public class TeamEconomy : ITeamEconomy
    {
        private readonly IEventBus _eventBus;
        private readonly Dictionary<Team, int> _goldByTeam = new();

        public TeamEconomy(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _goldByTeam[Team.Blue] = 0;
            _goldByTeam[Team.Red] = 0;
        }

        public int GetGold(Team team)
        {
            return _goldByTeam.TryGetValue(team, out int gold) ? gold : 0;
        }

        public void AddGold(Team team, int amount)
        {
            if (amount <= 0) return;

            int next = GetGold(team) + amount;
            _goldByTeam[team] = next;

            _eventBus.Publish(new TeamGoldChangedEvent
            {
                Team = team,
                Gold = next
            });
        }
    }
}
