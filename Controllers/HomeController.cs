using CoolEvents.Data;
using CoolEvents.Models;
using CoolEvents.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoolEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            HomePageViewModel data = new HomePageViewModel();
            data.HasAccessToStats = false;
            if (User.Identity.IsAuthenticated)
            {
                AppUser currentUser = await _userManager.GetUserAsync(User);
                if(await _userManager.IsInRoleAsync(currentUser, "Administrator"))
                {
                    data.HasAccessToStats = true;
                }
                data.EventsCount = _db.Events.Count();
                data.UsersCount = _userManager.Users.Count();
                data.TicketsCount = _db.Tickets.Count();
            }
            return View(data);
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