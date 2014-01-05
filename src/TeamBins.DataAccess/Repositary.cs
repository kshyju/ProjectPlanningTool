using System.Collections.Generic;

using SmartPlan.DataAccess;


using System.Linq;
using System;
using System.Data.Entity;
using System.Data.Objects;
namespace Planner.DataAccess
{

    
    public class Repositary : IRepositary
    {
        TeamEntities db;
        public Repositary()
        {
           db = new TeamEntities();
        }
        public User GetUser(int userId)
        {
            return db.Users.FirstOrDefault(s => s.ID == userId);
        }
        public bool DeleteProject(int projectId)
        {
            

            var project = db.Projects.FirstOrDefault(s => s.ID == projectId);

            var projectMembers = project.ProjectMembers.ToList();

            foreach(var members in projectMembers)
            {
                db.ProjectMembers.Remove(members);
            }
            db.SaveChanges();
            db.Projects.Remove(project);
            db.SaveChanges();
            return true;
        }
        public OperationStatus SaveProjectMember(ProjectMember projectMember)
        {
            try
            {
                db.ProjectMembers.Add(projectMember);
                db.SaveChanges();
                return new OperationStatus { Status = true, OperationID = projectMember.ID };
            }
            catch(Exception ex)
            {
                return new OperationStatus { Status = true, OperationID = projectMember.ID };
            }

          
        }
        /*

      /*  public Document GetDocument(string documentKey)
        {
            throw new System.NotImplementedException();
        }

        public List<Document> GetDocuments(int parentId, string type)
        {
            throw new System.NotImplementedException();
        }*/
/*
        public List<Bug> GetBugs(int page, int size)
        {
            throw new System.NotImplementedException();
        }
        */
        public IEnumerable<Project> GetProjects()
        {
            return db.Projects;
        }
        public Project GetProject(int projectId)
        {
            return db.Projects.FirstOrDefault(s => s.ID == projectId);
        }
        public Project GetProject(int projectId, int createdById)
        {
            return db.Projects.FirstOrDefault(s => s.ID == projectId && s.CreatedByID==createdById);
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
                issue.CreatedDate = DateTime.Now;
                db.Issues.Add(issue);
            }
            else
            {
                db.Entry(issue).State = EntityState.Modified;
            }
            db.SaveChanges();
            return new OperationStatus { Status = true, OperationID = issue.ID };
        }
        /*
       public OperationStatus SaveDocument(Document image)
        {
           throw new System.NotImplementedException();
        }
    */
        public Project SaveProject(Project project)
        {
            project.CreatedDate = DateTime.Now;
            db.Projects.Add(project);
            db.SaveChanges();
            return project;
        }
/*
        public Bug GetBug(int id)
        {
            throw new System.NotImplementedException();
        }
        */
        public OperationStatus SaveUser(User user)
        {
            try
            {
                if (user.ID == 0)
                {
                    user.CreatedDate = DateTime.Now;
                    db.Users.Add(user);
                }
                else
                {                  
                    db.Entry(user).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            catch(Exception ex)
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
                team.CreatedDate = DateTime.Now;
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
            request.CreatedDate = DateTime.Now;
            db.TeamMemberRequests.Add(request);
            db.SaveChanges();
            return request;
        }
        public TeamMember SaveTeamMember(TeamMember teamMember)
        {
            teamMember.CreatedDate = DateTime.Now;
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();
            return teamMember;
        }*/
      
        public IEnumerable<Issue> GetIssues()
        {
            return db.Issues;
        }

        public Issue GetIssue(int issueId)
        {
            return db.Issues.FirstOrDefault(s => s.ID == issueId);
        }
  /*
        public List<TechiesWeb.TeamBins.Entities.User> GetTeamMembers(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public List<TechiesWeb.TeamBins.Entities.User> GetIssueMembers(int issueId)
        {
            throw new System.NotImplementedException();
        }

        public List<TechiesWeb.TeamBins.Entities.User> GetNonIssueMembers(int teamId, int issueId)
        {
            throw new System.NotImplementedException();
        }

        public OperationStatus SaveIssueMember(int issueId, int memberId, int addedBy)
        {
            throw new System.NotImplementedException();
        }

        public OperationStatus DeleteIssueMember(int issueId, int memberId)
        {
            throw new System.NotImplementedException();
        }
        */
        public IEnumerable<Comment> GetCommentsForIssue(int issueId)
        {
            return db.Comments.Where(s => s.IssueID == issueId);
        }

        public OperationStatus SaveComment(Comment comment)
        {
            try
            {
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
            return db.Comments.Where(s => s.ID == commentId).FirstOrDefault();
        }
/*
        public IEnumerable<Activity> GetTeamActivity(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public OperationStatus SaveActivity(Activity comment)
        {
            throw new System.NotImplementedException();
        }
        */
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(string emailAddress)
        {
            return db.Users.FirstOrDefault(s => s.EmailAddress == emailAddress);
        }


        public IEnumerable<Team> GetTeams(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
