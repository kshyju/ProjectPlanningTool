using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

namespace TeamBins.DataAccess
{

    public interface IIssueRepository
    {
        IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count);
        IssueDetailVM GetIssue(int id);
        int SaveIssue(CreateIssue issue);
        DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId);
    }

  

    public class IssueRepository : IIssueRepository
    {

        public int SaveIssue(CreateIssue issue)
        {
            Issue issueEntity = null;
            using (var db = new TeamEntitiesConn())
            {

                if (issue.ID > 0)
                {
                    issueEntity = db.Issues.FirstOrDefault(s => s.ID == issue.ID);
                    if (issueEntity == null)
                    {
                        return 0;
                    }
                }
                else
                {
                    issueEntity = new Issue();
                }
                issueEntity.Title = issue.Title;
                issueEntity.Description = issue.Description;
                issueEntity.ProjectID = issue.ProjectID;
                issueEntity.TeamID = issue.TeamID;
                issueEntity.CategoryID = issue.SelectedCategory;
                
                issueEntity.CreatedByID = issue.CreatedByID;
                issueEntity.Location = issue.Iteration;
                issueEntity.StatusID = issue.SelectedStatus;
                issueEntity.PriorityID = issue.SelectedPriority;

                if (issueEntity.CategoryID == 0)
                {
                    issueEntity.CategoryID = db.Categories.FirstOrDefault().ID;
                }
                if (issueEntity.StatusID == 0)
                {

                    var status = db.Status.FirstOrDefault(s => s.Code == "New");
                    issueEntity.StatusID = status.ID;
                }
                if (issueEntity.PriorityID==null || issueEntity.PriorityID.Value==0)
                {
                    var priority = db.Priorities.FirstOrDefault(s => s.Code == "Normal");
                    issueEntity.PriorityID = priority.ID;
                }

                if (issue.ID == 0)
                {
                    issueEntity.CreatedDate = DateTime.UtcNow;
                    issueEntity.Active = true;
                    db.Issues.Add(issueEntity);
                }
                else
                {
                    issueEntity.ModifiedDate = DateTime.Now;
                    issueEntity.ModifiedByID = issue.CreatedByID;

                    db.Entry(issueEntity).State = EntityState.Modified;
                    
                }
               
                db.SaveChanges();
                return issueEntity.ID;
            }
        }
        public IssueDetailVM GetIssue(int id)
        {
            using (var db = new TeamEntitiesConn())
            {
                var issue = db.Issues.FirstOrDefault(s => s.ID == id);
                if (issue != null)
                {
                    var issueDto = new IssueDetailVM
                    {
                        ID = issue.ID,
                        Title = issue.Title,
                        Description = issue.Description ?? string.Empty,
                        Author = new UserDto { Id = issue.CreatedBy.ID, Name = issue.CreatedBy.FirstName },
                        Project = new KeyValueItem { Id = issue.Project.ID, Name = issue.Project.Name },
                        TeamID = issue.TeamID,
                        Status = new KeyValueItem { Id = issue.Category.ID, Name = issue.Status.Name },
                        CreatedDate = issue.CreatedDate,
                        Category = new KeyValueItem { Id = issue.Category.ID, Name = issue.Category.Name }
                       
                    };
                    if (issue.Priority != null)
                    {
                        issueDto.Priority = new KeyValueItem {Id = issue.Priority.ID, Name = issue.Priority.Name};
                    }


                    if (issue.ModifiedDate.HasValue && issue.ModifiedDate.Value > DateTime.MinValue && issue.ModifiedBy != null)
                    {
                        issueDto.LastModifiedDate = issue.ModifiedDate.Value.ToString("g");
                        issueDto.LastModifiedBy = issue.ModifiedBy.FirstName;
                    }

                    return issueDto;
                }
            }
            return null;
        }

        //var issueVM = new IssueVM { ID = bug.ID, Title = bug.Title, Description = bug.Description };
        //issueVM.OpenedBy = bug.CreatedBy.FirstName;
        //    issueVM.PriorityName = bug.PriorityName.Name;
        //    issueVM.StatusName = bug.StatusName.Name;
        //    issueVM.CategoryName = bug.CategoryName.Name;
        //    issueVM.Project = (bug.Project!=null?bug.Project.Name:"");
        //    issueVM.CreatedDate = bug.CreatedDate.ToShortDateString();


        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            var vm = new DashBoardItemSummaryVM();
            using (var db = new TeamEntitiesConn())
            {
                var statusCounts = db.Issues
                    .Where(s => s.TeamID == teamId)
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

        public IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count)
        {
            using (var db = new TeamEntitiesConn())
            {
               
                return db.Issues.AsNoTracking().Where(s => statusIds.Contains(s.StatusID)).OrderByDescending(s => s.CreatedDate)
                    .Take(count)
                    .Select(s => new IssueDetailVM
                    {
                        ID = s.ID,
                        Title = s.Title,
                        Description = s.Description,
                        PriorityName = s.Priority.Name,
                        StatusName = s.Status.Name,
                        CategoryName = s.Category.Name,
                        Priority = new KeyValueItem { Id = s.Project.ID, Name = s.Priority.Name },
                        Author = new UserDto { Id = s.CreatedBy.ID, Name = s.CreatedBy.FirstName },
                      //  Project = s.Project.Name,
                        Status =  new KeyValueItem { Id= s.Project.ID,  Name = s.Status.Name},
                        Project = new KeyValueItem { Id = s.Project.ID,Name = s.Project.Name },
                        CreatedDate = s.CreatedDate
                    }).ToList();
            }
        }
    }
}