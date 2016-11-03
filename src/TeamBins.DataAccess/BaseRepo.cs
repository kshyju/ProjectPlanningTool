
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;


namespace TeamBins.DataAccessCore
{


    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BaseRepo
    {
        private IConfiguration configuration;
        public BaseRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected string ConnectionString => configuration.GetSection("TeamBins:Data:ConnectionString").Value;
    }

    //public interface ICommentRepository
    //{
    //    int Save(CommentVM comment);
    //    CommentVM GetComment(int id);

    //    IEnumerable<CommentVM> GetComments(int issueId);
    //    void Delete(int id);
    //}


}
