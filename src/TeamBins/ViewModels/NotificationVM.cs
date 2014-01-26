using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechiesWeb.TeamBins.ViewModels
{    
    public class ActivityVM
    {
        public string Author { set; get; }
        public string Activity { set; get; }
        public string ObjectTite { set; get; }
        public string ObjectURL { set; get; }
        public string AuthorImageHash { set; get; }
        public string CreatedDateRelative { set; get; }
    }

}