using CoolEvents.Data;
using CoolEvents.Models;
using CoolEvents.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolEvents.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public TicketsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            string currentUserId = await _userManager.GetUserIdAsync(currentUser);

            List<Ticket> userTickets = await _db.Tickets
                .Where(x=>x.UserId == currentUserId)
                .Include(x => x.Event)
                .ToListAsync();

            return View(userTickets);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> All()
        {
            List<Ticket> userTickets = await _db.Tickets
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();

            return View(userTickets);
        }

        [HttpPost]
        public async Task<IActionResult> GetTicket(int eventId)
        {
            Ticket newTicket = new Ticket();
            newTicket.EventId = eventId;

            AppUser currentUser = await _userManager.GetUserAsync(User);
            newTicket.UserId = await _userManager.GetUserIdAsync(currentUser);

            await _db.Tickets.AddAsync(newTicket);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTicket(int eventId)
        {
            AppUser currentUser = await _userManager.GetUserAsync(User);
            string currentUserId = await _userManager.GetUserIdAsync(currentUser);

            Ticket ticketToRemove = await _db.Tickets
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.UserId == currentUserId);

            if (ticketToRemove == null)
            {
                return NotFound();
            }

            _db.Tickets.Remove(ticketToRemove);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id) {
            EditTicketViewModel data = new EditTicketViewModel();

            Ticket ticket = await _db.Tickets
                .Include(x=>x.User)
                .Include(x=>x.Event)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null) {
                return NotFound();
            }

            data.Events = await _db.Events.ToListAsync();
            data.Users = await _userManager.Users.ToListAsync();
            data.Ticket = ticket;

            return View(data);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditTicketViewModel ticketData)
        {
            if(ticketData.Ticket== null)
            {
                return NotFound();
            }

            _db.Tickets.Update(ticketData.Ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction("All");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            CreateTicketViewModel data = new CreateTicketViewModel();
            data.Events = await _db.Events.ToListAsync();
            data.Users = await _userManager.Users.ToListAsync();
            data.Ticket = new Ticket();

            return View(data);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketViewModel ticketData)
        {
            if (ticketData.Ticket == null)
            {
                return NotFound();
            }

            await _db.Tickets.AddAsync(ticketData.Ticket);
            await _db.SaveChangesAsync();

            return RedirectToAction("All");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            Ticket ticketToDelete = await _db.Tickets
                .FirstOrDefaultAsync(x => x.Id == id);

            if (ticketToDelete == null)
            {
                return NotFound();
            }

            _db.Tickets.Remove(ticketToDelete);
            await _db.SaveChangesAsync();

            return RedirectToAction("All");
        }
    }
}
