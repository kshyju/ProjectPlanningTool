using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public class TeamRepository : ITeamRepository
    {
        private TeamEntitiesConn db;
        public TeamRepository()
        {
            db = new TeamEntitiesConn();
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

        public List<TeamDto> GetTeams(int userId)
        {
            return db.TeamMembers.Where(s => s.MemberID == userId)
                .Select(x => new TeamDto
                {
                    Id = x.Team.ID, Name = x.Team.Name , IsRequestingUserTeamOwner = x.CreatedByID==userId,
                    MemberCount = x.Team.TeamMembers.Count()
                }).ToList();
        }

        public void SaveDefaultProject(int userId, int teamId, int? selectedProject)
        {
            var teamMember = db.TeamMembers.FirstOrDefault(s => s.MemberID == userId && s.TeamID == teamId);
            if (teamMember != null)
            {
                teamMember.DefaultProjectID = selectedProject;
                db.Entry(teamMember).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public int SaveTeam(TeamDto team)
        {
            Team teamEntity = null;
            if (team.Id == 0)
            {
                teamEntity = new Team {CreatedDate = DateTime.UtcNow, Name = team.Name, CreatedByID = team.CreatedById};
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

        public void SaveDefaultTeamForUser(int userId, int teamId)
        {
            var user = db.Users.FirstOrDefault(s => s.ID == userId);
            if (user != null)
            {
                user.DefaultTeamID = teamId;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public MemberVM GetTeamMember(int teamId, int userId)
        {
            var teamMember = db.TeamMembers.FirstOrDefault(s => s.MemberID == userId && s.TeamID == teamId);
            return new MemberVM {DefaultProjectId = teamMember.DefaultProjectID};

        }
    }
}