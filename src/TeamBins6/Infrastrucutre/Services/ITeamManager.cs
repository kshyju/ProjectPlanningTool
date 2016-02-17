using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Routing;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;

namespace TeamBins6.Infrastrucutre.Services
{
    public interface ITeamManager
    {
      //  List<TeamDto> GetTeams();
        IEnumerable<ActivityDto> GeActivityItems(int count);
        bool DoesCurrentUserBelongsToTeam();
    }
    public class TeamManager : ITeamManager
    {
        IActivityRepository activityRepository;
        IUserSessionHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        public TeamManager( IUserSessionHelper userSessionHelper, IActivityRepository activityRepository, ITeamRepository teamRepository)
        {
           this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
            this.activityRepository = activityRepository;
        }

        public bool DoesCurrentUserBelongsToTeam()
        {
            var member = this.teamRepository.GetTeamMember(this.userSessionHelper.TeamId, this.userSessionHelper.UserId);
            return member != null;

        }
        //public List<TeamDto> GetTeams()
        //{
        //   // return teamRepository.GetTeams(userSessionHelper.UserId);
        //}

        public IEnumerable<ActivityDto> GeActivityItems(int count)
        {
            var activities = activityRepository.GetActivityItems(userSessionHelper.TeamId,count);

            foreach (var activity in activities)
            {
                if (activity.ObjectType == "Issue")
                {
                    activity.ObjectUrl = "Issue/" + activity.ObjectId;
                    if (activity.Description.ToUpper() == "CREATED")
                    {
                        activity.NewState = "";
                    }
                    else if (activity.Description.ToUpper() == "CHANGED STATUS")
                    {
                        activity.Description = "changed status of";

                        activity.NewState = "from " + activity.OldState + " to " + activity.NewState;
                    }
                    else if (activity.Description.ToUpper() == "DUE DATE UPDATED")
                    {
                        activity.Description = "updated due date of";
                        activity.NewState = "to " + activity.NewState;
                    }
                }
            }

            return activities;
            ;
        }

    }
}
