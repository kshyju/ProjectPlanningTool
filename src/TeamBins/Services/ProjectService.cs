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
            return repo.GetProjects(teamId).
                         Select(s => new SelectListItem { Value = s.ID.ToString(), Text = s.Name }).ToList();
        }
        public static List<SelectListItem> GetPriorities(IRepositary repo)
        {
            return repo.GetPriorities().
                    Select(s => new SelectListItem { Value = s.ID.ToString(), Text = s.Name }).ToList();
            
        }

        public static List<SelectListItem> GetIterations()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value="SPRNT", Text="Sprint"},
                new SelectListItem { Value="BKLOG", Text="BackLog"},              
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
            return repo.GetStatuses().
                Select(s => new SelectListItem { Value = s.ID.ToString(), Text = s.Name }).ToList();
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
            return repo.GetCategories().
                Select(s => new SelectListItem { Value = s.ID.ToString(), Text = s.Name }).ToList();
        }
        public static List<SelectListItem> GetCycles(IRepositary repo)
        {
            List<SelectListItem> projectList = new List<SelectListItem>();           
           
           projectList.Add(new SelectListItem { Value = "1", Text = "1"});
           projectList.Add(new SelectListItem { Value = "2", Text = "2" });
           projectList.Add(new SelectListItem { Value = "3", Text = "3" });
           projectList.Add(new SelectListItem { Value = "4", Text = "4" });
           projectList.Add(new SelectListItem { Value = "5", Text = "5" });
            
            return projectList;
        }
    }
}