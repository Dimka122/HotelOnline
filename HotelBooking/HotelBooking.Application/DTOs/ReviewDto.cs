namespace HotelBooking.Application.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        public int HotelId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
