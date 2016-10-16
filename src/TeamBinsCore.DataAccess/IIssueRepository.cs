using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

using TeamBins.DataAccessCore;

namespace TeamBinsCore.DataAccess
{

    public class IssueRepository : BaseRepo, IIssueRepository
    {
        public IssueRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task SaveDueDate(int issueId, DateTime? dueDate, int userId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync("UPDATE Issue SET DueDate=@dueDate,ModifiedDate=@modifiedDate,ModifiedByID=@userId WHERE ID=@issueId",
                    new { @modifiedDate = DateTime.Now, issueId, dueDate, userId });
            }
        }

        public async Task RemoveIssueMember(int issueId, int userId)
        {
            await DeleteIssueMemberRecord(issueId, userId, IssueMemberRelationType.Assigned.ToString());

        }
        public async Task StarIssue(int issueId, int userId, bool isRequestForToStar)
        {
            await DeleteIssueMemberRecord(issueId, userId, IssueMemberRelationType.Starred.ToString());
            if (isRequestForToStar)
                await AddIssueMemberRecord(issueId, userId, IssueMemberRelationType.Starred.ToString());
        }

        private async Task AddIssueMemberRecord(int issueId, int userId, string type)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync("INSERT INTO IssueMember(IssueID,MemberID,RelationType,CreatedDate,CreatedByID) VALUES(@id,@userId,@type,@dt,@userId);",
                    new { @id = issueId, @userId = userId, @type = type, @dt = DateTime.Now, });
            }
        }
        private async Task DeleteIssueMemberRecord(int issueId, int userId, string type)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync("DELETE FROM IssueMember WHERE IssueID=@id AND MemberID=@userId and RelationType=@type",
                    new { @id = issueId, @userId = userId, @type = type });
            }
        }

        public DashBoardItemSummaryVm GetDashboardSummaryVM(int teamId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NameValueItem> GetStatuses()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<NameValueItem>("SELECT * from Status  WITH (NOLOCK)");
                return projects;
            }

        }

        public IEnumerable<NameValueItem> GetPriorities()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<NameValueItem>("SELECT * from Priority  WITH (NOLOCK)");
                return projects;
            }

        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects = con.Query<CategoryDto>("SELECT * from Category  WITH (NOLOCK)");
                return projects;
            }
        }

        public IssueDetailVM GetIssue(int id, int requestingUserId)
        {




            var q = @"SELECT I.Id,I.Title,I.Description,ISNULL(I.Description,'') as Description,
                        U.FirstName + + ISNULL(U.LastName,'') as OpenedBy,
                        I.CreatedDate,
                        I.DueDate,
                        I.Active,
                        CASE WHEN IM.ID IS NULL THEN 0 ELSE 1 END AS IsStarredForUser	,
                        SG.Id,
                        SG.Code,
                        SG.Name,
                        C.Id,
                        C.Name,C.Code,
                        P.Id,
                        P.Name,
                        I.CreatedDate,
                        S.Id,S.Name,S.Code,
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
                        LEFT JOIN (SELECT IssueId,ID FROM IssueMember WITH (NOLOCK) 
                        WHERE RelationType='Starred' AND MemberID=@userId) IM ON IM.IssueID=I.ID
                        where I.Id=@id";


            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var projects =
                    con
                        .Query
                        <IssueDetailVM, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, UserDto,
                            IssueDetailVM>(q, (issue, sg, cat, priority, status, project, user)
                                =>
                            {
                                issue.Status = status;
                                issue.StatusGroup = sg;
                                issue.Priority = priority;
                                issue.Project = project;
                                issue.Author = user;
                                issue.Category = cat;
                                return issue;
                            }, new { @id = id, @userId = requestingUserId }, null, true, "Id,Id,Id,Id,Id,Id");
                return projects.FirstOrDefault();
            }

        }

        public IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return con.Query<IssueDetailVM>("SELECT Id,Title,Description,CreatedDate,DueDate FROM Issue").ToList();
            }
        }


        public async Task<IEnumerable<ChartItem>> GetIssueCountsPerStatus(int teamId)
        {
            var q = @"SELECT  S.ID ItemId,S.NAME ItemName,S.Color, COUNT(I.ID) COUNT						 
                        FROM STATUS S  WITH (NOLOCK)  
                        LEFT JOIN (SELECT I.ID,I.STATUSID FROM ISSUE I  WITH (NOLOCK) WHERE I.TeamId=@teamId) I ON I.STATUSID =S.ID
                        GROUP BY S.ID,S.NAME,S.Color";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<ChartItem>(q, new { teamId });
            }
        }
        public async Task<IEnumerable<ChartItem>> GetIssueCountsPerPriority(int teamId)
        {
            var q = @"SELECT  S.ID ItemId,S.NAME ItemName,Color, COUNT(I.ID) COUNT						 
                    FROM Priority S  WITH (NOLOCK)  
                    LEFT JOIN (SELECT I.ID,I.PriorityID FROM ISSUE I  WITH (NOLOCK) WHERE I.TeamId=@teamId) I ON I.PriorityID =S.ID
                    GROUP BY S.ID,S.NAME,Color";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<ChartItem>(q, new {  teamId });
            }
        }

        public async Task<IEnumerable<ItemCount>> GetIssueCountsPerProject(int teamId)
        {
            var q = @"SELECT  S.ID ItemId,S.NAME ItemName, COUNT(I.ID) COUNT						 
                    FROM Project S  WITH (NOLOCK)  
                    LEFT JOIN (SELECT I.ID,I.ProjectID FROM ISSUE I  WITH (NOLOCK) WHERE I.TeamId=@teamId) I ON I.ProjectID =S.ID
                    WHERE S.TeamId=@teamId
                    GROUP BY S.ID,S.NAME ORDER BY COUNT DESC";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                return await con.QueryAsync<ChartItem>(q, new { teamId });
            }
        }

        public async Task<IEnumerable<IssueDetailVM>> GetIssuesAssignedToUser(int userId)
        {
            var q = @"SELECT I.Id,I.Title,
                        U.FirstName + + ISNULL(U.LastName,'') as OpenedBy,
                        I.CreatedDate,
                        I.DueDate,
                        CASE WHEN IM.ID IS NULL THEN 0 ELSE 1 END AS IsStarredForUser	,
                        SG.Id,
                        SG.Code,
                        SG.Name,
                        SG.DisplayOrder,
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
                        INNER JOIN Priority P  WITH (NOLOCK) on P.Id = I.PriorityID 
						INNER JOIN IssueMember IssuesAssigned WITH (NOLOCK) on IssuesAssigned.IssueID= I.ID
					    LEFT JOIN (SELECT IssueId,ID FROM IssueMember WITH (NOLOCK) WHERE RelationType=@starredRelationShipType 
                                    AND MemberID=@userId) IM ON IM.IssueID=I.ID
                        WHERE I.Active=1 
						AND IssuesAssigned.MemberID=@userId AND IssuesAssigned.RelationType=@assignedRelationShipType";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                return await con.QueryAsync<IssueDetailVM, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, IssueDetailVM>
                    (q, (issue, sg, cat, priority, status)
                        =>
                    {
                        issue.Status = status;
                        issue.StatusGroup = sg;
                        issue.Priority = priority;
                        issue.Category = cat;
                        return issue;
                    }, new
                    {
                        @userId,
                        @starredRelationShipType = IssueMemberRelationType.Starred.ToString(),
                        @assignedRelationShipType = IssueMemberRelationType.Assigned.ToString()
                    }, null, splitOn: "Id,Id,Id,Id");

            }
        }
        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count, int teamId, int requestingUserId)
        {



            var results = new List<IssuesPerStatusGroup>();
            var q = @"SELECT I.Id,I.Title,
                        U.FirstName + + ISNULL(U.LastName,'') as OpenedBy,
                        I.CreatedDate,
                        I.DueDate,
                        CASE WHEN IM.ID IS NULL THEN 0 ELSE 1 END AS IsStarredForUser	,
                        SG.Id,
                        SG.Code,
                        SG.Name,
                        SG.DisplayOrder,
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
                        INNER JOIN Priority P  WITH (NOLOCK) on P.Id = I.PriorityID 
                        LEFT JOIN (SELECT IssueId,ID FROM IssueMember WITH (NOLOCK) WHERE RelationType='Starred' AND MemberID=@userId) IM ON IM.IssueID=I.ID
                        WHERE I.Active=1 and TeamId=@t";

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();

                var projects = con
                    .Query<IssueDetailVM, KeyValueItem, KeyValueItem, KeyValueItem, KeyValueItem, IssueDetailVM>
                    (q, (issue, sg, cat, priority, status)
                        =>
                    {
                        issue.Status = status;
                        issue.StatusGroup = sg;
                        issue.Priority = priority;
                        issue.Category = cat;
                        return issue;
                    }, new { @t = teamId, @userId = requestingUserId }, null, splitOn: "Id,Id,Id,Id");
                //var projects = con.Query<IssueDetailVM>(q);



                results = projects.GroupBy(s => s.StatusGroup, x => x,
                    (k, v) => new IssuesPerStatusGroup { GroupCode = k.Code, DisplayOrder = k.DisplayOrder, GroupName = k.Name, Issues = v.ToList() }).OrderBy(s => s.DisplayOrder).ToList();

                //grou py now
            }
            return results;
        }

        public int SaveIssue(CreateIssue issue)
        {

            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                if (issue.Id == 0)
                {
                    var q =
                        con.Query<int>(
                            @"INSERT INTO Issue(Title,Description,DueDate,CategoryId,StatusID,PriorityID,ProjectID,TeamId,Active,CreatedDate,CreatedByID) 
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
                else
                {
                    con.Execute(
                        "UPDATE Issue SET Title=@title,Description=@description,CategoryId=@catId,ProjectId=@projectId,StatusId=@statusId,PriorityId=@priorityId WHERE Id=@id",
                        new
                        {
                            @title = issue.Title,
                            @description = issue.Description,
                            @priorityId = issue.SelectedPriority,
                            @statusId = issue.SelectedStatus,
                            @catId = issue.SelectedCategory,
                            @id = issue.Id,
                            @projectId = issue.SelectedProject
                        });
                    return issue.Id;
                }


            }
        }

        public async Task SaveIssueMember(int issueId, int memberId, int createdById, string relationShipType)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                await con.ExecuteAsync("INSERT INTO IssueMember (IssueID,MemberID,RelationType,CreatedDate,CreatedByID) VALUES(@issueId,@memberId,@rtype,@dt,@userId)",
                    new { @issueId = issueId, @memberId = memberId, @rtype = relationShipType, @dt = DateTime.UtcNow, @userId = createdById });
            }
        }

        public void Delete(int id, int userId)
        {
            // Soft delete
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                con.Query<int>("UPDATE Issue SET Active=0, ModifiedDate=@dt,ModifiedByID=@userId where Id=@id",
                    new { @id = id, @dt = DateTime.UtcNow, @userId = userId });
            }
        }

        public async Task<IEnumerable<UserDto>> GetNonIssueMembers(int issueId, int teamId)
        {


            using (var con = new SqlConnection(ConnectionString))
            {
                var q = @"  SELECT U.ID,U.FirstName as Name,
                              U.EmailAddress
                              FROM [USER] U WITH (NOLOCK) 
                              INNER JOIN TeamMember TM  WITH (NOLOCK) ON TM.MemberID=U.ID 
                              WHERE TM.TeamId=@teamId  
                              AND U.ID NOT IN ( SELECT MemberID FROM IssueMember WHERE IssueID=@issueId)";
                con.Open();
                return await con.QueryAsync<UserDto>(q, new { teamId, issueId });
            }
        }
        public async Task<IEnumerable<IssueMember>> GetIssueMembers(int issueId)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                var q = @"SELECT TM.RelationType,U.ID,U.FirstName as Name,
                        U.EmailAddress
                        FROM [USER] U WITH (NOLOCK) 
                        INNER JOIN IssueMember TM  WITH (NOLOCK) ON TM.MemberID=U.ID 
                        WHERE TM.IssueID=@issueId  ";
                con.Open();
                return await con.QueryAsync<IssueMember, UserDto, IssueMember>(q, (im, u) =>
                {
                    im.Member = u;
                    return im;
                },
                    new { @issueId = issueId }, null, true, "ID");
            }
        }
    }


    public interface IIssueRepository
    {
        Task<IEnumerable<IssueDetailVM>> GetIssuesAssignedToUser(int userId);
        Task RemoveIssueMember(int issueId, int userId);
        IEnumerable<NameValueItem> GetStatuses();
        IEnumerable<CategoryDto> GetCategories();
        IEnumerable<NameValueItem> GetPriorities();
        IEnumerable<IssueDetailVM> GetIssues(List<int> statusIds, int count);
        IssueDetailVM GetIssue(int id, int requestingUserId);
        int SaveIssue(CreateIssue issue);
        DashBoardItemSummaryVm GetDashboardSummaryVM(int teamId);

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int count, int teamId, int requestingUserId);
        Task SaveIssueMember(int issueId, int memberId, int createdById, string relationShipType);
        void Delete(int id, int userId);

        Task<IEnumerable<UserDto>> GetNonIssueMembers(int issueId, int teamId);
        Task<IEnumerable<IssueMember>> GetIssueMembers(int issueId);
        Task<IEnumerable<ChartItem>> GetIssueCountsPerStatus(int teamId);
        Task StarIssue(int issueId, int userId, bool isRequestForToStar);
        Task SaveDueDate(int issueId, DateTime? dueDate, int userId);

        Task<IEnumerable<ChartItem>> GetIssueCountsPerPriority(int teamId);
        Task<IEnumerable<ItemCount>> GetIssueCountsPerProject(int teamId);

    }
}