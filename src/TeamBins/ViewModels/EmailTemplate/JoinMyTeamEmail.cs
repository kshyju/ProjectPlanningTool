using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechiesWeb.TeamBins.Models.EmailTemplate
{
    public class JoinMyTeamEmail
    {
        public string EmailAddress { set; get; }
        public string JoinURL { set; get; }
        public string TeamName { set; get; }
        public string InviterName { set; get; }
    }
}