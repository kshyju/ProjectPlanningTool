using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.Controllers;
using TechiesWeb.TeamBins.ViewModels;
using System.Linq;

namespace TeamBins.Tests
{
    public class FakeRepositary : IRepositary
    {
        private FakeDataStore _fakeDataBase;
        public FakeRepositary(FakeDataStore fakeDataBase)
        {
            _fakeDataBase = fakeDataBase;
        }
        public bool DeleteProject(int projectId)
        {
            throw new System.NotImplementedException();
        }

        public Document GetDocument(string documentKey)
        {
            throw new System.NotImplementedException();
        }

        public List<Document> GetDocuments(int parentId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Project> GetProjects(int teamId)
        {
            return _fakeDataBase.Projects.Where(s=>s.TeamID==teamId);
        }

        public Project GetProject(int projectId)
        {
            throw new System.NotImplementedException();
        }

        public Project GetProject(int projectId, int createdById)
        {
            return _fakeDataBase.Projects.Where(s => s.ID == projectId).FirstOrDefault();
        }

        public Project GetProject(string name, int createdById)
        {
            throw new System.NotImplementedException();
        }

        public List<Priority> GetPriorities()
        {
            throw new System.NotImplementedException();
        }

        public List<Status> GetStatuses()
        {
            throw new System.NotImplementedException();
        }

        public List<Category> GetCategories()
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveIssue(Issue bug)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveDocument(Document image)
        {
            throw new System.NotImplementedException();
        }

        public Project SaveProject(Project project)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveProjectMember(ProjectMember projectMember)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveUser(User user)
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(string emailAddress)
        {
            throw new System.NotImplementedException();
        }

        public User GetUser(int userId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Team> GetTeams(int userId)
        {
            throw new System.NotImplementedException();
        }

        public Team GetTeam(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public Team SaveTeam(Team team)
        {
            throw new System.NotImplementedException();
        }

        public TeamMember SaveTeamMember(TeamMember teamMember)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Issue> GetIssues()
        {
            return _fakeDataBase.Issues;
        }
        public IEnumerable<Issue> GetIssues(int teamId)
        {
            return _fakeDataBase.Issues.Where(s => s.TeamID == teamId);
        }
        public Issue GetIssue(int issueId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Comment> GetCommentsForIssue(int issueId)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveComment(Comment comment)
        {
            throw new System.NotImplementedException();
        }

        public Comment GetComment(int commentId)
        {
            throw new System.NotImplementedException();
        }

        public List<IssueMember> GetIssueMembers(int issueId)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveIssueMember(int issueId, int memberId, int addedBy)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus DeleteIssueMember(int issueId, int memberId)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveTeamMemberRequest(TeamMemberRequest teamMemberRequest)
        {
            throw new System.NotImplementedException();
        }

        public TeamMemberRequest GetTeamMemberRequest(string activationCode)
        {
            throw new System.NotImplementedException();
        }

        public TeamMember GetTeamMember(int userId, int teamId)
        {
            throw new System.NotImplementedException();
        }

        public EmailTemplate GetEmailTemplate(string templateName)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<Activity> GetTeamActivity(int teamId)
        {
            throw new System.NotImplementedException();
        }

        public SmartPlan.DataAccess.OperationStatus SaveActivity(Activity comment)
        {
            throw new System.NotImplementedException();
        }
    }
}
