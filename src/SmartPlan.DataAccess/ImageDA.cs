using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Planner.Entities;
using TechiesWeb.TeamBins.Entities;
namespace Planner.DataAccess
{
    public class ImageDA
    {
        public static OperationStatus SaveDocument(Document model)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveDocument";
                    cmd.Parameters.AddWithValue("@name", model.DocName);
                    cmd.Parameters.AddWithValue("@parentId", model.ParentID);
                    cmd.Parameters.AddWithValue("@key", model.DocKey);
                    cmd.Parameters.AddWithValue("@extn", model.Extension);
                    cmd.Parameters.AddWithValue("@createdById", model.CreatedByID);

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

        public static List<Document> GetDocuments(int parentId, string type)
        {
            List<Document> bugList = new List<Document>();
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssueDocuments", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@issueId", parentId);                 

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var bug = new Document();
                                bug.DocID = reader.GetInt32(reader.GetOrdinal("DocumentID"));
                                bug.DocName = reader.GetString(reader.GetOrdinal("DocumentName"));
                                bug.DocKey = reader.GetString(reader.GetOrdinal("DocumentKey"));
                                bug.Extension = reader.GetString(reader.GetOrdinal("DocumentExtn"));
                                

                                bugList.Add(bug);
                            }
                        }
                    }
                }
            }
            return bugList;
        }

        public static Document GetDocument(string docKey)
        {
            using (var con = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand("GetIssueDocumentFromKey", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@docKey", docKey);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            
                            var bug = new Document();
                            bug.DocID = reader.GetInt32(reader.GetOrdinal("DocumentID"));
                            bug.DocName = reader.GetString(reader.GetOrdinal("DocumentName"));
                            bug.DocKey = reader.GetString(reader.GetOrdinal("DocumentKey"));
                            bug.Extension = reader.GetString(reader.GetOrdinal("DocumentExtn"));
                            return bug;
                            
                        }
                    }
                }
            }
            return null;
        }

    }
}
