using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using HealthCheck;

namespace HealthStatusService.Controllers
{
    public class HomeController : Controller
    {
        /*
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        */

        private readonly DataHealthCheck _dataHealthCheck;
        public HomeController()
        {
            _dataHealthCheck = new DataHealthCheck();
        }

        public ActionResult Index()
        {
            var data = _dataHealthCheck.GetData();
            return View(data);
        }
    }
}