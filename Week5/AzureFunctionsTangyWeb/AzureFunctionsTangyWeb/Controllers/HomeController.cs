using AzureFunctionsTangyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace AzureFunctionsTangyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory; //added

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory; //added
        }

        public IActionResult Index()
        {
            return View();
        }
        //added 
        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest)
        {
            salesRequest.Id = Guid.NewGuid().ToString(); //added 
            using var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("  http://localhost:7051/api/");
            client.BaseAddress = new Uri(" https://tangyfunc-c8cyd3dke3a9evfj.southeastasia-01.azurewebsites.net/api/OnSalesUploadWriteToQueue");
            using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await client.PostAsync("OnSalesUploadWriteToQueue", content);
                string returnValue = await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
