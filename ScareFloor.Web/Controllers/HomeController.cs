using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScareFloor.Web.Controllers
{
    public class HomeController : Controller
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=ScareFloor.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=tCrFeIzJOYwCIFHXtTpnnBek9cDNYyr/2z0Rt52eh/w=";

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
    }
}