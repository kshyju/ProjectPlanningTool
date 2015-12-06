using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

namespace TeamBins.Services
{
   

    public interface IIssueManager
    {
        Task<int> StarIssue(int issueId);
        IEnumerable<IssueVM> GetIssues(List<int> statusIds , int count);
        IssueDetailVM SaveIssue(CreateIssue issue, List<HttpPostedFileBase> files);
        DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId);
        IssueDetailVM GetIssue(int id);
        ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion);

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count);
    }
}