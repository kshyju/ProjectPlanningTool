using System.Collections.Generic;

using SmartPlan.DataAccess;


using System.Linq;
using System;
using System.Data.Entity;
namespace Planner.DataAccess
{

    
    public class Repositary : IRepositary
    {
        TeamEntities db;
        public Repositary()
        {
           db = new TeamEntities();
        }
        public bool DeleteProject(int projectId, int teamId)
        {
            throw new System.NotImplementedException();
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
        public IEnumerable<Project> GetProjects(int teamId)
        {
            return db.Projects.Where(s => s.TeamID == teamId);
        }

        public Project GetProject(int projectId, int teamId)
        {
            return db.Projects.FirstOrDefault(s => s.ID == projectId && s.TeamID == teamId);
        }

        public Project GetProject(string name, int teamId)
        {
            return db.Projects.FirstOrDefault(s => s.Name == name && s.TeamID == teamId);
        }
        /*
        public List<Entities.Priority> GetPriorities()
        {
            throw new System.NotImplementedException();
        }

        public List<Entities.Status> GetStatuses()
        {
            throw new System.NotImplementedException();
        }

        public List<Entities.Category> GetCategories()
        {
            throw new System.NotImplementedException();
        }

        public OperationStatus SaveIssue(Entities.Issue bug)
        {
            throw new System.NotImplementedException();
        }

      /*  public OperationStatus SaveDocument(Document image)
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

        public OperationStatus SaveUser(TechiesWeb.TeamBins.Entities.User user)
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(string emailAddress)
        {
            return db.Users.FirstOrDefault(a => a.EmailAddress == emailAddress);

        }
        */
        public IEnumerable<Team> GetTeams(int userId)
        {
           //throw new System.NotImplementedException();
           return db.TeamMembers.Where(s => s.UserID == userId).Select(s => s.Team);
        }
       
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
       
        public TeamMemberRequest AddTeamMemberRequest(TeamMemberRequest request)
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
        }
        /*
        public IEnumerable<Entities.Issue> GetIssues(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public Entities.Issue GetIssue(int issueId)
        {
            throw new System.NotImplementedException();
        }

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

        public IEnumerable<Comment> GetCommentsForIssue(int issueId)
        {
            throw new System.NotImplementedException();
        }

        public OperationStatus SaveComment(Comment comment)
        {
            throw new System.NotImplementedException();
        }

        public Comment GetComment(int commentId)
        {
            throw new System.NotImplementedException();
        }

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
    }
}
