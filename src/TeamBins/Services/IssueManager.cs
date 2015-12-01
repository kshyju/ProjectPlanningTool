using SmartPlan.ViewModels;
using System;
using System.Collections.Generic;
using System.Web;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ExtensionMethods;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins.Services
{

    public class IssueManager : IIssueManager
    {
        ITeamRepository teamRepository;
        IIssueRepository issueRepository;
        IUserSessionHelper userSessionHelper;
        IActivityRepository activityRepository;
        public IssueManager(IIssueRepository issueRepository, IUserSessionHelper userSessionHelper, IActivityRepository activityRepository, ITeamRepository teamRepository)
        {
            this.userSessionHelper = userSessionHelper;
            this.issueRepository = issueRepository;
            this.activityRepository = activityRepository;
            this.teamRepository = teamRepository;
        }

        public IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count)
        {
            return issueRepository.GetIssues(statusIds, count);
        }

        public IssueDetailVM SaveIssue(CreateIssue issue, List<HttpPostedFileBase> files)
        {
           
            issue.TeamID = userSessionHelper.TeamId;
            issue.CreatedByID = userSessionHelper.UserId;
            issue.ProjectID = GetDefaultProjectId();


            var issueId = issueRepository.SaveIssue(issue);
            return issueRepository.GetIssue(issueId);

            //Activity entry
            //  return SaveActivity(issue, previousVersion, newVersion);
        }


        private void SetUserEditPermissionsForIssue(IssueDetailVM issueVm)
        {
            if (userSessionHelper.UserId > 0)
            {
                var teamMember = teamRepository.GetTeamMember(issueVm.TeamID, userSessionHelper.UserId);
                issueVm.IsEditableForCurrentUser = teamMember != null;
            }
        }

        private int GetDefaultProjectId()
        {
            var teamMember = new TeamRepository().GetTeamMember(userSessionHelper.TeamId, userSessionHelper.UserId);
            if (teamMember != null)
            {
                return teamMember.DefaultProjectId.Value;
            }
            throw new MissingSettingsException("Default project is missing", "Default Project");
        }

        public ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion)
        {
            bool isStateChanged = false;
            var activity = new ActivityDto() { ObjectId = newVersion.ID, ObjectType = "Issue" };

            if (previousVersion == null)
            {
                activity.ObjectUrl = "issue/" + newVersion.ID;
                //activity.CreatedBy = 
                activity.Description = "Created";
                activity.ObjectTite = model.Title;
                isStateChanged = true;
            }
            else
            {
                if (previousVersion.Status.Id != newVersion.Status.Id)
                {
                    // status of issue updated
                    activity.Description = "Changed status";
                    activity.NewState = newVersion.Status.Name;
                    activity.OldState = previousVersion.Status.Name;
                    activity.ObjectTite = newVersion.Title;
                    isStateChanged = true;
                }
                else if (previousVersion.Category.Id != newVersion.Category.Id)
                {
                    activity.Description = "Changed category";
                    activity.NewState = newVersion.Category.Name;
                    activity.OldState = previousVersion.Category.Name;
                    activity.ObjectTite = newVersion.Title;
                    isStateChanged = true;
                }


            }

            activity.TeamId = userSessionHelper.TeamId;

            activity.Actor = new UserDto { Id = userSessionHelper.UserId };

            if (isStateChanged)
            {
                var newId = activityRepository.Save(activity);
                return activityRepository.GetActivityItem(newId);

            }
            return null;
        }

        public ActivityVM GetActivityVM(Activity activity)
        {
            var activityVM = new ActivityVM() { Id = activity.ID, Author = activity.User.FirstName, CreatedDate = activity.CreatedDate.ToJSONFriendlyDateTime() };
            if (activity.ActivityDesc.ToUpper() == "CREATED")
            {
                activityVM.Activity = activity.ActivityDesc;
                activityVM.ObjectTite = activity.NewState;
            }
            else if (activity.ActivityDesc.ToUpper() == "CHANGED STATUS")
            {
                activityVM.Activity = "changed status of";
                activityVM.ObjectTite = activity.OldState;
                activityVM.NewState = "to " + activity.NewState;
            }
            else if (activity.ActivityDesc.ToUpper() == "DUE DATE UPDATED")
            {
                activityVM.Activity = "updated due date of";
                activityVM.ObjectTite = activity.OldState;
                activityVM.NewState = "to " + activity.NewState;
            }
            activityVM.ObjectURL = String.Format("{0}Issues/{1}", "", activity.ObjectID);
            return activityVM;
        }

        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            return issueRepository.GetDashboardSummaryVM(userSessionHelper.TeamId);
        }

        public IssueDetailVM GetIssue(int id)
        {
            var issue = issueRepository.GetIssue(id);
            if (issue != null)
            {
                SetUserEditPermissionsForIssue(issue);
            }
            return issue;
        }

        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count)
        {
            return issueRepository.GetIssuesGroupedByStatusGroup(count);
        }
    }
}