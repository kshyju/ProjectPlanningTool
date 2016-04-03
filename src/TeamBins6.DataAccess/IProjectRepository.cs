using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TeamBins.Common;
using Dapper;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{
    public interface ICommentRepository
    {
        int Save(CommentVM comment);
        CommentVM GetComment(int id);

        IEnumerable<CommentVM> GetComments(int issueId);
        void Delete(int id);
    }

    public class CommentRepository : BaseRepo, ICommentRepository
    {
        public int Save(CommentVM comment)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var p = con.Query<int>("INSERT INTO Comment(CommentText,IssueID,CreatedDate,CreatedByID) VALUES (@cmnt,@issueId,@dt,@createdById);SELECT CAST(SCOPE_IDENTITY() as int)",
                                        new { cmnt = comment.CommentText, @issueId = comment.IssueId, @dt = DateTime.Now, @createdById = comment.Author.Id });
                return p.First();

            }
        }

        public IEnumerable<CommentVM> GetComments(int issueId)
        {
            var q = @"SELECT C.*,U.Id,U.FIRSTNAME AS NAME,U.EmailAddress FROM COMMENT C WITH (NOLOCK) 
                    INNER JOIN [USER] U WITH (NOLOCK)  ON C.CREATEDBYID=U.Id
                    WHERE C.IssueId=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = con.Query<CommentVM, UserDto, CommentVM>(q, (c, a) => { c.Author = a; return c; }, new { @id = issueId }, null, false).ToList();
                return com;
            }

        }
        public void Delete(int id)
        {
            var q = @"DELETE FROM COMMENT WHERE Id=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Query<int>(q, new { @id = id });

            }
        }

        public CommentVM GetComment(int id)
        {
            var q = @"SELECT C.*,U.Id,U.FIRSTNAME AS NAME,U.EmailAddress FROM COMMENT C WITH (NOLOCK) 
                    INNER JOIN [USER] U WITH (NOLOCK)  ON C.CREATEDBYID=U.Id
                    WHERE C.Id=@id";
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var com = con.Query<CommentVM, UserDto, CommentVM>(q, (c, a) => { c.Author = a; return c; }, new { @id = id }, null, false).ToList();
                return com.FirstOrDefault();
            }
        }
    }

    public interface IProjectRepository
    {
        IEnumerable<ProjectDto> GetProjects(int teamId);
        bool DoesProjectsExist(int teamId);
        void Save(CreateProjectVM model);
        ProjectDto GetProject(int id);

        ProjectDto GetDefaultProjectForTeam(int teamId);
        int GetIssueCountForProject(int projectId);

        Task<ProjectDto> GetDefaultProjectForTeamMember(int teamId, int userId);

        void Delete(int projectId);
    }

    public class ProjectRepository : BaseRepo, IProjectRepository
    {

        public void Delete(int projectId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Query<int>("DELETE from Project WHERE Id=@projectId", new { @projectId = projectId });
            }
        }

        public int GetIssueCountForProject(int projectId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var issueCount = con.Query<int>("SELECT COUNT(Id) from Issue WITH (NOLOCK) WHERE PROJECTID=@projectId", new { @projectId = projectId });
                return issueCount.First();
            }
        }


        public IEnumerable<ProjectDto> GetProjects(int teamId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<ProjectDto>("SELECT * FROM Project WITH (NOLOCK) WHERE TeamId=@teamId", new { @teamId = teamId });

























                return projects;
            }

        }
        

        public bool DoesProjectsExist(int teamId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projectCount = con.Query<int>("SELECT COUNT(1) FROM Project  WITH (NOLOCK) WHERE TeamId=@teamId", new { @teamId = teamId });
                return projectCount.First() > 0;
            }

        }

        public void Save(CreateProjectVM model)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                if (model.Id == 0)
                {
                    var p = con.Query<int>("INSERT INTO Project(Name,TeamID,CreatedDate,CreatedByID) VALUES (@name,@teamId,@dt,@createdById);SELECT CAST(SCOPE_IDENTITY() as int)",
                                            new { @name = model.Name, @teamId = model.TeamId, @dt = DateTime.Now, @createdById = model.CreatedById });
                    model.Id = p.First();
                }
                else
                {
                    con.Query<int>("UPDATE Project SET Name=@name WHERE Id=@id",new { @name = model.Name, @id = model.Id });

                }

                SetAsDefaultProjectIfNotExists(model);
            }
        }

        private void SetAsDefaultProjectIfNotExists(CreateProjectVM model)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var defaultProjectId = con.Query<int?>("SELECT TOP 1 DefaultProjectId from TEAMMEMBER WHERE TeamId = @teamId and MemberId = @userId", new { @teamId = model.TeamId, @userId = model.CreatedById });
                if (defaultProjectId.Any()==false || defaultProjectId.First() == null)
                {
                    con.Query<int>(" UPDATE TEAMMEMBER SET DEFAULTPROJECTID=@projectId WHERE TEAMID=@teamId AND MEMBERID=@userId",
                                      new
                                      {
                                          @projectId = model.Id,
                                          @teamId = model.TeamId,
                                          @userId = model.CreatedById
                                      });
                }
            }

        }

        public ProjectDto GetProject(int id)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<ProjectDto>("SELECT * FROM Project WHERE Id=@id", new { @id = id });
                return projects.First();
            }
        }

        public ProjectDto GetDefaultProjectForTeam(int teamId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<ProjectDto>("SELECT * FROM Project WHERE Id=@id", new { @id = teamId });
                return projects.Any() ? projects.First() : null;
            }
        }

        public async Task<ProjectDto> GetDefaultProjectForTeamMember(int teamId, int userId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<ProjectDto>(@"SELECT P.ID,P.Name from TeamMember TM
                                                JOIN Project P ON P.ID = TM.DefaultProjectID 
                                                where TM.TeamId =@teamId and MemberId=@memberId", 
                                                new { @teamId = teamId, @memberId = userId });
                if (!projects.Any())
                {
                    return null;

                }
                return projects.First();
            }
        }
    }


    //public class ProjectRepository : IProjectRepository
    //{
    //    private readonly TeamEntitiesConn db;
    //    public ProjectRepository()
    //    {
    //        db = new TeamEntitiesConn();
    //    }

    //    public bool DoesProjectsExist(int teamId)
    //    {
    //        return db.Projects.Any(s => s.TeamID == teamId);
    //    }

    //    public IEnumerable<ProjectDto> GetProjects(int teamId)
    //    {
    //        var projectList = new List<TeamBins.Common.ProjectDto>();

    //        using (var c = new SqlConnection(db.Database.Connection.ConnectionString))
    //        {

    //            var cmd = new SqlCommand("SELECT Id,Name from Project where TeamId=@teamId",c);
    //            cmd.Parameters.AddWithValue("teamId", teamId);
    //            c.Open();
    //            var reader = cmd.ExecuteReader();
    //            if (reader.HasRows)
    //            {
    //                while (reader.Read())
    //                {
    //                    var p = new ProjectDto
    //                    {
    //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
    //                        Name = reader.GetString(reader.GetOrdinal("Name"))
    //                    };
    //                    projectList.Add(p);
    //                }
    //            }
    //        }

    //        return projectList;
    //    }
    //}
}