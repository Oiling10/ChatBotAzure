using chatbot.Data;
using chatbot.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using chatbot.Services;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace chatbot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzCognitiveIntegrator _azCognitiveIntegrator;

        public HomeController(ILogger<HomeController> logger, IAzCognitiveIntegrator azCognitiveIntegrator)
        {
            _logger = logger;
            _azCognitiveIntegrator = azCognitiveIntegrator;
        }

        public async Task<string> Index(string pregunta)
        {
            string response = await _azCognitiveIntegrator.ObtenerRespuestaAsync(pregunta);
            return response;
        }

        public IActionResult Home()
        {
            return View("Index");
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
