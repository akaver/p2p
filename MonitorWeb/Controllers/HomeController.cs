using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MonitorWeb.Models;
using Microsoft.AspNetCore.Hosting;

namespace MonitorWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = GetViewModel();
            
            return View("Index", model);
        }

        private HomeViewModel GetViewModel()
        {
            var model = new HomeViewModel();
            List<string> hosts = GetHosts();

            foreach (var host in hosts)
            {
                var endpointData = new EndpointData();
                endpointData.Url = host;

                var hostWithoutSlash = host.TrimEnd('/');
                var hostParts = hostWithoutSlash.Split(":");
                var portPart = hostParts.Last();

                if (int.TryParse(portPart, out int portNumber))
                    endpointData.PortNumber = portNumber;
                
                model.Endpoints.Add(endpointData);
            }

            return model;
        }
        
        private List<string> GetHosts()
        {
            var res = new List<string>();

            var someRootOfSomething = _hostingEnvironment.ContentRootPath;
            var path = someRootOfSomething + "/Data/hosts.txt"; 

            if (System.IO.File.Exists(path))
                res.AddRange(System.IO.File.ReadAllLines(path).Where(p => !string.IsNullOrEmpty(p)));

            return res;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}