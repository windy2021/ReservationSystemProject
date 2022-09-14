using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using ReservationSystemProject.Models;
using System.Diagnostics;

namespace ReservationSystemProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            _logger.LogInformation("Entering the HomeController using the Index() method.");
            _logger.LogError(ViewResultExecutor.DefaultContentType.ToString());
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Page does not exist!");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult RedirectUser()
        {
            _logger.LogInformation("Entering the HomeController using the RedirectUser() method.");
            if (User.IsInRole("Manager") || User.IsInRole("Staff"))
            {
                _logger.LogInformation("Redirecting Manager and Staff to Admin area.");
                return RedirectToAction("Index", "Reservation", new { area = "Admin" });
            }
            else if (User.IsInRole("Member"))
            {
                _logger.LogInformation("Redirecting Member to the Member area.");
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            else
            {
                return RedirectToPage("Register");
            }
        }
    }
}
