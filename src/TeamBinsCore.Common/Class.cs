using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;

using System.ComponentModel.DataAnnotations;




namespace TeamBins.Common.ViewModels
{
    public class DashBoardItemSummaryVm
    {
        public int CurrentItems { set; get; }
        public int ItemsInProgress { set; get; }
        public int NewItems { set; get; }
        public int BacklogItems { set; get; }
        public int CompletedItems { set; get; }

        public IEnumerable<ChartItem> IssueCountsByStatus { set; get; }
    }
    public class TeamProjectListVM
    {
        public List<ProjectVM> Projects { set; get; }
        public string TeamName { set; get; }
        public int TeamId { set; get; }

        public TeamProjectListVM()
        {
            Projects = new List<ProjectVM>();
        }

    }
    public class ProjectDetailsVM : ProjectVM
    {

        public List<MemberVM> Members { set; get; }
        public ProjectDetailsVM()
        {
            Members = new List<MemberVM>();
        }
    }
    public class ProjectVM
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public bool IsProjectOwner { set; get; }

        public bool IsDefaultProject { get; set; }
    }

    public class ItemCount
    {
        public string ItemName { set; get; }
        public int Count { set; get; }
        public int ItemId { get; set; }
    }

    public class ChartItem : ItemCount
    {
        public string Color { set; get; }
    }
    public class AddProjectMemberVM
    {
        public int ProjectID { set; get; }
        public string ProjectName { set; get; }
        public string Email { set; get; }
    }
    public class CreateProjectVM : ProjectVM
    {
        [Required]
        public string Name { set; get; }

        public int TeamId { get; set; }

        public int CreatedById { set; get; }
    }
    public class DeleteProjectConfirmVM : ProjectVM
    {
        public int DependableItemsCount { set; get; }
    }

    public class DashBoardVm
    {
        // The Request key ( A Hash of the UserID)
        public int TeamKey { set; get; }
        public bool IsCurrentUserTeamMember { set; get; }
        public int TeamId { set; get; }
        public List<ProjectDto> Projects { set; get; }
        public List<IssueDetailVM> RecentIssues { set; get; }
        public IEnumerable<IssueDetailVM> IssuesAssignedToMe { set; get; }
        public DashBoardVm()
        {
            Projects = new List<ProjectDto>();
            RecentIssues = new List<IssueDetailVM>();
            IssuesAssignedToMe = new List<IssueDetailVM>();
        }
    }

    public class ActivityDto
    {
        public string OldState { set; get; }
        public string NewState { set; get; }
        public int ObjectId { set; get; }
        public string ObjectType { set; get; }
        public int TeamId { set; get; }

        public string Description { set; get; }
        public UserDto Actor { set; get; }

        public string ObjectUrl { set; get; }

        public string ObjectTitle { set; get; }

        public DateTime CreatedTime { set; get; }
        public int CreatedById { get; set; }
    }
    public class ActivityVM
    {
        public int Id { set; get; }
        public string Author { set; get; }
        public string Activity { set; get; }
        public string ObjectTite { set; get; }
        public string ObjectURL { set; get; }
        public string NewState { set; get; }
        public string AuthorImageHash { set; get; }
        public string CreatedDate { set; get; }
    }
}


namespace TeamBins.Common.ViewModels
{
    public class IssueDetailVMProjectDto : BaseEntityDto
    {

    }
    public class ProjectDto : BaseEntityDto
    {

    }
    public class SettingsVm
    {
        public EditProfileVm Profile { set; get; }
        public ChangePasswordVM PasswordChange { set; get; }

        public DefaultIssueSettings IssueSettings { set; get; }

        public UserEmailNotificationSettingsVM NotificationSettings { set; get; }

        public SettingsVm()
        {
            this.Profile = new EditProfileVm();
            PasswordChange = new ChangePasswordVM();
            IssueSettings = new DefaultIssueSettings();
            NotificationSettings = new UserEmailNotificationSettingsVM();
        }
    }

    public class EditProfileVm
    {
        [Required]
        [StringLength(20)]
        public string Name { set; get; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }

        public int Id { set; get; }

    }

    public class ResetPasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string Password { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string ConfirmPassword { set; get; }

        public string ActivationCode { set; get; }
    }

    public class UserEmailNotificationSettingsVM
    {
        public int TeamId { set; get; }
        public IEnumerable<EmailSubscriptionVM> EmailSubscriptions { set; get; }
        public int UserId { get; set; }

        public UserEmailNotificationSettingsVM()
        {
            EmailSubscriptions = new List<EmailSubscriptionVM>();
        }
    }
    public class EmailSubscriptionVM
    {
        public int NotificationTypeId { set; get; }
        public string Name { set; get; }
        public bool IsSelected { set; get; }
    }
}