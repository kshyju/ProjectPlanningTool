using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TeamBins.Common.ViewModels
{
    public class DefaultIssueSettings
    {
        public int UserId { set; get; }
        public int TeamId { set; get; }
        public List<SelectListItem> Projects { set; get; }
        public int? SelectedProject { set; get; }

        public DefaultIssueSettings()
        {
            Projects = new List<SelectListItem>();
        }
    }
}