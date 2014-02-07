using Planner.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeamBins.DataAccess;

namespace TeamBins.Services
{
    public class ProjectService
    {
        public static List<SelectListItem> GetProjects(IRepositary repo, int teamId)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetProjects(teamId);
            foreach (var pro in projects)
            {
                projectList.Add(new SelectListItem { Value = pro.ID.ToString(), Text = pro.Name });
            }
            return projectList;
        }
        public static List<SelectListItem> GetPriorities(IRepositary repo)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetPriorities();
            foreach (var pro in projects)
            {
                projectList.Add(new SelectListItem { Value = pro.ID.ToString(), Text = pro.Name });
            }
            return projectList;
        }

        public static List<SelectListItem> GetIterations()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value="SPRNT", Text="Sprint"},
                new SelectListItem { Value="BKLOG", Text="BackLog"},
               // new SelectListItem { Value="ARCHV", Text="Archieve"},
            };
        }
        public static string GetIterationName(string iterationCode)
        {
            switch(iterationCode)
            {
                case "ARCHV": return "Archived";
                              break;
                case "BKLOG": return "BackLog";
                              break;
                default: return "Current Sprint";
                              break;

                    
            }

        }

        public static List<SelectListItem> GetStatuses(IRepositary repo)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetStatuses();
            foreach (var pro in projects)
            {
                projectList.Add(new SelectListItem { Value = pro.ID.ToString(), Text = pro.Name });

            }
            return projectList;
        }
        public static List<SelectListItem> GetStatuses(IRepositary repo, List<string> itemsToShow)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetStatuses();
            foreach (var pro in projects)
            {
                if (itemsToShow.IndexOf(pro.Name)>=0)
                {
                    projectList.Add(new SelectListItem { Value = pro.ID.ToString(), Text = pro.Name });
                }

            }
            return projectList;
        }
        public static List<SelectListItem> GetCategories(IRepositary repo)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetCategories();
            foreach (var pro in projects)
            {
                projectList.Add(new SelectListItem { Value = pro.ID.ToString(), Text = pro.Name });

            }
            return projectList;
        }
        public static List<SelectListItem> GetCycles(IRepositary repo)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();
            var projects = repo.GetCategories();
           
           projectList.Add(new SelectListItem { Value = "1", Text = "1"});
           projectList.Add(new SelectListItem { Value = "2", Text = "2" });
           projectList.Add(new SelectListItem { Value = "3", Text = "3" });
           projectList.Add(new SelectListItem { Value = "4", Text = "4" });
           projectList.Add(new SelectListItem { Value = "5", Text = "5" });
            
            return projectList;
        }
    }
}