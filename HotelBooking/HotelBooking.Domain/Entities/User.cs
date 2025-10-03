using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}