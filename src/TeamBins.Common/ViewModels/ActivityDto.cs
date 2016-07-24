//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace TeamBins.Common.ViewModels
//{
//    public class TeamProjectListVM
//    {
//        public List<ProjectVM> Projects { set; get; }
//        public string TeamName { set; get; }
//        public int TeamId { set; get; }

//        public TeamProjectListVM()
//        {
//            Projects = new List<ProjectVM>();
//        }

//    }
//    public class ProjectDetailsVM : ProjectVM
//    {

//        public List<MemberVM> Members { set; get; }
//        public ProjectDetailsVM()
//        {
//            Members = new List<MemberVM>();
//        }
//    }

//    public class AddProjectMemberVM
//    {
//        public int ProjectID { set; get; }
//        public string ProjectName { set; get; }
//        public string Email { set; get; }
//    }
//    public class CreateProjectVM : ProjectVM
//    {
//        [Required]
//        public string Name { set; get; }

//        public int TeamId { get; set; }

//        public int CreatedById { set; get; }
//    }
//    public class DeleteProjectConfirmVM : ProjectVM
//    {
//        public int DependableItemsCount { set; get; }
//    }

//    public class DashBoardVM
//    {
//        public int TeamId { set; get; }
//        public List<ProjectDto> Projects { set; get; }
//        public List<IssueDetailVM> RecentIssues { set; get; }
//        public IEnumerable<IssueDetailVM> IssuesAssignedToMe { set; get; }
//        public DashBoardVM()
//        {
//            Projects = new List<ProjectDto>();
//            RecentIssues = new List<IssueDetailVM>();
//            IssuesAssignedToMe = new List<IssueDetailVM>();
//        }
//    }

//    public class ActivityDto
//    {
//        public string OldState { set; get; }
//        public string NewState { set; get; }
//        public int ObjectId { set; get; }
//        public string ObjectType { set; get; }
//        public int TeamId { set; get; }

//        public string Description { set; get; }
//        public UserDto Actor  { set; get; }

//        public string ObjectUrl { set; get; }

//        public string ObjectTitle { set; get; }

//        public DateTime CreatedTime { set; get; }
//        public int CreatedById { get; set; }
//    }
//    public class ActivityVM
//    {
//        public int Id { set; get; }
//        public string Author { set; get; }
//        public string Activity { set; get; }
//        public string ObjectTite { set; get; }
//        public string ObjectURL { set; get; }
//        public string NewState { set; get; }
//        public string AuthorImageHash { set; get; }
//        public string CreatedDate { set; get; }
//    }
//}