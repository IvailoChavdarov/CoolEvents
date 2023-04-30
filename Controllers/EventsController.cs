using CoolEvents.Data;
using CoolEvents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CoolEvents.Controllers
{
    public class EventsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public EventsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index(string? search)
        {
            List<Event> eventsData;
            if (!string.IsNullOrEmpty(search))
            {
                eventsData = await _db.Events.Where(x=>x.Name.Contains(search)).ToListAsync();
            }
            else
            {
                eventsData = await _db.Events.ToListAsync();
            }
            ViewBag.IsAdmin = false;
            if (User.Identity.IsAuthenticated)
            {
                AppUser currentUser = await _userManager.GetUserAsync(User);
                ViewBag.IsAdmin = await _userManager.IsInRoleAsync(currentUser, "Administrator");
                for (int i = 0; i < eventsData.Count; i++)
                {
                    eventsData[i].HasTicket = await _db.Tickets
                        .AnyAsync(x => x.UserId == currentUser.Id && x.EventId == eventsData[i].Id);
                }
            }

            return View(eventsData);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(Event eventToAdd)
        {
            eventToAdd.Tickets = new List<Ticket>();
            if (ModelState.IsValid) {
                await _db.Events.AddAsync(eventToAdd);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(eventToAdd);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(int id)
        {
            Event eventToEdit = _db.Events.FirstOrDefault(e => e.Id == id);

            if (eventToEdit == null)
            {
                return NotFound();
            }

            return View(eventToEdit);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(Event eventToEdit)
        {
            if (ModelState.IsValid)
            {
                _db.Events.Update(eventToEdit);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(eventToEdit);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Event eventToDelete = _db.Events.FirstOrDefault(e => e.Id == id);
            if (eventToDelete==null)
            {
                return NotFound();
            }
            _db.Events.Remove(eventToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
