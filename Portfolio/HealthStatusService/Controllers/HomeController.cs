using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using HealthCheck;

namespace HealthStatusService.Controllers
{
    public class HomeController : Controller
    {
        

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