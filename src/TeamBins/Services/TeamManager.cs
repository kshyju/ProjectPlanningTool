using System.Collections.Generic;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class TeamManager : ITeamManager
    {
        IActivityRepository activityRepository;
        IUserSessionHelper userSessionHelper;
        private readonly ITeamRepository teamRepository;
        public TeamManager(ITeamRepository teamRepository, IUserSessionHelper userSessionHelper, IActivityRepository activityRepository)
        {
            this.teamRepository = teamRepository;
            this.userSessionHelper = userSessionHelper;
            this.activityRepository = activityRepository;
        }

        public List<TeamDto> GetTeams()
        {
            return teamRepository.GetTeams(userSessionHelper.UserId);
        }

        public List<ActivityDto> GeActivityItems(int count)
        {
            var activities = activityRepository.GetActivityItems(count);

            foreach (var activity in activities)
            {
                if (activity.ObjectType == "Issue")
                {
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