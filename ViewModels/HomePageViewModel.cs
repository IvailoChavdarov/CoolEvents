namespace CoolEvents.ViewModels
{
    public class HomePageViewModel
    {
        public bool HasAccessToStats { get; set; } = false;
        public int UsersCount { get; set; }
        public int TicketsCount { get; set; }
        public int EventsCount { get; set; }
    }
}
