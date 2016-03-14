using System;
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
        void Delete(int id);
        //IEnumerable<MemberVM> 

    }

    public class TeamRepository : BaseRepo, ITeamRepository
    {
        public TeamDto GetTeam(int teamId)
        {
            var q =@"SELECT [Id],[Name]  FROM Team WHERE ID=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var teams = con.Query<TeamDto>(q, new { @id = teamId });
                return teams.FirstOrDefault();
            }
        }

        public int SaveTeam(TeamDto team)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                if (team.Id == 0)
                {
                    var p = con.Query<int>("INSERT INTO Team(Name,CreatedDate,CreatedByID) VALUES (@name,@dt,@createdById);SELECT CAST(SCOPE_IDENTITY() as int)",
                                            new { @name = team.Name, @dt = DateTime.Now, @createdById = team.CreatedById });
                    team.Id = p.First();

                    con.Execute("INSERT INTO TeamMember(MemberID,TeamID,CreatedDate,CreatedByID) VALUES (@memberId,@teamId,@dt,@createdById)",
                                           new { memberId = team.CreatedById, @teamId=team.Id, @dt = DateTime.Now, @createdById = team.CreatedById });


                }
                else
                {
                    con.Query<int>("UPDATE Team SET Name=@name WHERE Id=@id", new { @name = team.Name, @id = team.Id });

                }
                return team.Id;

            }
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
            var q =
              @" SELECT T.ID,T.Name,T.CreatedDate,T.CreatedByID,TeamMemberCount.Count as MemberCount 
                FROM Team T
                JOIN TeamMember TM ON T.ID=TM.TeamId
				JOIN (SELECT TeamId,COUNT(1) Count FROM TeamMember  group  by TeamId ) TeamMemberCount on TeamMemberCount.TeamId=T.ID
                WHERE @userId=@userId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<TeamDto>(q, new { @userId = userId});
                return projects.ToList();
            }
        }

        public void SaveDefaultTeamForUser(int userId, int teamId)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            var q =@"DELETE FROM TeamMember WHERE TeamId=@teamId";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Execute(q, new { @teamId = id });
            }
        }

        public MemberVM GetTeamMember(int teamId, int userId)
        {
            var q =
                @"SELECT [Id],[MemberID] ,[TeamID] ,[DefaultProjectID] FROM TeamMember WHERE TeamID=@t AND MemberID=@m";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<MemberVM>(q, new { @t = teamId,@m=userId });
                return projects.FirstOrDefault();
            }
        }
    }
}