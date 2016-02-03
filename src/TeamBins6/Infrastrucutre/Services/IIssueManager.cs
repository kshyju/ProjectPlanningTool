using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;
using TeamBins6.Infrastrucutre.Services;

namespace TeamBins.Services
{

    public class IssueManager : IIssueManager
    {
        private IIssueRepository issueRepository;
        private IProjectRepository iProjectRepository;
        private IUserSessionHelper urlSessionHelper;
        public IssueManager(IIssueRepository issueRepository, IProjectRepository iProjectRepository,IUserSessionHelper userSessionHelper)
        {
            this.issueRepository = issueRepository;
            this.iProjectRepository = iProjectRepository;
            this.urlSessionHelper = urlSessionHelper;
        }
        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            throw new NotImplementedException();
        }

        public IssueDetailVM GetIssue(int id)
        {
            return this.issueRepository.GetIssue(id);
        }

        public IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count)
        {
            throw new NotImplementedException();
        }

        public ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion)
        {
            throw new NotImplementedException();
        }

        public IssueDetailVM SaveIssue(CreateIssue issue, List<IFormFile> files)
        {
            if (issue.SelectedStatus==0)
            {
               var defaultProjectId = this.iProjectRepository.GetDefaultProjectForTeamMember(this.urlSessionHelper.TeamId,
                    this.urlSessionHelper.UserId);
                issue.SelectedStatus = defaultProjectId;

            }
            if (issue.SelectedCategory == 0)
            {
                var categories = this.issueRepository.GetCategories();
                issue.SelectedStatus = categories.First().Id;
            }
            if (issue.SelectedPriority == 0)
            {
                var categories = this.issueRepository.GetCategories();
                issue.SelectedPriority = categories.First().Id;
            }

            var issueId = this.issueRepository.SaveIssue(issue);

          

            var issueDetail = this.issueRepository.GetIssue(issueId);
            return issueDetail;
            ;
        }

        public Task<int> StarIssue(int issueId)
        {
            throw new NotImplementedException();
        }
    }
    public interface IIssueManager
    {
        Task<int> StarIssue(int issueId);
        IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count);
        IssueDetailVM SaveIssue(CreateIssue issue, List<IFormFile> files);
        DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId);
        IssueDetailVM GetIssue(int id);
        ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion);

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count);
    }
}