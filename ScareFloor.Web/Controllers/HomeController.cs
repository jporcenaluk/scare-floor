using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ScareFloor.Web.Controllers
{
    public class HomeController : Controller
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=ScareFloor.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=tCrFeIzJOYwCIFHXtTpnnBek9cDNYyr/2z0Rt52eh/w=";

        public HomeController()
        {
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public ActionResult Index()
        {
            return View();
        }

        //TODO: Move this to a Web API method and call asyncronously
        public async Task<string> SendMessageToDevice()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Ha ha ha."));
            await serviceClient.SendAsync("scarefloor", commandMessage);
            return "You sent a message.";
        }

        //TODO: Move this to a Web API method and call asyncronously
        public async Task<string> ResetDevice()
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Reset."));
            await serviceClient.SendAsync("scarefloor", commandMessage);
            return "You told the device to reset.";
        }
    }
}