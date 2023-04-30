using Microsoft.AspNetCore.Identity;

namespace CoolEvents.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
