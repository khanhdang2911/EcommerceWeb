using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_website.Models;

namespace Ecommerce_website.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Home","Category");
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult NotFound()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Forbidden()
    {
        return View();
    }
}
