using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.DataAccess;
namespace TeamBins.DataAccess
{    
    public class Repositary : IRepositary
    {
        private TeamEntities db;
        public Repositary()
        {
           db = new TeamEntities();
        }
        public IEnumerable<User> GetSubscribers(int teamId,string notificationtypeCode)
        {
            return db.UserNotificationSubscriptions.Where(s => s.NotificationType.Code == notificationtypeCode && s.TeamID==teamId).Select(s => s.User);
        }
        public IEnumerable<NotificationType> GetNotificationTypes()
        {
            return db.NotificationTypes;
        }
        public void SaveUserNotificationSubscription(UserNotificationSubscription subscription)
        {
            var existing = db.UserNotificationSubscriptions.FirstOrDefault(s => s.TeamID == subscription.TeamID && s.UserID == s.UserID && s.NotificationTypeID == subscription.NotificationTypeID);
            if(existing==null)
            {
                subscription.ModifiedDate = DateTime.UtcNow;
                db.UserNotificationSubscriptions.Add(subscription);

            }
            else
            {
                existing.ModifiedDate = DateTime.UtcNow;
                existing.Subscribed = subscription.Subscribed;
            }         
            db.SaveChanges();
        }

        public void SaveLastLogin(int userId)
        {
            var user = db.Users.FirstOrDefault(s=>s.ID==userId);
            user.LastLoginDate = DateTime.UtcNow;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void  SavePasswordResetRequest(PasswordResetRequest request)
        {
            try
            {
                request.CreatedDate = DateTime.UtcNow;
                db.PasswordResetRequests.Add(request);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Activity GetActivity(int id)
        {
            return db.Activities.Include(s=>s.User).FirstOrDefault(s=>s.ID==id);
        }
        public PasswordResetRequest GetPasswordResetRequest(string activationCode)
        {
            return db.PasswordResetRequests.OrderByDescending(s=>s.CreatedDate).FirstOrDefault(s => s.ActivationCode == activationCode);
        }
        public User GetUser(int userId)
        {
            return db.Users.FirstOrDefault(s => s.ID == userId);
        }
        public TeamMember GetTeamMember(int userId, int teamId)
        {
            return db.TeamMembers.Where(s => s.TeamID == teamId && s.MemberID == userId).FirstOrDefault();
        }
        public OperationStatus DeleteIssue(int issueId)
        {
            try
            {
                var issue=db.Issues.FirstOrDefault(s=>s.ID==issueId);
                if(issue!=null)
                {
                    issue.Active = false;
                    db.Entry(issue).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new OperationStatus { Status = true };
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Error deleting issue " + issueId, ex);
            }
        }
        public OperationStatus DeleteIssueMemberRelation(IssueMember issueMember)
        {
            try
            {
                var issueMemberRelation = db.IssueMembers.Where(s => s.IssueID == issueMember.IssueID && s.MemberID == issueMember.MemberID && s.RelationType == issueMember.RelationType).FirstOrDefault();
                if (issueMemberRelation != null)
                {
                    db.IssueMembers.Remove(issueMemberRelation);
                    db.SaveChanges();
                    return new OperationStatus { Status = true };
                }
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("error deleting issue member relation "+issueMember.ID , ex);
            }
            return new OperationStatus();
        }
        public OperationStatus SaveIssueMemberRelation(IssueMember issueMember)
        {
            try
            {
                issueMember.CreatedDate = DateTime.Now;
                db.IssueMembers.Add(issueMember);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return OperationStatus.CreateFromException("error saving issue member",ex);
            }
            return new OperationStatus();
        }
        public bool DeleteProject(int projectId)
        {
            
            
            var project = db.Projects.FirstOrDefault(s => s.ID == projectId);
            /*var projectMembers = project.ProjectMembers.ToList();

            foreach(var members in projectMembers)
            {
                db.ProjectMembers.Remove(members);
            }*/

            // IF the this project is the default project, let's delete that association

 

            db.SaveChanges();
            db.Projects.Remove(project);
            db.SaveChanges();
            return true;
        }
        public OperationStatus SaveTeamMemberRequest(TeamMemberRequest teamMemberRequest)
        {
            try
            {
                if (teamMemberRequest.ID == 0)
                {

                    teamMemberRequest.CreatedDate = DateTime.UtcNow;
                    db.TeamMemberRequests.Add(teamMemberRequest);
                }
                else
                {
                    db.Entry(teamMemberRequest).State = EntityState.Modified;
                }
                db.SaveChanges();
                return new OperationStatus { Status = true };
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Error in saving team member" , ex);
            }
        }            

        public Document GetDocument(string fileAlias)
        {
            return db.Documents.FirstOrDefault(s => s.FileAlias == fileAlias);            
        }
        
        public List<Document> GetDocuments(int parentId)
        {
            return db.Documents.Where(s => s.ParentID == parentId).ToList();
        }

        public IEnumerable<Project> GetProjects(int teamId)
        {
            return db.Projects.Where(s=>s.TeamID==teamId);
        }
        public Project GetProject(int projectId)
        {
            return db.Projects.FirstOrDefault(s => s.ID == projectId);
        }
        public Project GetProject(int projectId, int teamId)
        {
            return db.Projects.FirstOrDefault(s => s.ID == projectId && s.TeamID == teamId);
        }

        public Project GetProject(string name, int createdById)
        {
            return db.Projects.FirstOrDefault(s => s.Name == name && s.CreatedByID == createdById);
        }
       
        public List<Priority> GetPriorities()
        {
            return db.Priorities.ToList();
        }

        public List<Status> GetStatuses()
        {
            return db.Status.ToList();
        }

        public List<Category> GetCategories()
        {
            return db.Categories.ToList();
        }

        public OperationStatus SaveIssue(Issue issue)
        {
            if (issue.ID == 0)
            {
                issue.Active = true;
                issue.CreatedDate = DateTime.UtcNow;
                db.Issues.Add(issue);
            }
            else
            {                
                issue.ModifiedDate = DateTime.UtcNow;
                db.Entry(issue).State = EntityState.Modified;
            }
            db.SaveChanges();
            return new OperationStatus { Status = true, OperationID = issue.ID };
        }
       
       public OperationStatus SaveDocument(Document image)
        {
            image.CreatedDate = DateTime.UtcNow;
            db.Documents.Add(image);
            db.SaveChanges();
            return new OperationStatus { Status = true };
        }
    
        public Project SaveProject(Project project)
        {
            if (project.ID == 0)
            {
                project.CreatedDate = DateTime.UtcNow;
                db.Projects.Add(project);
            }
            else
            {
                db.Entry(project).State = EntityState.Modified;
            }
            db.SaveChanges();
            return project;
        }

        public OperationStatus SaveUser(User user)
        {
            try
            {
                if (user.ID == 0)
                {
                    user.CreatedDate = DateTime.UtcNow;
                    db.Users.Add(user);
                }
                else
                {
                    db.Entry(user).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                return  OperationStatus.CreateFromException("error", dbEx);
            }
            catch (Exception ex)
            {
                var res = new OperationStatus();

            }
            return new OperationStatus { OperationID = user.ID, Status = true };
        }
        /*
        public User GetUser(string emailAddress)
        {
            return db.Users.FirstOrDefault(a => a.EmailAddress == emailAddress);

        }
        */
        /*
        public IEnumerable<Team> GetTeams(int userId)
        {
           //throw new System.NotImplementedException();
           return db.TeamMembers.Where(s => s.UserID == userId).Select(s => s.Team);
        }
       */
        public Team GetTeam(int teamId)
        {
            return db.Teams.FirstOrDefault(s => s.ID == teamId);
        }

        public Team SaveTeam(Team team)
        {
            if (team.ID == 0)
            {
                team.CreatedDate = DateTime.UtcNow;
                db.Teams.Add(team);
            }
            else
            {
                db.Entry(team).State = EntityState.Modified;
            }
            db.SaveChanges();
            return team;
        }
       
       /* public TeamMemberRequest AddTeamMemberRequest(TeamMemberRequest request)
        {
            request.CreatedDate = DateTime.UtcNow;
            db.TeamMemberRequests.Add(request);
            db.SaveChanges();
            return request;
        }*/
        public TeamMember SaveTeamMember(TeamMember teamMember)
        {
            if (teamMember.ID == 0)
            {
                teamMember.CreatedDate = DateTime.UtcNow;
                db.TeamMembers.Add(teamMember);
            }
            else
            {
                db.Entry(teamMember).State = EntityState.Modified;
            }
            db.SaveChanges();
            return teamMember;
        }
      
        public IEnumerable<Issue> GetIssues()
        {
            return db.Issues;
        }
        public IEnumerable<Issue> GetIssues(int teamId)
        {
            return db.Issues.Where(s => s.TeamID == teamId && s.Active==true);
        }
        public Issue GetIssue(int issueId)
        {
            return db.Issues.Include(s => s.Priority).Include(x => x.Status).Include(s => s.Category).Include(s => s.CreatedBy).Include(s=>s.ModifiedBy).FirstOrDefault(s => s.ID == issueId && s.Active == true);
        }
  /*
        public List<TechiesWeb.TeamBins.Entities.User> GetTeamMembers(int teamId)
        {
            throw new System.NotImplementedException();
        }
        */ 
        public List<IssueMember> GetIssueMembers(int issueId)
        {
            return db.IssueMembers.Where(s => s.IssueID == issueId ).ToList();
        }
       /*
        public List<TechiesWeb.TeamBins.Entities.User> GetNonIssueMembers(int teamId, int issueId)
        {
            throw new System.NotImplementedException();
        }
       */
        public OperationStatus SaveIssueMember(int issueId, int memberId, int addedBy)
        {
            try
            {
                var issueMember = new IssueMember { IssueID = issueId, MemberID = memberId, CreatedByID = addedBy, CreatedDate = DateTime.UtcNow };
                db.IssueMembers.Add(issueMember);
                db.SaveChanges();
                return new OperationStatus { Status = true };
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Errorin saving issuemember", ex);
            }
           
        } 
        
        public OperationStatus DeleteIssueMember(int issueId, int memberId)
        {
            try
            {
                var issueMember = db.IssueMembers.Where(s => s.IssueID == issueId && s.MemberID == memberId).FirstOrDefault();
                if (issueMember != null)
                {
                    db.IssueMembers.Remove(issueMember);
                    db.SaveChanges();
                    return new OperationStatus { Status = true };
                }
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Error in deleting issue member. Issue:"+issueId+", member:"+memberId, ex);
            }
            return new OperationStatus { Status = false, Message="No record found!" };
        }
        
        public IEnumerable<Comment> GetCommentsForIssue(int issueId)
        {
            return db.Comments.Where(s => s.IssueID == issueId);
        }

        public OperationStatus SaveComment(Comment comment)
        {
            try
            {
                comment.CreatedDate = DateTime.UtcNow;
                db.Comments.Add(comment);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return OperationStatus.CreateFromException("Error in comment", ex);
            }
            

            return new OperationStatus { Status = true, OperationID = comment.ID };

        }

        public Comment GetComment(int commentId)
        {
            return db.Comments.Where(s => s.ID == commentId).Include(s=>s.Issue).FirstOrDefault();
        }

        public IEnumerable<Activity> GetTeamActivity(int teamId)
        {
            return db.Activities.Where(s => s.TeamID == teamId);
        }

        public OperationStatus SaveActivity(Activity activity)
        {
            try
            {
                activity.CreatedDate = DateTime.UtcNow;
                db.Activities.Add(activity);
                db.SaveChanges();
                return new OperationStatus { Status = true };
            }
            catch(Exception ex)
            {
                return OperationStatus.CreateFromException("error in saving", ex);
            }
        }
        
        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        public User GetUser(string emailAddress)
        {
            return db.Users.FirstOrDefault(s => s.EmailAddress == emailAddress);
        }
        public TeamMemberRequest GetTeamMemberRequest(string activationCode)
        {
            return db.TeamMemberRequests.Include(s=>s.CreatedBy).Include(s=>s.Team).FirstOrDefault(s => s.ActivationCode == activationCode);
        }

        public IEnumerable<Team> GetTeams(int userId)
        {
            return db.Teams.Where(s => s.TeamMembers.Any(f => f.MemberID == userId));
        }
        public EmailTemplate GetEmailTemplate(string templateName)
        {
            return db.EmailTemplates.FirstOrDefault(s => s.Name == templateName);
        }
    }
}
