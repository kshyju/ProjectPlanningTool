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

namespace Planner.DataAccess
{
    public class PriorityDA
    {
        public static List<Priority> GetProjects()
        {
            using (SqlConnection conn = new SqlConnection(AppConfiguration.ConnectionString))
            {
                conn.Open();
                string qry = "  SELECT * FROM Priority";
                var priorities = conn.Query<Priority>(qry).ToList();
                return priorities;
            }
        }

    }
}
