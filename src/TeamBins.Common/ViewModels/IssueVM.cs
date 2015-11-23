using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TeamBins.Common.ViewModels
{
    public class NewIssueCommentVM
    {
        [Required]
        [AllowHtml]
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

   

    public class IssueDetailVM : IssueVM
    {
        public bool IsStarredForUser { set; get; }
        public List<DocumentVM> Images { set; get; }
        public List<DocumentVM> Attachments { set; get; }
        public List<MemberVM> Members { set; get; }
        public List<CommentVM> Comments { set; get; }
        public bool IsEditableForCurrentUser { set; get; }
        public int TeamID { set; get; }
        public int ProjectID { set; get; }
        public string LastModifiedDate { set; get; }
        public string LastModifiedBy { set; get; }
        public IssueDetailVM()
        {
            Images = new List<DocumentVM>();
            Attachments = new List<DocumentVM>();
            Members = new List<MemberVM>();
            Comments = new List<CommentVM>();
        }

       public KeyValueItem Priority { set; get; }
        public KeyValueItem Status { set; get; }
        public KeyValueItem Category { set; get; }

    }

    public class IssueVM
    {
        public int ID { set; get; }
        public string Title { set; get; }
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Description { set; get; }
        public string PriorityName { set; get; }
        public string CategoryName { set; get; }
        public string StatusName { set; get; }
        public string StatusCode { set; get; }
        public string OpenedBy { set; get; }
        public string LastModifiedBy { set; get; }
        public string Iteration { set; get; }
        public string Project { get; set; }
        public DateTime CreatedDate { set; get; }
        public string IssueDueDate { set; get; }
        public UserDto Author { get; set; }
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
        public List<SelectListItem> Cycles { set; get; }
        public List<SelectListItem> Iterations { set; get; }

        public string Version { set; get; }

        public List<HttpPostedFileBase> files { set; get; }

        public CreateIssue()
        {
            files = new List<HttpPostedFileBase>();
        }
    }
}
