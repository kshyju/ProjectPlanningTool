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
    public class UserDA
    {
        public static OperationStatus SaveUser(User user)
        {
            var opStatus = new OperationStatus();
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SaveUser";
                    cmd.Parameters.AddWithValue("@name", user.DisplayName);
                    cmd.Parameters.AddWithValue("@email", user.EmailAddress);
                    cmd.Parameters.AddWithValue("@pass", user.Password);
                    cmd.Parameters.AddWithValue("@salt", user.PasswordSalt);
                    cmd.Parameters.AddWithValue("@siteId", user.SiteID);

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

        public static User GetUserFromEmail(string email)
        {
            using (var conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                conn.Open();
                var img = conn.Query<User>("SELECT * FROM [User] WHERE EmailAddress=@email", new { email = email }).FirstOrDefault();
                return img;
            }
        }

    }
}
