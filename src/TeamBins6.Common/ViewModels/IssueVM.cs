using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;


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
            this.Statuses=new List<SelectListItem>();
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
