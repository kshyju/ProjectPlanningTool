using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.Services;

namespace TeamBins6.Infrastrucutre.Services
{
    public class SessionUserAuthHelper : IUserAuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session;

        public SessionUserAuthHelper(IHttpContextAccessor httpContextAccessor)
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
                return 0;
            }
        }

        public void Logout()
        {
            _session.Clear();
        }
        public void SetTeamId(int teamId)
        {
            _session.SetInt32(teamIdKey, teamId);
        }
        public void SetUserId(int userId)
        {
            _session.SetInt32(userIdKey, userId);
        }

        public void SetUserIDToSession(LoggedInSessionInfo loggedInSessionInfo)
        {
            _session.SetInt32(userIdKey, loggedInSessionInfo.UserId);
            _session.SetInt32(teamIdKey, loggedInSessionInfo.TeamId);

        }
        public void SetUserIDToSession(int userId, int teamId)
        {
            _session.SetInt32(userIdKey, userId);
            _session.SetInt32(teamIdKey, teamId);

        }
        public int UserId
        {
            get
            {
                if (_session != null && _session.GetInt32(userIdKey).HasValue)
                {
                    return _session.GetInt32(userIdKey).Value;
                }
                return 0;
            }
        }
    }

    //public interface IUserAuthHelper
    //{
    //    //int TeamId { get; }
    // //   int UserId { get; }
    //    void SetTeamId(int teamId);
    //    void SetUserIDToSession(int userId, int teamId);
    //    void SetUserId(int userId);

    //    void SetUserIDToSession(LoggedInSessionInfo loggedInSessionInfo);

    //    void Logout();

    //}

    //public class TestSessionHelper : IUserAuthHelper
    //{
    //    public int TeamId
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public int UserId
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public void Logout()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetTeamId(int teamId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetUserId(int userId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetUserIDToSession(LoggedInSessionInfo loggedInSessionInfo)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void SetUserIDToSession(int userId, int teamId)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class UserSessionHelper : IUserAuthHelper
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private ISession _session; 

    //    public UserSessionHelper(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //        _session = _httpContextAccessor.HttpContext.Session;
    //    }

    //    private const string teamIdKey = "TB_TeamId";
    //    private const string userIdKey = "TB_UserId";
    //    private const string DefaultTeamIdKey = "TB_DefaultTeamId";
    //    public int TeamId
    //    {
    //        get
    //        {
    //            if (_session != null && _session.GetInt32(teamIdKey).HasValue)
    //            {
    //                return _session.GetInt32(teamIdKey).Value;
    //            }
    //            return 0;
    //        }
    //    }

    //    public void Logout()
    //    {
    //        _session.Clear();
    //    }
    //    public void SetTeamId(int teamId)
    //    {
    //        _session.SetInt32(teamIdKey, teamId);
    //    }
    //    public void SetUserId(int userId)
    //    {
    //        _session.SetInt32(userIdKey, userId);
    //    }

    //    public void SetUserIDToSession(LoggedInSessionInfo loggedInSessionInfo)
    //    {
    //        _session.SetInt32(userIdKey, loggedInSessionInfo.UserId);
    //        _session.SetInt32(teamIdKey, loggedInSessionInfo.TeamId);

    //    }
    //    public void SetUserIDToSession(int userId, int teamId)
    //    {
    //        _session.SetInt32(userIdKey, userId);
    //        _session.SetInt32(teamIdKey,teamId);

    //    }
    //    public int UserId
    //    {
    //        get
    //        {
    //            if (_session != null && _session.GetInt32(userIdKey).HasValue)
    //            {
    //                return _session.GetInt32(userIdKey).Value;
    //            }
    //            return 0;
    //        }
    //    }

    //}
}
