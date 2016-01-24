using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Services;

namespace TeamBins6.Infrastrucutre.Services
{
    public interface IUserSessionHelper
    {
        int TeamId { get; }
        int UserId { get; }
    }
    public class UserSessionHelper : IUserSessionHelper
    {
        public int TeamId
        {
            get { return 12105; }
        }

        public int UserId
        {
            get { return 1; }
        }
    }
}
