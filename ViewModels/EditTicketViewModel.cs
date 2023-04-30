using CoolEvents.Models;

namespace CoolEvents.ViewModels
{
    public class EditTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public List<AppUser> Users { get; set; }
        public List<Event> Events { get; set; }
    }
}
