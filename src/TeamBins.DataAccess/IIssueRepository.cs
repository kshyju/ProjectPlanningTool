using System;
using System.Collections.Generic;
using System.Linq;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{

    public interface IIssueRepository
    {
        IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count);
        IssueDetailVM GetIssue(int id);
        int SaveIssue(CreateIssue issue);
        DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId);
    }


    public class IssueRepository : IIssueRepository
    {

        public int SaveIssue(CreateIssue issue)
        {
            using (var db = new TeamEntities())
            {
                var issueEntity = new Issue { Title = issue.Title, Description = issue.Description };
                issueEntity.ProjectID = issue.ProjectID;
                issueEntity.TeamID = issue.TeamID;
                issueEntity.CategoryID = issue.SelectedCategory;
                issueEntity.CreatedDate = DateTime.UtcNow;
                issueEntity.Active = true;
                issueEntity.CreatedByID = issue.CreatedByID;
                issueEntity.Location = issue.Iteration;
                issueEntity.StatusID = issue.SelectedStatus;

                if (issueEntity.CategoryID == 0)
                {
                    issueEntity.CategoryID = db.Categories.FirstOrDefault().ID;
                }
                if (issueEntity.StatusID == 0)
                {
                    //TO DO  : User CODE
                    var status = db.Status.FirstOrDefault(s => s.Name == "New");
                    issueEntity.StatusID = status.ID;
                }


                db.Issues.Add(issueEntity);
                db.SaveChanges();
                return issueEntity.ID;
            }
        }
        public IssueDetailVM GetIssue(int id)
        {
            using (var db = new TeamEntities())
            {
                var issue = db.Issues.FirstOrDefault(s => s.ID == id);
                if (issue != null)
                {
                    var issueDto = new IssueDetailVM
                    {
                        ID = issue.ID,
                        Title = issue.Title,
                        Description = issue.Description,
                        // TeamID = issue.TeamId,
                        Status = new KeyValueItem { Id = issue.Category.ID, Name = issue.Status.Name },
                        Category = new KeyValueItem { Id = issue.Category.ID, Name = issue.Category.Name }
                    };
                    return issueDto;
                }
            }
            return null;
        }

        //var issueVM = new IssueVM { ID = bug.ID, Title = bug.Title, Description = bug.Description };
        //issueVM.OpenedBy = bug.CreatedBy.FirstName;
        //    issueVM.Priority = bug.Priority.Name;
        //    issueVM.StatusName = bug.StatusName.Name;
        //    issueVM.CategoryName = bug.CategoryName.Name;
        //    issueVM.Project = (bug.Project!=null?bug.Project.Name:"");
        //    issueVM.CreatedDate = bug.CreatedDate.ToShortDateString();


        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            var vm = new DashBoardItemSummaryVM();
            using (var db = new TeamEntities())
            {
                var statusCounts = db.Issues
                    .Where(s=>s.TeamID==teamId)
                    .GroupBy(d => d.Status, g => g.ID, (k, i) => new
                ItemCount
                {
                    ItemId = k.ID,
                    ItemName = k.Name,
                    Count = i.Count()
                }).ToList();

                vm.IssueCountsByStatus = statusCounts;
            }
            
            return vm;
        }

        public IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count)
        {
            using (var db = new TeamEntities())
            {
                
                return db.Issues.Where(s => statusIds.Contains(s.StatusID)).OrderByDescending(s => s.CreatedDate)
                    .Take(count)
                    .Select(s => new IssueVM
                    {
                        ID = s.ID,
                        Title = s.Title,
                        Description = s.Description,
                        Priority = s.Priority.Name,
                        StatusName = s.Status.Name,
                        CategoryName = s.Category.Name,
                        Project = s.Project.Name,
                        CreatedDate = s.CreatedDate
                    }).ToList();
            }
        }
    }
}