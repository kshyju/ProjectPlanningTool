using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planner.DataAccess;
using Planner.Entities;
using TeamBins.Entities;
using Dapper;
using TechiesWeb.TeamBins.Entities;
namespace SmartPlan.DataAccess
{
    class TeamDA
    {
        public static OperationStatus SaveTeam(Team team)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveTeam";
                    cmd.Parameters.AddWithValue("@name", team.Name);
                    cmd.Parameters.AddWithValue("@userId", team.CreatedBy.ID);
                  
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
        public static List<Team> GetTeams(int userId)
        {
            List<Team> bugList = new List<Team>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("SELECT * FROM Team", con))
                {
                    cmd.CommandType = CommandType.Text;
                   
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Team();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("TeamID"));
                                bug.Name = reader.GetString(reader.GetOrdinal("TeamName"));


                                bugList.Add(bug);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

        public static Team GetTeamFromID(int teamId)
        {
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var con = new SqlConnection(AppConfiguration.ConnectionString))
                {
                    using (var cmd = new SqlCommand("GetTeam", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@teamId", teamId);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                
                                var team = new Team();
                                team.ID = reader.GetInt32(reader.GetOrdinal("TeamID"));
                                team.Name = reader.GetString(reader.GetOrdinal("TeamName"));
                                return team;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static List<User> GetTeamMembers(int teamId)
        {
            List<User> bugList = new List<User>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetAllTeamMembers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@teamId", teamId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var member = new User();
                                member.ID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                member.DisplayName = reader.GetString(reader.GetOrdinal("Name"));
                                member.JobTitle = reader.GetString(reader.GetOrdinal("JobTitle"));
                                member.EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress"));

                                bugList.Add(member);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

        public static List<User> GetNonIssueMembers(int teamId, int issueId)
        {
            List<User> bugList = new List<User>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetNonIssueMembers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@teamId", teamId);
                    cmd.Parameters.AddWithValue("@issueId", issueId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var member = new User();
                                member.ID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                member.DisplayName = reader.GetString(reader.GetOrdinal("Name"));
                                member.JobTitle = reader.GetString(reader.GetOrdinal("JobTitle"));
                                member.EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress"));

                                bugList.Add(member);
                            }
                        }
                    }
                }
            }
            return bugList;
        }


        public static OperationStatus SaveTeamMemberRequest(TeamMemberRequest request)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "CreateTeamMemberReq";
                    cmd.Parameters.AddWithValue("@email", request.EmailAddress);
                    cmd.Parameters.AddWithValue("@addedBy", request.CreatedBy.ID);
                    cmd.Parameters.AddWithValue("@teamId", request.Team.ID);
                    cmd.Parameters.AddWithValue("@acode", request.ActivationCode);

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

        public static OperationStatus SaveActivity(Activity activity)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveTeamActivity";
                    cmd.Parameters.AddWithValue("@teamId", activity.TeamID);
                    cmd.Parameters.AddWithValue("@itemId", activity.ItemID);
                    cmd.Parameters.AddWithValue("@itemName", activity.ItemName);
                    cmd.Parameters.AddWithValue("@itemType", activity.ItemType);
                    cmd.Parameters.AddWithValue("@action", activity.Action);
                    cmd.Parameters.AddWithValue("@newState", (object)activity.NewState??DBNull.Value);
                    cmd.Parameters.AddWithValue("@userId", activity.CreatedBy.ID);
                    

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

        public static List<Activity> GetTeamActivity(int teamId)
        {
            List<Activity> bugList = new List<Activity>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetTeamActivity", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@teamId", teamId);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var activity = new Activity();
                                activity.ID = reader.GetInt32(reader.GetOrdinal("ActivityID"));
                                activity.ItemID = reader.GetInt32(reader.GetOrdinal("ItemID"));
                                activity.ItemName = reader.GetString(reader.GetOrdinal("ItemName"));
                                activity.ItemType = reader.GetString(reader.GetOrdinal("ItemType"));
                                 activity.Action = reader.GetString(reader.GetOrdinal("Activity"));
                                
                                if(!reader.IsDBNull(reader.GetOrdinal("NewState")))
                                    activity.NewState = reader.GetString(reader.GetOrdinal("NewState"));

                                activity.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                                activity.CreatedBy.ID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                activity.CreatedBy.DisplayName = reader.GetString(reader.GetOrdinal("Name"));

                                bugList.Add(activity);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

    }
}
