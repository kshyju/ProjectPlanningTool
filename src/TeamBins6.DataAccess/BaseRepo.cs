using Microsoft.Extensions.Configuration;

namespace TeamBins.DataAccess
{
    public class BaseRepo
    {
        private IConfiguration configuration;
        public BaseRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected string ConnectionString => configuration.Get<string>("TeamBins:Data:ConnectionString");
    }
}