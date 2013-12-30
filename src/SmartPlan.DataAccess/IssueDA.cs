using Planner.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;
using TechiesWeb.TeamBins.Entities;

namespace Planner.DataAccess
{
    public class IssueDA
    {
        public static Comment GetCommentFromID(int issueId)
        {
            //   Bug bug = new Bug();
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var con = new SqlConnection(AppConfiguration.ConnectionString))
                {
                    using (var cmd = new SqlCommand("GetIssueCommentFromID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@commentId", issueId);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                var bug = new Comment();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("CommentID"));
                                bug.CommentBody = reader.GetString(reader.GetOrdinal("Comment"));

                                bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));

                                if (!reader.IsDBNull(reader.GetOrdinal("AuthorName")))
                                    bug.Author.DisplayName = reader.GetString(reader.GetOrdinal("AuthorName"));

                                if (!reader.IsDBNull(reader.GetOrdinal("AuthorEmail")))
                                    bug.Author.EmailAddress = reader.GetString(reader.GetOrdinal("AuthorEmail"));

                                return bug;
                            }
                        }
                    }
                }
            }
            return null;

        }
        public static List<Comment> GetCommentsForIssue(int issueId)
        {
            List<Comment> bugList = new List<Comment>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssueComments", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@issueId", issueId);


                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Comment();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("CommentID"));
                                bug.CommentBody = reader.GetString(reader.GetOrdinal("Comment"));
                 
                                bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));

                                if (!reader.IsDBNull(reader.GetOrdinal("AuthorName")))
                                    bug.Author.DisplayName = reader.GetString(reader.GetOrdinal("AuthorName"));

                                if (!reader.IsDBNull(reader.GetOrdinal("AuthorEmail")))
                                    bug.Author.EmailAddress = reader.GetString(reader.GetOrdinal("AuthorEmail"));
                                
                                bugList.Add(bug);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

        public static List<Issue> GetBugs(int teamId)
        {
            List<Issue> bugList = new List<Issue>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssues", con))
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
                                var bug = new Issue();
                                bug.ID = reader.GetInt32(reader.GetOrdinal("IssueID"));
                                bug.Title = reader.GetString(reader.GetOrdinal("IssueTitle"));
                                if (!reader.IsDBNull(reader.GetOrdinal("IssueDescription")))
                                    bug.Description = reader.GetString(reader.GetOrdinal("IssueDescription"));

                                bug.Category.CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));
                                bug.Category.CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                                bug.Project.ID = reader.GetInt32(reader.GetOrdinal("ProjectID"));
                                bug.Project.Name = reader.GetString(reader.GetOrdinal("ProjectName"));

                                bug.Priority.PriorityID = reader.GetInt32(reader.GetOrdinal("PriorityID"));
                                bug.Priority.PriorityName = reader.GetString(reader.GetOrdinal("PriorityName"));

                                bug.Status.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                                bug.Status.StatusName = reader.GetString(reader.GetOrdinal("StatusName"));

                                bug.CreatedBy.ID = reader.GetInt32(reader.GetOrdinal("CreatedByID"));
                                bug.CreatedBy.DisplayName = reader.GetString(reader.GetOrdinal("CreatedByName"));

                                bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));

                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedDate")))
                                    bug.ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"));
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByID")))
                                    bug.ModifiedBy.ID = reader.GetInt32(reader.GetOrdinal("ModifiedByID"));
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByName")))
                                    bug.ModifiedBy.DisplayName = reader.GetString(reader.GetOrdinal("ModifiedByName"));
                                
                                if (!reader.IsDBNull(reader.GetOrdinal("Iteration")))
                                    bug.Iteration = reader.GetString(reader.GetOrdinal("Iteration"));

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
      
        public static Issue GetIssue(int issueId)
        {
            //   Bug bug = new Bug();
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var con = new SqlConnection(AppConfiguration.ConnectionString))
                {
                    using (var cmd = new SqlCommand("GetIssuefromID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@issueId", issueId);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var bug = new Issue();
                                    bug.ID = reader.GetInt32(reader.GetOrdinal("IssueID"));
                                    bug.Title = reader.GetString(reader.GetOrdinal("IssueTitle"));
                                    if (!reader.IsDBNull(reader.GetOrdinal("IssueDescription")))
                                        bug.Description = reader.GetString(reader.GetOrdinal("IssueDescription"));

                                    bug.Category.CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));
                                    bug.Category.CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                                    bug.Project.ID = reader.GetInt32(reader.GetOrdinal("ProjectID"));
                                    bug.Project.Name = reader.GetString(reader.GetOrdinal("ProjectName"));

                                    bug.Priority.PriorityID = reader.GetInt32(reader.GetOrdinal("PriorityID"));
                                    bug.Priority.PriorityName = reader.GetString(reader.GetOrdinal("PriorityName"));

                                    bug.Status.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                                    bug.Status.StatusName = reader.GetString(reader.GetOrdinal("StatusName"));
                                    bug.Status.StatusCode = reader.GetString(reader.GetOrdinal("StatusCode"));

                                    bug.CreatedBy.ID = reader.GetInt32(reader.GetOrdinal("CreatedByID"));
                                    bug.CreatedBy.DisplayName = reader.GetString(reader.GetOrdinal("CreatedByName"));

                                    bug.CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"));

                                    if (!reader.IsDBNull(reader.GetOrdinal("ModifiedDate")))
                                        bug.ModifiedDate = reader.GetDateTime(reader.GetOrdinal("ModifiedDate"));
                                    if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByID")))
                                        bug.ModifiedBy.ID = reader.GetInt32(reader.GetOrdinal("ModifiedByID"));
                                    if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByName")))
                                        bug.ModifiedBy.DisplayName = reader.GetString(reader.GetOrdinal("ModifiedByName"));

                                    if (!reader.IsDBNull(reader.GetOrdinal("DueDate")))
                                        bug.DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate"));

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
        public static OperationStatus DeleteIssueMember(int issueId, int memberId)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DeleteIssueMember";
                    cmd.Parameters.AddWithValue("@issueId", issueId);
                    cmd.Parameters.AddWithValue("@memberId", memberId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    opStatus.Status = true;
                }
            }
            return opStatus;
        }
        public static OperationStatus SaveIssueMember(int issueId, int memberId,int addedBy)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveIssueMember";
                    cmd.Parameters.AddWithValue("@issueId", issueId);
                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    cmd.Parameters.AddWithValue("@createdBy", addedBy);                                     

                    conn.Open();
                    cmd.ExecuteNonQuery();                   
                    conn.Close();                   
                    opStatus.Status = true;
                }
            }
            return opStatus;
        }
        public static List<User> GetIssueMembers(int issueId)
        {
            List<User> bugList = new List<User>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssueMembers", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
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

        public static OperationStatus SaveComment(Comment comment)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveComment";
                    cmd.Parameters.AddWithValue("@body", comment.CommentBody);
                    cmd.Parameters.AddWithValue("@issueId", comment.ParentID);
                    cmd.Parameters.AddWithValue("@createdBy", comment.Author.ID);
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
