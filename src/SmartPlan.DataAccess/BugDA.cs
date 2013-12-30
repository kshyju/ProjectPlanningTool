using Planner.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace Planner.DataAccess
{
    public class BugDA
    {
   



        public static Bug GetBug(int id)
        {
         //   Bug bug = new Bug();
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var con = new SqlConnection(AppConfiguration.ConnectionString))
                {
                    using (var cmd = new SqlCommand("GetIssuefromID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);                       

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var bug = new Bug();
                                    bug.ID = reader.GetInt32(reader.GetOrdinal("IssueID"));
                                    bug.Title = reader.GetString(reader.GetOrdinal("IssueTitle"));
                                    bug.Description = reader.GetString(reader.GetOrdinal("Description"));
                                    bug.Category.CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));
                                    bug.Category.CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                                    bug.Project.ID = reader.GetInt32(reader.GetOrdinal("ProjectID"));
                                    bug.Project.Name = reader.GetString(reader.GetOrdinal("ProjectName"));
                                    bug.Priority.PriorityID = reader.GetInt32(reader.GetOrdinal("PriorityID"));
                                    bug.Priority.PriorityName = reader.GetString(reader.GetOrdinal("PriorityName"));
                                    bug.Status.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                                    bug.Status.StatusName = reader.GetString(reader.GetOrdinal("StatusName"));
                                    bug.CreatedBy.ID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                    bug.CreatedBy.DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"));
                                    bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                                    return bug;


                                    //if (reader.HasColumn("TotalRows"))
                                    //  product.TotalRows = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("TotalRows")));

                                
                                }
                            }
                        }
                    }
                }
            }
            return null;
           
        }

        public static List<Bug> GetBugs(int page, int size)
        {
            List<Bug> bugList = new List<Bug>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssuesWithPaging", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@page", page);
                    cmd.Parameters.AddWithValue("@size", size);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Bug();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("IssueID"));
                                bug.Title = reader.GetString(reader.GetOrdinal("IssueTitle"));
                                bug.Description = reader.GetString(reader.GetOrdinal("Description"));
                                bug.Category.CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));
                                bug.Category.CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                                bug.Project.ID = reader.GetInt32(reader.GetOrdinal("ProjectID"));
                                bug.Project.Name = reader.GetString(reader.GetOrdinal("ProjectName"));
                                bug.Priority.PriorityID = reader.GetInt32(reader.GetOrdinal("PriorityID"));
                                bug.Priority.PriorityName = reader.GetString(reader.GetOrdinal("PriorityName"));
                                bug.Status.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                                bug.Status.StatusName = reader.GetString(reader.GetOrdinal("StatusName"));
                                bug.CreatedBy.ID = reader.GetInt32(reader.GetOrdinal("UserID"));
                                bug.CreatedBy.DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"));
                                bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));
                                if(!reader.IsDBNull(reader.GetOrdinal("ModifiedDate")))
                                    bug.ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"));
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByID")))
                                    bug.ModifiedBy.ID = reader.GetInt32(reader.GetOrdinal("ModifiedByID"));
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByName")))
                                    bug.ModifiedBy.DisplayName = reader.GetString(reader.GetOrdinal("ModifiedByName"));

                                //if (reader.HasColumn("TotalRows"))
                                //  product.TotalRows = Convert.ToInt32(reader.GetInt64(reader.GetOrdinal("TotalRows")));

                                bugList.Add(bug);
                            }
                        }
                    }
                }
            }
            return bugList;
        }


        public static OperationStatus SaveIssue(Issue issue)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveIssue";
                    cmd.Parameters.AddWithValue("@id", issue.ID);
                    cmd.Parameters.AddWithValue("@title", issue.Title);
                    cmd.Parameters.AddWithValue("@desc", (object)issue.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@categoryId", issue.Category.CategoryID);
                    cmd.Parameters.AddWithValue("@projectId", issue.Project.ID);
                    cmd.Parameters.AddWithValue("@priorityId", issue.Priority.PriorityID);
                    cmd.Parameters.AddWithValue("@statusId", issue.Status.StatusID);
                    cmd.Parameters.AddWithValue("@cycle", issue.Status.StatusID);
                    cmd.Parameters.AddWithValue("@modifiedBy", issue.ModifiedBy.ID);
                    cmd.Parameters.AddWithValue("@teamId", issue.Team.ID);
                    if(issue.DueDate.Year>2000)
                        cmd.Parameters.AddWithValue("@dueDate", issue.DueDate);
                    else
                        cmd.Parameters.AddWithValue("@dueDate", DBNull.Value);


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
    }
}
