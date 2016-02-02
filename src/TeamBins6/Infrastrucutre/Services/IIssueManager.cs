using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using TeamBins.Common.ViewModels;
using TeamBins.DataAccess;

namespace TeamBins.Services
{

    public class IssueManager : IIssueManager
    {
        private IIssueRepository issueRepository;
        public IssueManager(IIssueRepository issueRepository)
        {
            this.issueRepository = issueRepository;
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