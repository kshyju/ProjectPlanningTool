using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins.Services
{
    public class CommentService : IActivity
    {
        public CommentService(string siteBaseUrl)
        {
            SiteBaseURL = siteBaseUrl;
        }
        public string SiteBaseURL { set; get; }
        public ActivityVM GetActivityVM(Activity activity)
        {
            var activityVM = new ActivityVM() { Author = activity.User.FirstName, CreatedDateRelative = activity.CreatedDate.ToString() };
            if (activity.ActivityDesc.ToUpper() == "COMMENTED")
            {
                activityVM.Activity = activity.ActivityDesc+" on";
                activityVM.ObjectTite = activity.OldState;
                var seoFriendlyIssueTitle = activity.OldState.Replace(" ", "-");
                if (seoFriendlyIssueTitle.Length > 40)
                    seoFriendlyIssueTitle = seoFriendlyIssueTitle.Substring(0, 39);

                activityVM.ObjectURL = String.Format("{0}issuecomment/{1}/"+seoFriendlyIssueTitle, SiteBaseURL, activity.ObjectID);
            }
            return activityVM;
        }
    }
}