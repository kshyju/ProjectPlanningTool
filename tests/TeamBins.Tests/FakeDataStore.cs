using NUnit.Framework;
using System.Collections.Generic;
using System.Web.Mvc;
using TeamBins.DataAccess;
using TechiesWeb.TeamBins.Controllers;
using TechiesWeb.TeamBins.ViewModels;
using System.Linq;

namespace TeamBins.Tests
{
    public class FakeDataStore
    {
        public List<Issue> Issues { set; get; }
        public List<Project> Projects { set; get; }
        public FakeDataStore()
        {
            Projects = new List<Project>();
            Issues = new List<Issue>();
        }
    }
}
