using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using TeamBins.Common;
using TeamBins.Common.Infrastructure.Enums.TeamBins.Helpers.Enums;
using TeamBins.Common.ViewModels;

using TeamBins.DataAccessCore;
using TeamBins.Infrastrucutre;
using TeamBins.Infrastrucutre.Cache;
using TeamBins.Infrastrucutre.Extensions;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
   

    public class IssueManager : IIssueManager
    {
        private IActivityRepository activityRepository;
        private IIssueRepository issueRepository;
        private IProjectRepository iProjectRepository;
        private IUploadRepository uploadRepository;
        private IUserAuthHelper userSessionHelper;
        private readonly ICache cache;
        private IEmailManager emailManager;
        public IssueManager(IIssueRepository issueRepository, IProjectRepository iProjectRepository, 
            IActivityRepository activityRepository, IUserAuthHelper userSessionHelper, ICache cache,IUploadRepository uploadRepository,IEmailManager emailManager)
        {
            this.issueRepository = issueRepository;
            this.iProjectRepository = iProjectRepository;
            this.activityRepository = activityRepository;
            this.userSessionHelper = userSessionHelper;
            this.cache = cache;
            this.uploadRepository = uploadRepository;
            this.emailManager = emailManager;
        }
        public DashBoardItemSummaryVm GetDashboardSummaryVM(int teamId)
        {
            throw new NotImplementedException();
        }

        private bool IsImageType(string fileExtension)
        {
            return fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".tif";
        }

        public async  Task<IssueDetailVM> GetIssue(int id)
        {
            var issue = this.issueRepository.GetIssue(id, this.userSessionHelper.UserId);
            var uploads = await this.uploadRepository.GetUploads(id);
            var allUploads = uploads.ToList();
            foreach (var uploadDto in allUploads)
            {
                uploadDto.FileExtn = Path.GetExtension(uploadDto.FileName);
                if (IsImageType(uploadDto.FileExtn))
                {
                    issue.Images.Add(uploadDto);
                }
                else
                {
                    issue.Attachments.Add(uploadDto);
                }
            }
            return issue;
        }

        public IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IssueDetailVM>> GetIssuesAssignedToUser(int userId)
        {
            return await this.issueRepository.GetIssuesAssignedToUser(userId);
        }
        public IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int teamId, int count)
        {
            return this.issueRepository.GetIssuesGroupedByStatusGroup(count, teamId, this.userSessionHelper.UserId);
        }

        public ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion)
        {
            bool isStateChanged = false;
            var activity = new ActivityDto() { ObjectId = newVersion.Id, ObjectType = "Issue" };

            if (previousVersion == null)
            {
                activity.ObjectUrl = "issue/" + newVersion.Id;
                //activity.CreatedBy = 
                activity.Description = "Created";
                activity.ObjectTitle = model.Title;
                isStateChanged = true;
            }
            else
            {
                if (previousVersion.Status.Id != newVersion.Status.Id)
                {
                    // status of issue updated
                    activity.Description = "Changed status";
                    activity.NewState = newVersion.Status.Name;
                    activity.OldState = previousVersion.Status.Name;
                    activity.ObjectTitle = newVersion.Title;
                    isStateChanged = true;
                }
                else if (previousVersion.Category.Id != newVersion.Category.Id)
                {
                    activity.Description = "Changed category";
                    activity.NewState = newVersion.Category.Name;
                    activity.OldState = previousVersion.Category.Name;
                    activity.ObjectTitle = newVersion.Title;
                    isStateChanged = true;
                }


            }
            activity.CreatedTime = DateTime.Now;
            activity.TeamId = userSessionHelper.TeamId;

            activity.Actor = new UserDto { Id = userSessionHelper.UserId };

            if (isStateChanged)
            {
                var newId = activityRepository.Save(activity);
                return activityRepository.GetActivityItem(newId);

            }
            return null;
        }


        public async Task LoadDropdownData(CreateIssue issue)
        {
            issue.Projects =this.iProjectRepository.GetProjects(this.userSessionHelper.TeamId)
                       .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                       .ToList();


            var statuses = await this.cache.Get(CacheKey.Statuses, () =>  this.issueRepository.GetStatuses(),360);
            var priorities = await this.cache.Get(CacheKey.Priorities, () => this.issueRepository.GetPriorities(), 360);
            var catagories = await this.cache.Get(CacheKey.Categories, () => this.issueRepository.GetCategories(), 360);

            issue.Statuses = statuses.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
            issue.Priorities = priorities.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();
            issue.Categories = catagories.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();



        }

        public async Task<IssueDetailVM> SaveIssue(CreateIssue issue, List<IFormFile> files)
        {
            bool isNewIssue = issue.Id == 0;
            if (issue.ProjectId == 0)
            {
                var defaultProject = await iProjectRepository.GetDefaultProjectForTeamMember(this.userSessionHelper.TeamId,
                     this.userSessionHelper.UserId);
                if (defaultProject == null)
                {
                    throw new MissingSettingsException("Missing data", "Default project");
                }
                issue.ProjectId = defaultProject.Id;

            }
            if (issue.CategoryId == 0)
            {
                var categories =await  this.issueRepository.GetCategories();
                issue.CategoryId = categories.First().Id;
            }
            if (issue.PriorityId == 0)
            {
                var categories = await this.issueRepository.GetPriorities();
                issue.PriorityId = categories.First().Id;
            }
            if (issue.StatusId == 0)
            {
                var statuses = await this.issueRepository.GetStatuses();
                issue.StatusId = statuses.First().Id;
            }
            issue.CreatedById = this.userSessionHelper.UserId;
            issue.TeamID = this.userSessionHelper.TeamId;
            var issueId = this.issueRepository.SaveIssue(issue);
            var issueDetail = this.issueRepository.GetIssue(issueId, this.userSessionHelper.UserId);


            if (isNewIssue)
            {
                await emailManager.SendIssueCreatedEmail(issueDetail, this.userSessionHelper.TeamId);
                //.QueueBackgroundWorkItem(ct => SendMailAsync(user.Email));
            }
               

            return issueDetail;
            
        }

        public Task<int> StarIssue(int issueId)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            this.issueRepository.Delete(id, this.userSessionHelper.UserId);
        }
        public async Task<IEnumerable<UserDto>> GetNonIssueMembers(int issueId)
        {
            var members = await this.issueRepository.GetNonIssueMembers(issueId, this.userSessionHelper.TeamId);
            foreach (var userDto in members)
            {
                userDto.GravatarUrl = userDto.EmailAddress.ToGravatarUrl();
            }
            return members;
        }

        public async Task RemoveIssueMember(int issueId, int memberId)
        {
            await issueRepository.RemoveIssueMember(issueId, memberId);
        }

        public async Task SaveIssueAssignee(int issueId, int userId)
        {
            await issueRepository.SaveIssueMember(issueId, userId, userSessionHelper.UserId, IssueMemberRelationType.Assigned.ToString());
        }
        public async Task<IEnumerable<IssueMember>> GetIssueMembers(int issueId)
        {
            var members = await this.issueRepository.GetIssueMembers(issueId);
            members = members.Where(s => s.Relationtype == IssueMemberRelationType.Assigned.ToString()).ToList();
            foreach (var userDto in members)
            {
                userDto.Member.GravatarUrl = userDto.Member.EmailAddress.ToGravatarUrl();
            }
            return members;
        }

        public async Task StarIssue(int issueId, int userId, bool isRequestForToStar)
        {
            await this.issueRepository.StarIssue(issueId, this.userSessionHelper.UserId, isRequestForToStar);
        }

        public async Task SaveDueDate(int issueId, DateTime? dueDate)
        {
            await issueRepository.SaveDueDate(issueId, dueDate, this.userSessionHelper.UserId);
        }

     
    }
    public interface IIssueManager
    {
        //Task<IEnumerable<string>> SaveAttachment(IFormFile formFile, int parentId);
        Task<IEnumerable<IssueDetailVM>> GetIssuesAssignedToUser(int userId);
        Task RemoveIssueMember(int issueId, int memberId);
        Task<IEnumerable<IssueMember>> GetIssueMembers(int issueId);
        Task SaveIssueAssignee(int issueId, int userId);
        Task<IEnumerable<UserDto>> GetNonIssueMembers(int issueId);
        Task<int> StarIssue(int issueId);
        IEnumerable<IssueVM> GetIssues(List<int> statusIds, int count);
        Task<IssueDetailVM> SaveIssue(CreateIssue issue, List<IFormFile> files);
        DashBoardItemSummaryVm GetDashboardSummaryVM(int count);
        Task<IssueDetailVM> GetIssue(int id);
        ActivityDto SaveActivity(CreateIssue model, IssueDetailVM previousVersion, IssueDetailVM newVersion);

        IEnumerable<IssuesPerStatusGroup> GetIssuesGroupedByStatusGroup(int teamId, int count);
        void Delete(int id);
        Task LoadDropdownData(CreateIssue issue);

        Task StarIssue(int issueId, int userId, bool isRequestForToStar);

        Task SaveDueDate(int issueId, DateTime? dueDate);

    }
}