using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TechiesWeb.TeamBins.Controllers
{
    public class ChartData
    {
        public string title { set; get; }
        public Axis axisX { set; get; }

        public Axis axisY { set; get; }

        public Data data { set; get; }
    }

    public class Axis
    {
        public string title { set; get; }
        public string titleFontColor { set; get; }
    }

    public class Data
    {
        public string color { set; get; }
        public List<DataPoint> data { set; get; }
    }

    public class DataPoint
    {
        public int x { set; get; }
        public int y { set; get; }
    }

    public class HomeController : Controller
    {

        public ActionResult ChartData()
        {
        var v=new ChartData();
        v.title = "Some";
        v.axisX = new Axis
        {
            title = "Title"
        };
        v.axisY = new Axis()
        {
            title = "Tes",
            titleFontColor = "red"
        };
        v.data = new Data
        {
            color = "blue",
            data = new List<DataPoint>()
            {
                new DataPoint {x = 10, y = 20},
                new DataPoint {x = 10, y = 20},
                new DataPoint {x = 10, y = 20},
                new DataPoint {x = 10, y = 20}
            }
        };
        return Json(v,JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult team(int id)
        {
            Session["TB_TeamID"] = id;
            return RedirectToAction("Index", "Issues");
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult GettingStarted()
        {
            return View();
        }
    }
}
