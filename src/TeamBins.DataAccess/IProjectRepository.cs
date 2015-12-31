using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using TeamBins.Common;

namespace TeamBins.DataAccess
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectDto> GetProjects(int teamId);
        bool DoesProjectsExist(int teamId);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly TeamEntitiesConn db;
        public ProjectRepository()
        {
            db = new TeamEntitiesConn();
        }

        public bool DoesProjectsExist(int teamId)
        {
            return db.Projects.Any(s => s.TeamID == teamId);
        }

        public IEnumerable<ProjectDto> GetProjects(int teamId)
        {
            var projectList = new List<TeamBins.Common.ProjectDto>();
            
            using (var c = new SqlConnection(db.Database.Connection.ConnectionString))
            {

                var cmd = new SqlCommand("SELECT ID,Name from Project where TeamId=@teamId",c);
                cmd.Parameters.AddWithValue("teamId", teamId);
                c.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var p = new ProjectDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ID")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                        projectList.Add(p);
                    }
                }
            }

            return projectList;
        }
    }
}