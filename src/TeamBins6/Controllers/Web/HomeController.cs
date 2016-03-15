using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using StackExchange.Exceptional;
using TeamBins.Common;
using TeamBins6.Infrastrucutre.Services;

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
        private IUserSessionHelper userSessionHelper;

        public HomeController(IUserSessionHelper userSessionHelper)
        {
            this.userSessionHelper = userSessionHelper;
        }
        public IActionResult Index()
        {
            
            if (this.userSessionHelper.UserId > 0)
            {
                return RedirectToAction("Index","Issue");
           }
               

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
