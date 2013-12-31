using Planner.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechiesWeb.TeamBins.Entities;
using System.Data.Common;
namespace Planner.DataAccess
{
    public class ProjectDA
    {
        public static List<Project> GetProjectss()
        {
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                conn.Open();
                string qry = "  SELECT * FROM Project";
                var projects = conn.Query<Project>(qry).ToList();
                return projects;
            }
        }

        public static List<Project> GetProjects(int teamId)
        {
            List<Project> bugList = new List<Project>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM Project WHERE TeamID=" + teamId, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@teamId", teamId);


                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Project();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                                bug.Name = reader.GetString(reader.GetOrdinal("Name"));
                                if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                    bug.Description = reader.GetString(reader.GetOrdinal("Description"));
                                                             

                                bugList.Add(bug);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

        public static OperationStatus SaveProject(Project model)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveProject";
                    cmd.Parameters.AddWithValue("@id", model.ID);
                    cmd.Parameters.AddWithValue("@name", model.Name);
                    cmd.Parameters.AddWithValue("@descr", (object)model.Description??DBNull.Value);
                    cmd.Parameters.AddWithValue("@teamId", model.Team.ID);
                    cmd.Parameters.AddWithValue("@createdBy", model.CreatedBy.ID);
                  

                    DbParameter returnValue;
                    returnValue = cmd.CreateParameter();
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(returnValue);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(returnValue.Value);
                    conn.Close();
                    opStatus.OperationID = result;
                    opStatus.Status = true;
                }
            }
            return opStatus;
        }

        public static Project GetProject(string name, int teamId)
        {
            string query="SELECT TOP 1 * FROM Project WHERE TeamID=" + teamId+" AND Name='"+name+"'";  
            return GetProject(query);
        }

        private static Project GetProject(string query)
        {
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Project();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                                bug.Name = reader.GetString(reader.GetOrdinal("Name"));
                                if (!reader.IsDBNull(reader.GetOrdinal("Description")))
                                    bug.Description = reader.GetString(reader.GetOrdinal("Description"));
                                return bug;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static Project GetProject(int id, int teamId)
        {
            string query = "SELECT TOP 1 * FROM Project WHERE TeamID=" + teamId + " AND ID=" + id;
            return GetProject(query);
        }
        public static OperationStatus DeleteProject(int projectId, int teamId)
        {
            string query = "DELETE FROM Project WHERE TeamID=@teamId AND ID=@projectId";

            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@teamId", teamId);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                  
                    conn.Open();
                    cmd.ExecuteNonQuery();                  
                    conn.Close();                  
                    opStatus.Status = true;
                }
            }
            return opStatus;

        }

    }
}
