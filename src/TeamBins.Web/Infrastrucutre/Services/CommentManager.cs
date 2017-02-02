using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBins.Common;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccessCore;
using TeamBins.Infrastrucutre.Extensions;
using TeamBins.Infrastrucutre.Services;
using TeamBins.DataAccess;
using System.Linq;
using Microsoft.Extensions.Options;
using TeamBins.Infrastrucutre;

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
        readonly TeamBinsAppSettings _settings;
        private ITeamRepository _teamRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IIssueRepository _issueRepository;
        private readonly IUserAuthHelper _userSessionHelper;
        private readonly IEmailManager _emailManager;
        public CommentManager(ICommentRepository commentRepository,IUserAuthHelper userSessionHelper,
            IIssueRepository issueRepository, IActivityRepository activityRepository,IEmailManager emailManager,ITeamRepository teamRepository,IEmailRepository emailRepository, IOptions<TeamBinsAppSettings> settings)
        {
            this._settings = settings.Value;
            this._emailRepository = emailRepository;
            this._teamRepository = teamRepository;
            this._emailManager = emailManager;
            this._commentRepository = commentRepository;
            this._userSessionHelper = userSessionHelper;
            this._issueRepository = issueRepository;
            this._activityRepository = activityRepository;
        }
        public void Delete(int id)
        {
            _commentRepository.Delete(id);
        }

        public CommentVM GetComment(int id)
        {
            CommentVM commentVm = this._commentRepository.GetComment(id);
            if (commentVm != null)
            {
                commentVm.Author.GravatarUrl = commentVm.Author.EmailAddress.ToGravatarUrl();
                commentVm.IsOwner = commentVm.Author.Id == this._userSessionHelper.UserId;
            }
            return commentVm;
        }

        public IEnumerable<CommentVM> GetComments(int issueId)
        {
            var c= this._commentRepository.GetComments(issueId);
            foreach (var commentVm in c)
            {
                commentVm.Author.GravatarUrl = commentVm.Author.EmailAddress.ToGravatarUrl();
                commentVm.IsOwner = commentVm.Author.Id == this._userSessionHelper.UserId;
            }
            return c;
        }

        
        public int SaveComment(CommentVM comment)
        {
            comment.Author = new UserDto {Id = this._userSessionHelper.UserId};
            int commentId= this._commentRepository.Save(comment);

          

            return commentId;
        }

        public ActivityDto SaveActivity(int commentId, int issueId)
        {
            var issue = _issueRepository.GetIssue(issueId, this._userSessionHelper.UserId);
            if (issue != null)
            {
                var activity = new ActivityDto() { ObjectId = commentId, ObjectType = "Comment" };
                activity.Description = "Commented";
                //activity.NewState = newVersion.Status.Name;
                // activity.OldState = previousVersion.Status.Name;
                activity.ObjectTitle = issue.Title;
                activity.TeamId = _userSessionHelper.TeamId;

                activity.Actor = new UserDto { Id = _userSessionHelper.UserId };

                var newId = _activityRepository.Save(activity);
                return _activityRepository.GetActivityItem(newId);

            }
            return null;
        }



        /// <summary>
        /// Send email to subscribers of the team about the new comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task SendEmailNotificaionForNewComment(CommentVM comment)
        {
            try
            {
                var subscibers = await _teamRepository.GetSubscribers(_userSessionHelper.TeamId, "NewComment");
                if (subscibers.Any())
                {
                    var emailTemplate = await _emailRepository.GetEmailTemplate("NewComment");
                    if (emailTemplate != null)
                    {
                        var emailSubject = emailTemplate.Subject;
                        var emailBody = emailTemplate.EmailBody;
                        var email = new Email();


                        foreach (var subsciber in subscibers)
                        {
                            email.ToAddress.Add(subsciber.EmailAddress);
                        }

                        var issueUrl = this._settings.SiteUrl + "/issues/" + comment.IssueId;
                        var issueLink = string.Format("<a href='{0}'>" + "#{1} {2}</a>", issueUrl, comment.IssueId, comment.Issue.Title);

                        emailBody = emailBody.Replace("@author", comment.Author.Name);
                        emailBody = emailBody.Replace("@issueId", comment.IssueId.ToString());
                        emailBody = emailBody.Replace("@link", issueLink);

                        emailBody = emailBody.Replace("@comment", comment.CommentText);
                        emailSubject = emailSubject.Replace("@issueId", comment.IssueId.ToString());

                        email.Body = emailBody;
                        email.Subject = emailSubject;
                        await this._emailManager.Send(email);
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail. We will log this. But we do not want to show an error to user because of this
            }
        }

       
    }
 }