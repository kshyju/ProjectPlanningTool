using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Common;

namespace Planner.DataAccess
{
    public class AppConfiguration
    {
        #region Public methods
        public static string ConnectionString
        {
            get
            {

                // return HttpContext.Current.Application["dsn"] as string;
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
        }
        #endregion
    }
}