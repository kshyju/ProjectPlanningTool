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
    public class GraphPoint 
    {
        /// <summary>
        /// Date represented as a unix epoch
        /// </summary>
        public virtual long DateEpoch { get; set; }
        /// <summary>
        /// Value of the top (or only) point
        /// </summary>
        public virtual double? Value { get; set; }
    }

    public class DoubleGraphPoint : GraphPoint
    {
        /// <summary>
        /// Value of the bottom (or second) point
        /// </summary>
        public virtual double? BottomValue { get; set; }
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

            var list = new List<GraphPoint>();
            list.Add(new GraphPoint { DateEpoch = 132321, Value = 705});

            list.Add(new GraphPoint { DateEpoch = 212000 });
            list.Add(new GraphPoint { DateEpoch = 212134, Value = 233 });
            list.Add(new GraphPoint { DateEpoch = 432321, Value = 275 });
            list.Add(new GraphPoint { DateEpoch = 669321, Value = 145 });


            var s = list.Max(f => f.Value);

            Func<GraphPoint, double> getter = p => p.Value.GetValueOrDefault(0);

            var s52 = list.Where(f => f.DateEpoch == 212000).Max(getter);

            var s2 = list.Where(f => f.DateEpoch < 212134).Max(getter);


          //  var s23 = list.Where(f => f.DateEpoch < 132321).DefaultIfEmpty(new GraphPoint()).Max(getter);


            if (this.userSessionHelper.UserId > 0)
            {
                //return RedirectToAction("Index","Issue");
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
