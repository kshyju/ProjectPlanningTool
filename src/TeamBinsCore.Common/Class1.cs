using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeamBins.Common.ViewModels;


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
            public int RKey { set; get; }
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
