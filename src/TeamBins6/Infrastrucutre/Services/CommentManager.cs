using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;


namespace TeamBins.Services
{
    public interface ICommentManager
    {
        IEnumerable<CommentVM> GetComments(int issueId);
        int SaveComment(CommentVM comment);
        ActivityDto SaveActivity(int commentId, int issueId);

        CommentVM GetComment(int id);

        Task SendEmailNotificaionForNewComment(CommentVM comment);
        Task Delete(int id);
    }
    //public class CommentManager : ICommentManager
    //{
    //    IUserRepository userRepository;
    //    IIssueRepository issueRepository;
    //    ICommentRepository commentRepository;
    //    IUserSessionHelper userSessionHelper;
    //    IActivityRepository activityRepository;
    //    ICommentEmailManager commentEmailManager;
    //    public CommentManager(ICommentRepository commentRepository, IIssueRepository issueRepository,
    //        IActivityRepository activityRepository,
    //        IUserRepository userRepository,
    //        ICommentEmailManager commentEmailManager,
    //        IUserSessionHelper userSessionHelper)
    //    {
    //        this.commentRepository = commentRepository;
    //        this.issueRepository = issueRepository;
    //        this.userSessionHelper = userSessionHelper;
    //        this.activityRepository = activityRepository;
    //        this.userRepository = userRepository;
    //        this.commentEmailManager = commentEmailManager;
    //    }
    //    public IEnumerable<CommentVM> GetComments(int issueId)
    //    {
    //        var comments = commentRepository.GetComments(issueId);
    //        foreach (var commentVm in comments)
    //        {
    //            commentVm.IsOwner = commentVm.Author.Id == userSessionHelper.UserId;
    //            commentVm.Author.GravatarUrl = UserService.GetImageSource(commentVm.Author.EmailAddress, 42);
    //        }
    //        return comments;
    //    }

    //    public CommentVM GetComment(int id)
    //    {
    //        var commentVm = commentRepository.GetComment(id);
    //        commentVm.IsOwner = commentVm.Author.Id == userSessionHelper.UserId;
    //        commentVm.Author.GravatarUrl = UserService.GetImageSource(commentVm.Author.EmailAddress, 42);
    //        return commentVm;
    //    }

    //    public ActivityDto SaveActivity(int commentId, int issueId)
    //    {
    //        var issue = issueRepository.GetIssue(issueId);
    //        if (issue != null)
    //        {
    //            var activity = new ActivityDto() {ObjectId = commentId, ObjectType = "Comment"};
    //            activity.Description = "Commented";
    //            //activity.NewState = newVersion.Status.Name;
    //            // activity.OldState = previousVersion.Status.Name;
    //            activity.ObjectTite = issue.Title;
    //            activity.TeamId = userSessionHelper.TeamId;

    //            activity.Actor = new UserDto {Id = userSessionHelper.UserId};


    //            var newId = activityRepository.Save(activity);
    //            return activityRepository.GetActivityItem(newId);
                
    //        }
    //        return null;
    //    }

    //    public int SaveComment(CommentVM comment)
    //    {
    //        return commentRepository.Save(comment);
    //    }
    //    public async Task SendEmailNotificaionForNewComment(CommentVM comment)
    //    {
    //        await commentEmailManager.SendNewCommentEmail(comment,userSessionHelper.TeamId);
    //    }

    //    public async Task Delete(int id)
    //    {
    //        await commentRepository.Delete(id);
    //    }
    //}
}