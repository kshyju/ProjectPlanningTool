using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface ITeamRepository
    {
        TeamDto GetTeam(int teamId);
        int SaveTeam(TeamDto team);

        void SaveTeamMember(int teamId, int memberId, int createdById);
        void SaveDefaultProject(int userId, int teamId, int? selectedProject);
        List<TeamDto> GetTeams(int userId);

        void SaveDefaultTeamForUser(int userId, int teamId);
        MemberVM GetTeamMember(int teamId, int userId);

        //IEnumerable<MemberVM> 

    }

    public class TeamRepository : BaseRepo, ITeamRepository
    {
        public TeamDto GetTeam(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public int SaveTeam(TeamDto team)
        {
            throw new System.NotImplementedException();
        }

        public void SaveTeamMember(int teamId, int memberId, int createdById)
        {
            throw new System.NotImplementedException();
        }

        public void SaveDefaultProject(int userId, int teamId, int? selectedProject)
        {
            throw new System.NotImplementedException();
        }

        public List<TeamDto> GetTeams(int userId)
        {
            throw new System.NotImplementedException();
        }

        public void SaveDefaultTeamForUser(int userId, int teamId)
        {
            throw new System.NotImplementedException();
        }

        public MemberVM GetTeamMember(int teamId, int userId)
        {
            var q =
                @"SELECT [ID],[MemberID] ,[TeamID] ,[DefaultProjectID] FROM TeamMember WHERE TeamID=@t AND MemberID=@m";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<MemberVM>(q, new { @t = teamId,@m=userId });
                return projects.FirstOrDefault();
            }
        }
    }
}