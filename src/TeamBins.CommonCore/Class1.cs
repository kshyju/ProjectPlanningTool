using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using TeamBins.Common.ViewModels;
using TeamBins.CommonCore;

namespace TeamBins.Common.Infrastructure.Enums
{
    namespace TeamBins.Helpers.Enums
    {
        public enum IssueMemberRelationType
        {
            Assigned,
            Starred,
            Following
        }

        public enum ActivityObjectType
        {
            Issue,
            IssueComment
        }

        public enum NotificationTypeCode
        {
            NewComment,
            NewIssue
        }

        public enum MessageType
        {
            Success,
            Warning,
            Error,
            Info
        }
    }
}




namespace TeamBins.Common.ViewModels
{
    public class KeyValueItem : IEquatable<KeyValueItem>
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }

        public string Color { set; get; }

        public int DisplayOrder { set; get; }

        public bool Equals(KeyValueItem obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            KeyValueItem p = (KeyValueItem)obj;
            return (Id == p.Id);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
namespace TeamBins.Common.ViewModels
{
    public class LoginVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        public bool RememberMe { set; get; }

    }
    public class AccountSignupVM
    {
        [Required]
        [StringLength(20)]
        public string Name { set; get; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must have minimum of 6 characters")]
        public string Password { set; get; }

        public string ReturnUrl { set; get; }
    }
    public class ForgotPasswordVM
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }
    }


    public class NewIssueCommentVM
    {
        [Required]
        //[AllowHtml]
        public string CommentBody { set; get; }
        [Required]
        public int IssueID { set; get; }
    }
    public class IssueListVM
    {
        public bool IsReadonlyView { set; get; }
        public bool IsPublicTeam { set; get; }
        public int TeamID { set; get; }
        public List<IssueVM> Bugs { set; get; }

        public IssueListVM()
        {
            Bugs = new List<IssueVM>();
        }

        public string CurrentTab { set; get; }
        public bool ProjectsExist { set; get; }
        public bool IsUserTeamMember { set; get; }
        //public bool IsDefaultProjectSet { set; get; }
    }
    public class DeleteIssueConfirmationVM : IssueVM
    {

    }



    public class IssueDetailWithStatusGroup : IssueDetailVM
    {

        public string StatusGroupName { set; get; }

    }
    public class IssueDetailVM : IssueVM
    {
        public bool IsReadOnly { set; get; }
        public bool IsStarredForUser { set; get; }
        public List<DocumentVM> Images { set; get; }
        public List<DocumentVM> Attachments { set; get; }
        public IEnumerable<UserDto> Members { set; get; }
        //    public List<CommentVM> Comments { set; get; }
        public bool IsEditableForCurrentUser { set; get; }
        public int TeamID { set; get; }
        public int ProjectID { set; get; }
        public string LastModifiedDate { set; get; }
        public string LastModifiedBy { set; get; }
        public IssueDetailVM()
        {
            Images = new List<DocumentVM>();
            Attachments = new List<DocumentVM>();
            Members = new List<UserDto>();
            //     Comments = new List<CommentVM>();
        }

        public KeyValueItem Priority { set; get; }
        public KeyValueItem Status { set; get; }
        public KeyValueItem Category { set; get; }
        public KeyValueItem Project { set; get; }
        public string StatusGroupCode { get; set; }
        public KeyValueItem StatusGroup { set; get; }



    }

    public class StatusDto
    {
        // this could be a dapper PR ( Make the mapping work without making the properties (ClassName+Id)
        public int StatusId { set; get; }
        public string StatusName { set; get; }
        public string StatusCode { set; get; }
    }

    public class StatusGroupVm
    {
        public string Name { set; get; }
        public List<IssueVM> Issues { set; get; }
    }
    public class IssuesPerStatusGroup
    {
        public string GroupCode { set; get; }
        public string GroupName { set; get; }
        public List<IssueDetailVM> Issues { set; get; }

        public int DisplayOrder { set; get; }
    }
    public class IssueVM
    {
        public int Id { set; get; }
        public string Title { set; get; }
        [DataType(DataType.MultilineText)]
        // [AllowHtml]
        public string Description { set; get; }
        public string PriorityName { set; get; }
        public string CategoryName { set; get; }
        public string StatusName { set; get; }
        public string StatusCode { set; get; }
        public string OpenedBy { set; get; }
        public string LastModifiedBy { set; get; }
        public string Iteration { set; get; }
        public string ProjectName { get; set; }
        public DateTime CreatedDate { set; get; }

        public DateTime? DueDate { set; get; }
        public string IssueDueDate { set; get; }
        public UserDto Author { get; set; }

        public bool Active { set; get; }
    }

    public class DocumentVM
    {
        public int ID { set; get; }
        public string FileName { set; get; }
        public string FileKey { set; get; }
        public string FileExtn { set; get; }
    }
    public class ImageVM : DocumentVM
    {

    }

    public class CreateIssue : IssueDetailVM
    {
        public int CreatedByID;
        public bool IncludeIssueInResponse { set; get; }
        public bool IsFromModalWindow { set; get; }
        public int SelectedStatus { set; get; }

        public int SelectedPriority { set; get; }

        public int SelectedProject { set; get; }

        public int SelectedCategory { set; get; }

        public string SelectedIteration { set; get; }

        public int SelectedCycle { set; get; }

        
        public List<SelectListItem> Statuses { set; get; }
        public List<SelectListItem> Categories { set; get; }
        public List<SelectListItem> Projects { set; get; }
        public List<SelectListItem> Priorities { set; get; }
        //public List<SelectListItem> Cycles { set; get; }
        //public List<SelectListItem> Iterations { set; get; }

        public string Version { set; get; }

        // public List<HttpPostedFileBase> files { set; get; }

        public CreateIssue()
        {
            this.Statuses = new List<SelectListItem>();
            this.Categories = new List<SelectListItem>();
            this.Projects = new List<SelectListItem>();
            this.Priorities = new List<SelectListItem>();


            //      files = new List<HttpPostedFileBase>();
        }

        public CreateIssue(IssueDetailVM issueDetail)
        {
            this.Title = issueDetail.Title;
            this.Description = issueDetail.Description;
            this.SelectedProject = issueDetail.Project.Id;
            this.SelectedStatus = issueDetail.Status.Id;
            this.SelectedCategory = issueDetail.Category.Id;
            this.SelectedPriority = issueDetail.Priority.Id;


            this.Statuses = new List<SelectListItem>();
            this.Categories = new List<SelectListItem>();
            this.Projects = new List<SelectListItem>();
            this.Priorities = new List<SelectListItem>();

        }
    }

}

namespace TeamBins.CommonCore
{

    public class CategoryDto : NameValueItem
    {

    }

    public class NameValueItem
    {
        public string Name { set; get; }
        public string Code { set; get; }
        public int Id { set; get; }
    }

    public class UserAccountDto : UserDto
    {

        public string Password { set; get; }
        public int? DefaultTeamId { set; get; }
    }

    public class UserDto
    {
        public int Id { set; get; }
        public string EmailAddress { set; get; }
        public string Name { set; get; }
        public string GravatarUrl { get; set; }

        public double? TestVal { set; get; }


    }

    public class IssueMember
    {
        public UserDto Member { set; get; }
        public string Relationtype { set; get; }
    }
    public class AppUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string Password { set; get; }
    }


    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".


        public class LoggedInSessionInfo
        {
            public int TeamId { set; get; }
            public int UserId { set; get; }
            public string UserDisplayName { set; get; }
        }

        public class TeamDto : BaseEntityDto
        {
            public bool IsPublic { set; get; }
            public int CreatedById { get; set; }
            public int MemberCount { set; get; }

            public bool IsRequestingUserTeamOwner { set; get; }
        }


        public class TeamListVM
        {
            public List<TeamDto> Teams { set; get; }
            public TeamListVM()
            {
                Teams = new List<TeamDto>();
            }
        }

        public class UserMenuHeaderVM
        {
            public List<TeamDto> Teams { set; get; }
            public int SelectedTeam { set; get; }
            //  public int CurrentTeamID { set; get; }
            public string CurrentTeamName { set; get; }
            public string UserDisplayName { set; get; }
            public string UserEmailAddress { set; get; }
            public UserMenuHeaderVM()
            {
                Teams = new List<TeamDto>();
            }
        }

        public class EditTeamVm : TeamVM
        {
            //public List<SelectListItem> Visibilities { set; get; }
            //public int Visibility { set; get; }
        }

        public class TeamVM : TeamDto
        {
            public bool IsPublic { set; get; }
            public IEnumerable<TeamMemberDto> Members { set; get; }
            public List<MemberInvitation> MembersInvited { set; get; }

            public TeamVM()
            {
                Members = new List<TeamMemberDto>();
                MembersInvited = new List<MemberInvitation>();
            }
        }
        public class MemberInvitation
        {
            public string EmailAddress { set; get; }
            public string DateInvited { set; get; }
            public string AvatarHash { set; get; }
        }


        public class TeamMemberDto : UserDto
        {
            public DateTime JoinedDate { set; get; }
            public DateTime LastLoginDate { set; get; }
        }


        public class MemberVM
        {
            public int MemberID { set; get; }
            public string Name { set; get; }
            [Required]
            [DataType(DataType.EmailAddress)]
            [StringLength(100)]
            public string EmailAddress { set; get; }
            public string GravatarUrl { set; get; }
            public string JoinedDate { set; get; }
            public string MemberType { set; get; }
            public string AvatarHash { set; get; }
            public string JobTitle { set; get; }
            public string LastLoginDate { set; get; }

            public int? DefaultProjectId { set; get; }
        }
        public class AddTeamMemberRequestVM : MemberVM
        {
            public int Id { set; get; }
            public int TeamID { set; get; }
            public int CreatedById { set; get; }

            public string ActivationCode { set; get; }

            public DateTime CreatedDate { set; get; }

            public string SiteBaseUrl { set; get; }

            public UserDto CreatedBy { set; get; }

            public TeamDto Team { set; get; }

            public AddTeamMemberRequestVM()
            {
                this.CreatedBy = new UserDto();
                this.Team = new TeamDto();
            }
        }



        public class CommentVM
        {
            public int IssueId;
            public int Id { set; get; }
            public string CommentText { set; get; }
            public string AuthorName { set; get; }
            public string AvatarHash { set; get; }
            public string CreatedDateRelative { set; get; }
            public DateTime CreatedDate { set; get; }
            public bool IsOwner { set; get; }
            public UserDto Author { set; get; }
            public IssueVM Issue { set; get; }
        }
        public abstract class BaseEntityDto
        {
            public int Id { set; get; }
            public string Name { set; get; }

        }
        public class TeamActivityVM
        {
            public string LinkURL { set; get; }
            public int UserID { set; get; }
            public string UserName { set; get; }
            public string Activity { set; get; }
            public string ItemName { set; get; }
            public string EventDate { get; set; }
            public string EventDateRelative { get; set; }
            public string NewState { set; get; }
        }
    
}
