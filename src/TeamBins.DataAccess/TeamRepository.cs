using System;
using System.Data.Entity;
using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public class TeamRepository : ITeamRepository
    {
        private TeamEntities db;
        public TeamRepository()
        {
            db = new TeamEntities();
        }

        public TeamDto GetTeam(int teamId)
        {
            var team = db.Teams.FirstOrDefault(s => s.ID == teamId);
            if (team != null)
            {
                return new TeamDto
                {
                    Id = team.ID,
                    Name = team.Name
                };
            }
            return null;
        }

        public int SaveTeam(TeamDto team)
        {
            Team teamEntity = null;
            if (team.Id == 0)
            {
                teamEntity = new Team {CreatedDate = DateTime.UtcNow};
                db.Teams.Add(teamEntity);
            }
            else
            {
                teamEntity = db.Teams.FirstOrDefault(s => s.ID == team.Id);
                if (teamEntity != null)
                {
                    teamEntity.Name = team.Name;
                    db.Entry(team).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return teamEntity.ID;
        }

        public void SaveTeamMember(int teamId, int memberId, int createdById)
        {
            var teamMember = new TeamMember { MemberID = memberId, TeamID = teamId, CreatedByID = createdById };
            teamMember.CreatedDate = DateTime.UtcNow;
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();
        }
    }
}