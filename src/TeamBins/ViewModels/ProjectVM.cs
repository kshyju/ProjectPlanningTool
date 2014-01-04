using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechiesWeb.TeamBins.ViewModels
{
    public class TeamProjectListVM
    {
        public List<ProjectVM> Projects { set; get; }
        public string TeamName { set; get; }
        public int TeamID { set; get; }

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
        public int ID { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public bool IsProjectOwner { set; get; }
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

        public int TeamID { get; set; }
    }
    public class DeleteProjectConfirmVM :ProjectVM
    {
        public int DependableItemsCount { set; get; }
    }
}
