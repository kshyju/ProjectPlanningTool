using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBins.Common.ViewModels;

namespace TeamBins.Common
{
    public class LoggedInSessionInfo
    {
        public int TeamId { set; get; }
        public int UserId { set; get; }
        public string UserDisplayName { set;get; }
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
