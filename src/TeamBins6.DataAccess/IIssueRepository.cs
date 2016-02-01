using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count);
        Task<int> SaveIssueMember(int issueId, int userId, string relationShipType);
    }

    public class IssueRepository : BaseRepo, IIssueRepository
    {
        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            throw new NotImplementedException();
        }

        public IssueDetailVM GetIssue(int id)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<IssueDetailVM>("SELECT * FROM Issue WHERE ID=@issueId", new { @issueId = id });
                return projects.FirstOrDefault();
            }

        }

        public IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<IssueDetailVM>("SELECT * FROM Issue");
                return projects;
            }
        }

        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count)
        {
            throw new NotImplementedException();
        }

        public int SaveIssue(CreateIssue issue)
        {

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var q =
                    con.Query(
                        "INSERT INTO Issue(Title,Description,DueDate,CategoryId,StatusID,PriorityID,ProjectID,TeamID) VALUES(@title,@description,@dueDate,@categoryId,@priortiyId,@projectId,@location,@teamId)",
                        new { @title=issue.Title, @description=issue.Description, @dueDate=issue.IssueDueDate, @categoryId=issue.SelectedCategory
                        , @priortiyId=issue.SelectedPriority, @projectId=issue.SelectedProject, @teamId =issue.TeamID});

                return 1;

            }
        }

        public Task<int> SaveIssueMember(int issueId, int userId, string relationShipType)
        {
            throw new NotImplementedException();
        }
    }


    //public class IssueRepository : IIssueRepository
    //{

    //    public int SaveIssue(CreateIssue issue)
    //    {
    //        Issue issueEntity = null;
    //        using (var db = new TeamEntitiesConn())
    //        {

    //            if (issue.Id > 0)
    //            {
    //                issueEntity = db.Issues.FirstOrDefault(s => s.ID == issue.Id);
    //                if (issueEntity == null)
    //                {
    //                    return 0;
    //                }
    //            }
    //            else
    //            {
    //                issueEntity = new Issue();
    //            }
    //            issueEntity.Title = issue.Title;
    //            issueEntity.Description = issue.Description;
    //            issueEntity.ProjectID = issue.ProjectID;
    //            issueEntity.TeamID = issue.TeamID;
    //            issueEntity.CategoryID = issue.SelectedCategory;

    //            issueEntity.CreatedByID = issue.CreatedByID;
    //            issueEntity.Location = issue.Iteration;
    //            issueEntity.StatusID = issue.SelectedStatus;
    //            issueEntity.PriorityID = issue.SelectedPriority;

    //            if (issueEntity.CategoryID == 0)
    //            {
    //                issueEntity.CategoryID = db.Categories.FirstOrDefault().ID;
    //            }
    //            if (issueEntity.StatusID == 0)
    //            {

    //                var status = db.Status.FirstOrDefault(s => s.Code == "New");
    //                issueEntity.StatusID = status.ID;
    //            }
    //            if (issueEntity.PriorityID == null || issueEntity.PriorityID.Value == 0)
    //            {
    //                var priority = db.Priorities.FirstOrDefault(s => s.Code == "Normal");
    //                issueEntity.PriorityID = priority.ID;
    //            }

    //            if (issue.Id == 0)
    //            {
    //                issueEntity.CreatedDate = DateTime.UtcNow;
    //                issueEntity.Active = true;
    //                db.Issues.Add(issueEntity);
    //            }
    //            else
    //            {
    //                issueEntity.ModifiedDate = DateTime.Now;
    //                issueEntity.ModifiedByID = issue.CreatedByID;

    //                db.Entry(issueEntity).State = EntityState.Modified;

    //            }

    //            db.SaveChanges();
    //            return issueEntity.ID;
    //        }
    //    }
    //    public IssueDetailVM GetIssue(int id)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var issue = db.Issues.FirstOrDefault(s => s.ID == id);
    //            if (issue != null)
    //            {
    //                var issueDto = new IssueDetailVM
    //                {
    //                    Id = issue.ID,
    //                    Title = issue.Title,
    //                    Description = issue.Description ?? string.Empty,
    //                    Author = new UserDto { Id = issue.CreatedBy.ID, Name = issue.CreatedBy.FirstName },
    //                    Project = new KeyValueItem { Id = issue.Project.ID, Name = issue.Project.Name },
    //                    TeamID = issue.TeamID,
    //                    Status = new KeyValueItem { Id = issue.Category.ID, Name = issue.Status.Name },
    //                    CreatedDate = issue.CreatedDate,
    //                    Category = new KeyValueItem { Id = issue.Category.ID, Name = issue.Category.Name },
    //                    StatusGroupCode = issue.Status.StatusGroup.Code

    //                };
    //                if (issue.Priority != null)
    //                {
    //                    issueDto.Priority = new KeyValueItem { Id = issue.Priority.ID, Name = issue.Priority.Name };
    //                }


    //                if (issue.ModifiedDate.HasValue && issue.ModifiedDate.Value > DateTime.MinValue && issue.ModifiedBy != null)
    //                {
    //                    issueDto.LastModifiedDate = issue.ModifiedDate.Value.ToString("g");
    //                    issueDto.LastModifiedBy = issue.ModifiedBy.FirstName;
    //                }

    //                return issueDto;
    //            }
    //        }
    //        return null;
    //    }

    //    //var issueVM = new IssueVM { Id = bug.Id, Title = bug.Title, Description = bug.Description };
    //    //issueVM.OpenedBy = bug.CreatedBy.FirstName;
    //    //    issueVM.PriorityName = bug.PriorityName.Name;
    //    //    issueVM.StatusName = bug.StatusName.Name;
    //    //    issueVM.CategoryName = bug.CategoryName.Name;
    //    //    issueVM.Project = (bug.Project!=null?bug.Project.Name:"");
    //    //    issueVM.CreatedDate = bug.CreatedDate.ToShortDateString();


    //    public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
    //    {
    //        var vm = new DashBoardItemSummaryVM();
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var statusCounts = db.Issues
    //                .Where(s => s.TeamID == teamId)
    //                .GroupBy(d => d.Status, g => g.ID, (k, i) => new
    //            ItemCount
    //                {
    //                    ItemId = k.ID,
    //                    ItemName = k.Name,
    //                    Count = i.Count()
    //                }).ToList();

    //            vm.IssueCountsByStatus = statusCounts;
    //        }

    //        return vm;
    //    }

    //    public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {

    //            return db.Status.GroupBy(s => s.StatusGroup, s => s, (k, items) =>
    //                new IssuesPerStatusGroup
    //                {
    //                    GroupName = k.Name,
    //                    GroupCode = k.Code,
    //                    Issues = items.SelectMany(d => d.Issues)
    //                        .Select(p => new IssueDetailVM
    //                        {
    //                            Id = p.ID,
    //                            Title = p.Title,
    //                            Description = p.Description,
    //                            PriorityName = p.Priority.Name,
    //                            StatusName = p.Status.Name,
    //                            CategoryName = p.Category.Name,
    //                            Category = new KeyValueItem { Id = p.Category.ID, Name = p.Category.Name },
    //                            Priority = new KeyValueItem { Id = p.Project.ID, Name = p.Priority.Name },
    //                            Author = new UserDto { Id = p.CreatedBy.ID, Name = p.CreatedBy.FirstName },
    //                            Status = new KeyValueItem { Id = p.Project.ID, Name = p.Status.Name },
    //                            Project = new KeyValueItem { Id = p.Project.ID, Name = p.Project.Name },
    //                            CreatedDate = p.CreatedDate
    //                        }).ToList()
    //                }).ToList();
    //        }

    //    }

    //    public IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {

    //            return db.Issues.AsNoTracking().Where(s => statusIds.Contains(s.StatusID))
    //                .OrderByDescending(s => s.CreatedDate)
    //                .Take(count)

    //                .Select(s => new IssueDetailVM
    //                {
    //                    Id = s.ID,
    //                    Title = s.Title,
    //                    Description = s.Description,
    //                    PriorityName = s.Priority.Name,
    //                    StatusName = s.Status.Name,
    //                    CategoryName = s.Category.Name,
    //                    Category = new KeyValueItem { Id = s.Category.ID, Name = s.Category.Name },
    //                    Priority = new KeyValueItem { Id = s.Project.ID, Name = s.Priority.Name },
    //                    Author = new UserDto { Id = s.CreatedBy.ID, Name = s.CreatedBy.FirstName },
    //                    //  Project = s.Project.Name,
    //                    Status = new KeyValueItem { Id = s.Project.ID, Name = s.Status.Name },
    //                    Project = new KeyValueItem { Id = s.Project.ID, Name = s.Project.Name },
    //                    CreatedDate = s.CreatedDate
    //                })
    //                .ToList();
    //        }
    //    }

    //    public async Task<int> SaveIssueMember(int issueId, int userId, string relationShipType)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var re =
    //                db.IssueMembers.FirstOrDefault(
    //                    s => s.IssueID == issueId && s.MemberID == userId && s.RelationType == relationShipType);
    //            if (re == null)
    //            {
    //                re = new IssueMember
    //                {
    //                    MemberID = userId,
    //                    IssueID = issueId,
    //                    CreatedByID = userId,
    //                    CreatedDate = DateTime.UtcNow
    //                };
    //                db.IssueMembers.Add(re);
    //                await db.SaveChangesAsync();
    //                return 1;

    //            }
    //            else
    //            {
    //                db.IssueMembers.Remove(re);
    //                await db.SaveChangesAsync();
    //                return 0;
    //            }
    //        }
    //    }
    //}
}