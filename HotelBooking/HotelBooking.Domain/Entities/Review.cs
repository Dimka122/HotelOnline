using System;

namespace HotelBooking.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }           // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Hotel Hotel { get; set; }
        public virtual User User { get; set; }
    }
}
