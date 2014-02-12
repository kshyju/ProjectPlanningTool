using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;

namespace TeamBins.Services
{
    public class CommentService : IActivitySavable
    {
        IRepositary repo;
        public CommentService(IRepositary repositary,string siteBaseUrl)
        {
            repo = new Repositary();
            SiteBaseURL = siteBaseUrl;
        }
        public string SiteBaseURL { set; get; }

        public CommentVM GetCommentVM(int commentId)
        {
            var commentVM = new CommentVM();
            var comment = repo.GetComment(commentId);
            if (comment != null)
            {
                commentVM.ID = comment.ID;
                commentVM.AuthorName = comment.Author.FirstName;
                commentVM.CommentBody = comment.CommentText;
                commentVM.CreativeDate = comment.CreatedDate.ToString("g");
                commentVM.AvatarHash = UserService.GetImageSource(comment.Author.EmailAddress, 42);
                commentVM.CreatedDateRelative = comment.CreatedDate.ToShortDateString(); //.ToRelativeDateTime();
            }
            return commentVM;
        }
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

        public ActivityVM GetActivityVM(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public Activity SaveActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }
    }
}