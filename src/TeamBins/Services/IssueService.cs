using System.Collections.Generic;
using System.Linq;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.ViewModels;
using System;

using TechiesWeb.TeamBins.ExtensionMethods;
using SmartPlan.ViewModels;
using TechiesWeb.TeamBins.Infrastructure;
using TechiesWeb.TeamBibs.Helpers.Logging;
using System.Text.RegularExpressions;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

namespace TeamBins.Services
{

    public class IssueService : IDisposable
    {
        IRepositary repo;
        public string SiteBaseURL { set; get; }
        private int _userId;
        private int _teamId;
        public IssueService(IRepositary repositary, int userId, int teamId)
        {
            repo = repositary;
          
            _userId = userId;
            _teamId = teamId;
        }
        public IssueService(IRepositary repositary, int userId, int teamId,string siteBaseUrl)
        {
            repo = repositary;

            _userId = userId;
            _teamId = teamId;
            SiteBaseURL = siteBaseUrl;
        }
        /*
        public List<ActivityVM> GetTeamActivityVMs(int teamId)
        {
            List<ActivityVM> activityVMList = new List<ActivityVM>();
            try
            {
                var activityList = repo.GetTeamActivity(teamId).OrderByDescending(s => s.CreatedDate).ToList();
                
                ActivityVM activityVM=new ActivityVM();
                foreach (var item in activityList)
                {
                    if (item.ObjectType.ToUpper() == "ISSUE")
                    {
                        activityVM = GetActivityVM(item);
                    }
                    else if (item.ObjectType.ToUpper() == "ISSUECOMMENT")
                    {
                        activityVM = new CommentService(repo,SiteBaseURL).GetActivityVM(item);
                    }

                    activityVMList.Add(activityVM);
                }
            }
            catch (Exception ex)
            {
                
            }
            return activityVMList;
        }
        */
        
        /*
        public ActivityVM GetActivityVM(Activity item)
        {
            var activityVM = new ActivityVM() { Author = item.User.FirstName, CreatedDateRelative = item.CreatedDate.ToString() };
            if (item.ActivityDesc.ToUpper() == "CREATED")
            {
                activityVM.Activity = item.ActivityDesc;
                activityVM.ObjectTite = item.NewState;
                activityVM.ObjectURL = String.Format("{0}Issues/details/{1}", SiteBaseURL, item.ObjectID);
            }
            return activityVM;
        }*/

        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            var vm = new DashBoardItemSummaryVM();
            var issues = repo.GetIssues(teamId);
            vm.CurrentItems = issues.Where(s => s.Location == LocationType.SPRNT.ToString()).Count();
            vm.CompletedItems = issues.Where(s => s.Location == LocationType.ARCHV.ToString()).Count();
            vm.BacklogItems = issues.Where(s => s.Location == LocationType.BKLOG.ToString()).Count();
            vm.ItemsInProgress = issues.Where(s => s.Status.Name.ToUpper() == "IN PROGRESS").Count();
            vm.NewItems = issues.Where(s => s.Status.Name.ToUpper() == "NEW").Count();
            return vm;
        }

        //public void SendEmailNotificaionForNewComment(Comment comment,Issue issue, int teamId, int currentUserId, string siteBaseUrl)
        //{
        //    try
        //    {
        //        var subscribers = repo.GetSubscribers(teamId, "NewComment");
        //        if (subscribers.Count() > 0)
        //        {
        //            var emailTemplate = repo.GetEmailTemplate("NewComment");
        //            if (emailTemplate != null)
        //            {
        //                var currentTeam = repo.GetTeam(teamId);
        //                string emailSubject = emailTemplate.EmailSubject;
        //                string emailBody = emailTemplate.EmailBody;
        //                Email email = new Email();

        //                string issueUrl = siteBaseUrl + "issues/" + issue.Id;
        //                var issueLink = "<a href='" + issueUrl + "'>" + "# " + issue.Id + " " + issue.Title + "</a>";
        //                emailBody = emailBody.Replace("@author", issue.CreatedBy.FirstName);
        //                emailBody = emailBody.Replace("@link", issueLink);
        //                emailBody = emailBody.Replace("@comment", comment.CommentText);
        //                emailBody = emailBody.Replace("@issueId", issue.Id.ToString());  
        //                email.Body = emailBody;
        //                emailSubject = emailSubject.Replace("@issueId", issue.Id.ToString());                      
        //                email.Subject = emailSubject;

        //                foreach (var subscriber in subscribers)
        //                {
        //                    email.ToAddress.Add(subscriber.EmailAddress);
        //                }
        //                email.Send();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var log = new Logger("SendEmailNotificaions");
        //        log.Error(ex);
        //    }
        //}
        //public void SendEmailNotificationsToSubscribers(Issue issue,int teamId, int currentUserId,string siteBaseUrl)
        //{
        //    SendEmailNotificaionForNewIssue(issue, teamId, currentUserId, siteBaseUrl);           
        //}

        //private void SendEmailNotificaionForNewIssue(Issue issue, int teamId, int currentUserId,string siteBaseUrl)
        //{
        //    try
        //    {
        //        var subscribers = repo.GetSubscribers(teamId, "NewIssue");
        //        if (subscribers.Count() > 0)
        //        {
        //            var emailTemplate = repo.GetEmailTemplate("NewIssue");
        //            if (emailTemplate != null)
        //            {
        //                var currentTeam = repo.GetTeam(teamId);
        //                string emailSubject = emailTemplate.EmailSubject;
        //                string emailBody = emailTemplate.EmailBody;
        //                Email email = new Email();

        //                string issueUrl = siteBaseUrl + "issues/" + issue.Id;
        //                var issueLink = "<a href='" + issueUrl + "'>" + "#" + issue.Id + " " + issue.Title + "</a>";
        //                emailBody = emailBody.Replace("@issueAuthor", issue.CreatedBy.FirstName);
        //                emailBody = emailBody.Replace("@issueLink", issueLink);
        //                emailBody = emailBody.Replace("@teamName", currentTeam.Name);
        //                email.Body = emailBody;
        //                emailSubject = emailSubject.Replace("@teamName", currentTeam.Name);
        //                emailSubject = emailSubject + " (#" + issue.Id + ")";
        //                email.Subject = emailSubject;

        //                foreach (var subscriber in subscribers)
        //                {
        //                    email.ToAddress.Add(subscriber.EmailAddress);
        //                }
        //                email.Send();
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        var log = new Logger("SendEmailNotificaions");
        //        log.Error(ex);
        //    }
        //}
      

        public List<IssueVM> GetIssueListVMs(string iteration, int teamId, int size)
        {
            string iterationName = LocationType.SPRNT.ToString();
            if(iteration=="backlog")
            {
                iterationName = LocationType.BKLOG.ToString();
            }
            else if(iteration=="completed")
            {
                iterationName = LocationType.ARCHV.ToString();
            }

            var bugList = repo.GetIssues(teamId).Where(g => g.Location == iterationName).OrderByDescending(s => s.ID).Take(size).ToList();
            List<IssueVM> issueList = new List<IssueVM>();
            foreach (var bug in bugList)
            {
                var issueVM = GetIssueVM(bug);
                issueList.Add(issueVM);
            }
            return issueList;
        }

        public IssueVM GetIssueVM(Issue bug)
        {
            var issueVM = new IssueDetailVM { Id = bug.ID, Title = bug.Title, Description = bug.Description };
            issueVM.OpenedBy = bug.CreatedBy.FirstName;
            issueVM.PriorityName = bug.Priority.Name;
            issueVM.StatusName = bug.Status.Name;
            issueVM.CategoryName = bug.Category.Name;
            issueVM.ProjectName = (bug.Project!=null?bug.Project.Name:"");
            issueVM.CreatedDate = bug.CreatedDate;
            return issueVM;
        }


        

        public void SetUserPermissionsForIssue(IssueDetailVM issueVm, int currentUserId = 0, int teamId = 0)
        {
            if(currentUserId>0)
            {
                
                var teamMember = repo.GetTeamMember(currentUserId, teamId);
                if(teamMember!=null)
                {
                    issueVm.IsEditableForCurrentUser = true;
                }
            }
        }

        public void LoadIssueMembers(int id, IssueDetailVM issueVm, int currentUserId = 0)
        {

            var issueMemberRelations = repo.GetIssueMembers(id);

            //check the current user has starred the issue
            if (currentUserId != 0)
            {
                var isCurrentUserStarredIssue = issueMemberRelations.Where(s => s.RelationType == IssueMemberRelationType.Star.ToString() && s.MemberID == currentUserId).FirstOrDefault();
                issueVm.IsStarredForUser = isCurrentUserStarredIssue != null;
            }
            /**
            //Now get the members assigned to this issue
            var issueMemberList = issueMemberRelations.Where(s => s.RelationType ==IssueMemberRelationType.Member.ToString());
            foreach (var member in issueMemberList)
            {
                var vm = new MemberVM { MemberType = member.Member.JobTitle, Name = member.Member.FirstName, MemberID = member.MemberID };
                vm.AvatarHash = UserService.GetImageSource(member.Member.EmailAddress);
                issueVm.Members.Add(vm);
            }*/
        }
        public bool SaveIssueMember(int issueId, int userId, int createdById)
        {
            return SaveIssueMemberRelation(issueId, userId, IssueMemberRelationType.Member,createdById);
        }

        public bool StarIssue(int issueId, int userId)
        {
            return SaveIssueMemberRelation(issueId, userId, IssueMemberRelationType.Star,userId);
        }
        public bool UnStarIssue(int issueId, int userId)
        {
            return DeleteIssueMemberRelation(issueId, userId, IssueMemberRelationType.Star);
        }
        private bool DeleteIssueMemberRelation(int issueId, int userId, IssueMemberRelationType relationType)
        {
            try
            {
                var issueMember = new IssueMember { MemberID = userId, IssueID = issueId,  RelationType = relationType.ToString() };
                var result = repo.DeleteIssueMemberRelation(issueMember);
            }
            catch (Exception ex)
            {
                // to do : Log
                return false;
            }
            return true;
        }
        private bool SaveIssueMemberRelation(int issueId, int userId, IssueMemberRelationType relationType, int createdById)
        {
            try
            {
                var issueMember = new IssueMember { MemberID = userId, IssueID = issueId, CreatedByID=createdById, RelationType = relationType.ToString() };
                var result = repo.SaveIssueMemberRelation(issueMember);
            }
            catch(Exception ex)
            {
                // to do : Log
                return false;
            }
            return true;    
        }

        public Activity SaveActivityForDueDate(int issueId, int teamId,int currentUserId)
        {
            var issue = repo.GetIssue(issueId);

            Activity activity = new Activity();
            activity.CreatedByID = currentUserId;
            activity.OldState = issue.Title;
            activity.NewState = issue.DueDate.Value.ToShortDateString();
            activity.ObjectID = issueId;
            activity.ObjectType = "Issue";
            activity.ActivityDesc = "Due date updated";
            activity.TeamID = teamId;
            var result = repo.SaveActivity(activity);
            if (result.Status)
            {
                return repo.GetActivity(activity.ID);
            }
            return null;
        }
       
        public Activity SaveActivity(Comment comment,int teamId)
        {
            var issue = repo.GetIssue(comment.IssueID);
     
            Activity activity = new Activity();
            activity.CreatedByID = comment.CreatedByID;
            activity.OldState = issue.Title;
            activity.NewState = comment.CommentText;
            activity.ObjectID = comment.ID;
            activity.ObjectType = "IssueComment";
            activity.ActivityDesc = "Commented";
            activity.TeamID = teamId;
            var result = repo.SaveActivity(activity);
            if (result.Status)
            {
                return repo.GetActivity(activity.ID);
            }
            return null;
        }
        
        public void Dispose()
        {
            if (repo != null)
            {
                repo.Dispose();
            }
        }

        public ActivityVM GetActivityVM(Activity activity)
        {
            var activityVM = new ActivityVM() { Id=activity.ID, Author = activity.User.FirstName, CreatedDate = activity.CreatedDate.ToJSONFriendlyDateTime()};
            if (activity.ActivityDesc.ToUpper() == "CREATED")
            {
                activityVM.Activity = activity.ActivityDesc;
                activityVM.ObjectTite = activity.NewState;               
            }
            else if (activity.ActivityDesc.ToUpper() == "CHANGED STATUS")
            {
                activityVM.Activity = "changed status of";
                activityVM.ObjectTite = activity.OldState;
                activityVM.NewState = "to "+activity.NewState;               
            }
            else if (activity.ActivityDesc.ToUpper() == "DUE DATE UPDATED")
            {
                activityVM.Activity = "updated due date of";
                activityVM.ObjectTite = activity.OldState;
                activityVM.NewState = "to " + activity.NewState;               
            }
            activityVM.ObjectURL = String.Format("{0}Issues/{1}", SiteBaseURL, activity.ObjectID);
            return activityVM;
        }
               
        public List<CommentVM> GetIssueCommentVMs(int id,int currentUserId)
        {
            var commentVMList=new List<CommentVM>();
            var commentList = repo.GetCommentsForIssue(id);
            foreach (var item in commentList)
            {
                var commentVM = GetIssueCommentVM(currentUserId, item);
                commentVMList.Add(commentVM);
            }
            return commentVMList;
        }

        public CommentVM GetIssueCommentVM(int currentUserId, Comment item)
        {
            var commentVM = new CommentVM { ID = item.ID, CommentBody = item.CommentText, AuthorName = item.Author.FirstName,
                CreativeDate = item.CreatedDate };
            commentVM.CommentBody = commentVM.CommentBody.ConvertUrlsToLinks();
            commentVM.AvatarHash = UserService.GetAvatarUrl(item.Author.Avatar, 42);
            commentVM.CreatedDateRelative = item.CreatedDate.ToJSONFriendlyDateTime();//.ToRelativeDateTime();
            commentVM.IsOwner = item.CreatedByID == currentUserId;
            return commentVM;
        }
        
        public Activity SaveActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }
    }
}
