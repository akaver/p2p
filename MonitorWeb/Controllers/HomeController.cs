using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MonitorWeb.Models;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace MonitorWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(IHostingEnvironment hostingEnvironment, IHttpClientFactory clientFactory)
        {
            _hostingEnvironment = hostingEnvironment;
            _clientFactory = clientFactory;
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

/*        private IActionResult PollEndpoint(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = _clientFactory.CreateClient();
            var response = client.SendAsync(request);

            if (response.IsCompletedSuccessfully)
            {
                
            }
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}