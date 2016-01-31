using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using TeamBins.Common;

namespace TeamBins6.Controllers
{
    public class Test
    {
       // public List<SelectL> 
      //  [Required]
        public string Name { set; get; }
    }
    public class HomeController : Controller
    {
    
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
