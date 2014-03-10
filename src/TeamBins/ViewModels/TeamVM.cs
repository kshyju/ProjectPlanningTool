using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechiesWeb.TeamBins.ViewModels
{
    public class TeamListVM
    {
        public List<TeamVM> Teams { set; get; }
        public TeamListVM()
        {
            Teams = new List<TeamVM>();
        }
    }

    

    public class TeamVM
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public int MemberCount { set; get; }
        public List<MemberVM> Members { set; get; }
        public List<MemberInvitation> MembersInvited { set; get; }
        public bool IsTeamOwner { set; get; }
        public TeamVM()
        {
            Members = new List<MemberVM>();
            MembersInvited = new List<MemberInvitation>();           
        }        
    }
    public class MemberInvitation
    {
        public string EmailAddress { set; get; }
        public string DateInvited { set; get; }
        public string AvatarHash { set; get; }
    }
    public class MemberVM
    {
        public int MemberID { set; get; }
        public string Name { set; get; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string EmailAddress { set; get; }
        public string EmailHash { set; get; }
        public string JoinedDate { set; get; }
        public string MemberType { set; get; }
        public string AvatarHash { set; get; }
        public string JobTitle { set; get; }
        public string LastLoginDate { set; get; }
    }
    public class AddTeamMemberRequestVM :MemberVM
    {
        public int TeamID { set; get; }      
    }

    public class CommentVM
    {
        public int ID { set;get;}
        public string CommentBody { set;get;}
        public string AuthorName { set;get;}
        public string AvatarHash { set; get; }
        public string CreatedDateRelative  {set;get;}
        public string CreativeDate { set;get;}
        public bool IsOwner { set; get; }
    }
    public class TeamActivityVM
    {
        public string LinkURL { set; get; }
        public int UserID { set; get; }
        public string UserName { set; get; }
        public string Activity { set; get; }
        public string ItemName { set; get; }
        public string EventDate { get;set;}
        public string EventDateRelative { get; set; }
        public string NewState { set; get; }
    }

}
