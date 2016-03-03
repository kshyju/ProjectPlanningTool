using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;
using TeamBins6.Common;
namespace TeamBins.DataAccess
{

    public interface IIssueRepository
    {
        IEnumerable<NameValueItem> GetStatuses();
        IEnumerable<CategoryDto> GetCategories();
        IEnumerable<NameValueItem> GetPriorities();
        IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count);
        IssueDetailVM GetIssue(int id);
        int SaveIssue(CreateIssue issue);
        DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId);

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count);
        Task<int> SaveIssueMember(int issueId, int userId, string relationShipType);
        void Delete(int id, int userId);
    }

    public class IssueRepository : BaseRepo, IIssueRepository
    {
        public DashBoardItemSummaryVM GetDashboardSummaryVM(int teamId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NameValueItem> GetStatuses()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<NameValueItem>("SELECT * from Status");
                return projects;
            }

        }
        public IEnumerable<NameValueItem> GetPriorities()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<NameValueItem>("SELECT * from Priority");
                return projects;
            }

        }
        public IEnumerable<CategoryDto> GetCategories()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<CategoryDto>("SELECT * from Category");
                return projects;
            }
        }
        public IssueDetailVM GetIssue(int id)
        {
            var q = @"SELECT I.Id,I.Title,I.Description,ISNULL(I.Description,'') as Description,
                        U.FirstName + + ISNULL(U.LastName,'') as OpenedBy,
                        I.CreatedDate,
                        I.Active,
                        SG.Id,
                        SG.Code,
                        SG.Name,
                        C.Id,
                        C.Name,C.Code,
                        P.Id,
                        P.Name,
                        I.CreatedDate,
                        S.Id,S.Name,
                        Pr.Id,
                        Pr.Name,
                        U.Id,U.FirstName as Name,
						U.EmailAddress 						 
                        from Issue I  WITH (NOLOCK)
                        INNER JOIN Status S  WITH (NOLOCK) ON I.StatusID =S.Id
                        INNER JOIN StatusGroup SG  WITH (NOLOCK) ON SG.Id =S.StatusGroupId
                        INNER JOIN dbo.[USer] U  WITH (NOLOCK) ON U.Id=I.CreatedByid
                        INNER JOIN Category C  WITH (NOLOCK) on C.Id = I.CategoryID
                        INNER JOIN Priority P  WITH (NOLOCK) on P.Id = I.PriorityID
                        INNER JOIN Project Pr  WITH (NOLOCK) ON Pr.Id=I.ProjectID
                        WHERE I.Id=@id";

          
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<IssueDetailVM, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem,UserDto, IssueDetailVM >(q, (issue, sg, cat, priority, status,project,user)
                 => {
                     issue.Status = status;
                     issue.StatusGroup = sg;
                     issue.Priority = priority;
                        issue.Project = project;
                        issue.Author = user;
                     issue.Category = cat; return issue;
                 },  new { @id = id },null,true, "Id,Id,Id,Id,Id,Id");
                return projects.FirstOrDefault();
            }

        }

        public IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<IssueDetailVM>("SELECT Id,Title,Description,CreatedDate,DueDate as IssueDueDate FROM Issue");
                return projects;
               // Status
            }
        }

        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count)
        {
            var results = new List<IssuesPerStatusGroup>();
            var q = @"SELECT I.Id,I.Title,
                        U.FirstName + + ISNULL(U.LastName,'') as OpenedBy,
                        I.CreatedDate,
                        SG.Id,
                        SG.Code,
                        SG.Name,
                        C.Id,
                        C.Name,C.Code,
                        P.Id,
                        P.Name,
                       
                        S.Id,S.Name						 
                        from Issue I  WITH (NOLOCK)
                        INNER JOIN Status S  WITH (NOLOCK) ON I.StatusID =S.Id
                        INNER JOIN StatusGroup SG  WITH (NOLOCK) ON SG.Id =S.StatusGroupId
                        INNER JOIN dbo.[USer] U  WITH (NOLOCK) ON U.Id=I.CreatedByid
                        INNER JOIN Category C  WITH (NOLOCK) on C.Id = I.CategoryID
                        INNER JOIN Priority P  WITH (NOLOCK) on P.Id = I.PriorityID WHERE I.Active=1";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
               
                var projects = con.Query<IssueDetailVM, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, IssueDetailVM >
                    (q, (issue, sg,cat,priority,status)
                    => { issue.Status = status;
                           issue.StatusGroup = sg;
                           issue.Priority = priority;
                           issue.Category = cat; return issue; }, splitOn: "Id,Id,Id,Id");
                //var projects = con.Query<IssueDetailVM>(q);



                results = projects.GroupBy(s => s.StatusGroup.Code, x => x,
                    (k, v) => new IssuesPerStatusGroup {GroupCode = k, GroupName = k,Issues = v.ToList()}).ToList();

                //grou py now
            }
            return results;
        }

        public int SaveIssue(CreateIssue issue)
        {

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var q =
                    con.Query<int>(
                        @"INSERT INTO Issue(Title,Description,DueDate,CategoryId,StatusID,PriorityID,ProjectID,TeamID,Active,CreatedDate,CreatedByID) 
                        VALUES(@title,@description,@dueDate,@categoryId,@statusId,@priortiyId,@projectId,@teamId,1,@createdDate,@userId);SELECT CAST(SCOPE_IDENTITY() as int)",
                        new
                        {
                            @title = issue.Title,
                            @description = issue.Description,
                            @dueDate = issue.IssueDueDate,
                            @categoryId = issue.SelectedCategory
                        ,
                            @statusId = issue.SelectedStatus,
                            @priortiyId = issue.SelectedPriority,
                            @projectId = issue.SelectedProject,
                            @teamId = issue.TeamID,
                            @createdDate = DateTime.Now,
                            @userId = issue.CreatedByID
                        });

                return q.First();

            }
        }

        public Task<int> SaveIssueMember(int issueId, int userId, string relationShipType)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id,int userId)
        {
            // Soft delete
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Query<int>("UPDATE Issue SET Active=0, ModifiedDate=@dt,ModifiedByID=@userId where Id=@id", new {@id=id, @dt=DateTime.UtcNow,@userId= userId });
            }
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
    //                issueEntity = db.Issues.FirstOrDefault(s => s.Id == issue.Id);
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
    //                issueEntity.CategoryID = db.Categories.FirstOrDefault().Id;
    //            }
    //            if (issueEntity.StatusID == 0)
    //            {

    //                var status = db.Status.FirstOrDefault(s => s.Code == "New");
    //                issueEntity.StatusID = status.Id;
    //            }
    //            if (issueEntity.PriorityID == null || issueEntity.PriorityID.Value == 0)
    //            {
    //                var priority = db.Priorities.FirstOrDefault(s => s.Code == "Normal");
    //                issueEntity.PriorityID = priority.Id;
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
    //            return issueEntity.Id;
    //        }
    //    }
    //    public IssueDetailVM GetIssue(int id)
    //    {
    //        using (var db = new TeamEntitiesConn())
    //        {
    //            var issue = db.Issues.FirstOrDefault(s => s.Id == id);
    //            if (issue != null)
    //            {
    //                var issueDto = new IssueDetailVM
    //                {
    //                    Id = issue.Id,
    //                    Title = issue.Title,
    //                    Description = issue.Description ?? string.Empty,
    //                    Author = new UserDto { Id = issue.CreatedBy.Id, Name = issue.CreatedBy.FirstName },
    //                    Project = new KeyValueItem { Id = issue.Project.Id, Name = issue.Project.Name },
    //                    TeamID = issue.TeamID,
    //                    Status = new KeyValueItem { Id = issue.Category.Id, Name = issue.Status.Name },
    //                    CreatedDate = issue.CreatedDate,
    //                    Category = new KeyValueItem { Id = issue.Category.Id, Name = issue.Category.Name },
    //                    StatusGroupCode = issue.Status.StatusGroup.Code

    //                };
    //                if (issue.Priority != null)
    //                {
    //                    issueDto.Priority = new KeyValueItem { Id = issue.Priority.Id, Name = issue.Priority.Name };
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
    //                .GroupBy(d => d.Status, g => g.Id, (k, i) => new
    //            ItemCount
    //                {
    //                    ItemId = k.Id,
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
    //                            Id = p.Id,
    //                            Title = p.Title,
    //                            Description = p.Description,
    //                            PriorityName = p.Priority.Name,
    //                            StatusName = p.Status.Name,
    //                            CategoryName = p.Category.Name,
    //                            Category = new KeyValueItem { Id = p.Category.Id, Name = p.Category.Name },
    //                            Priority = new KeyValueItem { Id = p.Project.Id, Name = p.Priority.Name },
    //                            Author = new UserDto { Id = p.CreatedBy.Id, Name = p.CreatedBy.FirstName },
    //                            Status = new KeyValueItem { Id = p.Project.Id, Name = p.Status.Name },
    //                            Project = new KeyValueItem { Id = p.Project.Id, Name = p.Project.Name },
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
    //                    Id = s.Id,
    //                    Title = s.Title,
    //                    Description = s.Description,
    //                    PriorityName = s.Priority.Name,
    //                    StatusName = s.Status.Name,
    //                    CategoryName = s.Category.Name,
    //                    Category = new KeyValueItem { Id = s.Category.Id, Name = s.Category.Name },
    //                    Priority = new KeyValueItem { Id = s.Project.Id, Name = s.Priority.Name },
    //                    Author = new UserDto { Id = s.CreatedBy.Id, Name = s.CreatedBy.FirstName },
    //                    //  Project = s.Project.Name,
    //                    Status = new KeyValueItem { Id = s.Project.Id, Name = s.Status.Name },
    //                    Project = new KeyValueItem { Id = s.Project.Id, Name = s.Project.Name },
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