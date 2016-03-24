using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using TeamBins.Services;

namespace TeamBins6.Infrastrucutre.Services
{
    public interface IUserSessionHelper
    {
        int TeamId { get; }
        int UserId { get; }
        void SetTeamId(int teamId);
        void SetUserIDToSession(int userId, int teamId);
        void SetUserId(int userId);

    }
    public class UserSessionHelper : IUserSessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session; 

        public UserSessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }

        private const string teamIdKey = "TB_TeamId";
        private const string userIdKey = "TB_UserId";
        private const string DefaultTeamIdKey = "TB_DefaultTeamId";
        public int TeamId
        {
            get
            {
                if (_session != null && _session.GetInt32(teamIdKey).HasValue)
                {
                    return _session.GetInt32(teamIdKey).Value;
                }
                return 12101;
            }
        }

        public void SetTeamId(int teamId)
        {
            _session.SetInt32(teamIdKey, teamId);
        }
        public void SetUserId(int userId)
        {
            _session.SetInt32(userIdKey, userId);
        }

        public void SetUserIDToSession(int userId, int teamId)
        {
            _session.SetInt32(userIdKey, userId);
            _session.SetInt32(teamIdKey,teamId);
          
        }
        public int UserId
        {
            get
            {
                if (_session != null && _session.GetInt32(userIdKey).HasValue)
                {
                    return _session.GetInt32(userIdKey).Value;
                }
                return 1;
            }
        }

    }
}
