using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;

namespace TeamBins.Services
{
    public interface IProjectManager
    {
        //GetProjects(int teamId);

        bool DoesProjectsExist();

        int GetDefaultProjectForCurrentTeam();

        IEnumerable<ProjectDto> GetProjects();

    }

    public class ProjectManager : IProjectManager
    {
        IProjectRepository projectRepository;
        private IUserSessionHelper userSessionHelper;

        public ProjectManager(IProjectRepository projectRepository, IUserSessionHelper userSessionHelper)
        {
            this.projectRepository = projectRepository;
            this.userSessionHelper = userSessionHelper;
        }

        public bool DoesProjectsExist()
        {
            return this.projectRepository.DoesProjectsExist(this.userSessionHelper.TeamId);
        }

        public int GetDefaultProjectForCurrentTeam()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProjectDto> GetProjects()
        {
            throw new NotImplementedException();
        }
    }

    //public class CommentService : IActivitySavable
    //{
    //    IRepositary repo;
    //    public CommentService(IRepositary repositary,string siteBaseUrl)
    //    {
    //        repo = new Repositary();
    //        SiteBaseURL = siteBaseUrl;
    //    }
    //    public string SiteBaseURL { set; get; }

    //    public CommentVM GetCommentVM(int commentId)
    //    {
    //        var commentVM = new CommentVM();
    //        var comment = repo.GetComment(commentId);
    //        if (comment != null)
    //        {
    //            commentVM.ID = comment.ID;
    //            commentVM.AuthorName = comment.Author.FirstName;
    //            commentVM.CommentBody = comment.CommentText.ConvertUrlsToLinks();
    //            commentVM.CreativeDate = comment.CreatedDate;
    //            commentVM.AvatarHash = UserService.GetAvatarUrl(comment.Author.Avatar, 42);
    //            commentVM.CreatedDateRelative = comment.CreatedDate.ToShortDateString(); //.ToRelativeDateTime();
    //        }
    //        return commentVM;
    //    }
    //    public ActivityVM GetActivityVM(Activity activity)
    //    {
    //        var activityVM = new ActivityVM() { Id = activity.ID, Author = activity.User.FirstName };
    //        activityVM.CreatedDate = activity.CreatedDate.ToJSONFriendlyDateTime();
    //        if (activity.ActivityDesc.ToUpper() == "COMMENTED")
    //        {
    //            activityVM.Activity = activity.ActivityDesc+" on";
    //            activityVM.ObjectTite = activity.OldState;
    //            var seoFriendlyIssueTitle = activity.OldState.Replace(" ", "-").Replace("/","-");
    //            if (seoFriendlyIssueTitle.Length > 40)
    //                seoFriendlyIssueTitle = seoFriendlyIssueTitle.Substring(0, 39);

    //            activityVM.ObjectURL = String.Format("{0}issuecomment/{1}/"+seoFriendlyIssueTitle, SiteBaseURL, activity.ObjectID);
    //        }
    //        return activityVM;
    //    }

    //    public ActivityVM GetActivityVM(IActivity activity)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Activity SaveActivity(IActivity activity)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}