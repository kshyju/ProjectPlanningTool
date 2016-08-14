using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccessCore;
using TeamBins6.Infrastrucutre.Extensions;
using TeamBins6.Infrastrucutre.Services;
using TeamBinsCore.DataAccess;

namespace TeamBins.Services
{
    public interface ICommentManager
    {
        IEnumerable<CommentVM> GetComments(int issueId);
        int SaveComment(CommentVM comment);
        ActivityDto SaveActivity(int commentId, int issueId);

        CommentVM GetComment(int id);

        Task SendEmailNotificaionForNewComment(CommentVM comment);
        void Delete(int id);
    }

    public class CommentManager : ICommentManager
    {
        private readonly IActivityRepository activityRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IIssueRepository issueRepository;
        private readonly IUserAuthHelper userSessionHelper;
        public CommentManager(ICommentRepository commentRepository,IUserAuthHelper userSessionHelper, IIssueRepository issueRepository, IActivityRepository activityRepository)
        {
            this.commentRepository = commentRepository;
            this.userSessionHelper = userSessionHelper;
            this.issueRepository = issueRepository;
            this.activityRepository = activityRepository;
        }
        public void Delete(int id)
        {
            commentRepository.Delete(id);
        }

        public CommentVM GetComment(int id)
        {
            CommentVM commentVm = this.commentRepository.GetComment(id);
            if (commentVm != null)
            {
                commentVm.Author.GravatarUrl = commentVm.Author.EmailAddress.ToGravatarUrl();
                commentVm.IsOwner = commentVm.Author.Id == this.userSessionHelper.UserId;
            }
            return commentVm;
        }

        public IEnumerable<CommentVM> GetComments(int issueId)
        {
            var c= this.commentRepository.GetComments(issueId);
            foreach (var commentVm in c)
            {
                commentVm.Author.GravatarUrl = commentVm.Author.EmailAddress.ToGravatarUrl();
                commentVm.IsOwner = commentVm.Author.Id == this.userSessionHelper.UserId;
            }
            return c;
        }

        
        public int SaveComment(CommentVM comment)
        {
            comment.Author = new UserDto {Id = this.userSessionHelper.UserId};
            int commentId= this.commentRepository.Save(comment);

          

            return commentId;
        }

        public ActivityDto SaveActivity(int commentId, int issueId)
        {
            var issue = issueRepository.GetIssue(issueId, this.userSessionHelper.UserId);
            if (issue != null)
            {
                var activity = new ActivityDto() { ObjectId = commentId, ObjectType = "Comment" };
                activity.Description = "Commented";
                //activity.NewState = newVersion.Status.Name;
                // activity.OldState = previousVersion.Status.Name;
                activity.ObjectTitle = issue.Title;
                activity.TeamId = userSessionHelper.TeamId;

                activity.Actor = new UserDto { Id = userSessionHelper.UserId };

                var newId = activityRepository.Save(activity);
                return activityRepository.GetActivityItem(newId);

            }
            return null;
        }



        public Task SendEmailNotificaionForNewComment(CommentVM comment)
        {
            throw new NotImplementedException();
        }
    }
 }