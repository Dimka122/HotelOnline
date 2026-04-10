using Microsoft.EntityFrameworkCore;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetHotelReviewsAsync(int hotelId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Hotel)
                .Where(r => r.HotelId == hotelId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = r.Hotel.Name,
                    UserId = r.UserId,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(string userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Hotel)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = r.Hotel.Name,
                    UserId = r.UserId,
                    UserName = r.User.FirstName + " " + r.User.LastName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }

        public async Task<ReviewDto> CreateReviewAsync(string userId, int hotelId, int rating, string comment)
        {
            // Проверяем, бронировал ли пользователь этот отель
            var hasBooking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .AnyAsync(b => b.UserId == userId && 
                              b.Room.HotelId == hotelId && 
                              b.Status == Domain.Enums.BookingStatus.Completed);

            if (!hasBooking)
            {
                throw new InvalidOperationException("Вы можете оставлять отзывы только на отели, в которых останавливались.");
            }

            // Проверяем, не оставлял ли уже отзыв
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.HotelId == hotelId);

            if (existingReview != null)
            {
                throw new InvalidOperationException("Вы уже оставляли отзыв на этот отель.");
            }

            var review = new Review
            {
                UserId = userId,
                HotelId = hotelId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Загружаем связанные данные для возврата
            await _context.Entry(review).Reference(r => r.User).LoadAsync();
            await _context.Entry(review).Reference(r => r.Hotel).LoadAsync();

            return new ReviewDto
            {
                Id = review.Id,
                HotelId = review.HotelId,
                HotelName = review.Hotel.Name,
                UserId = review.UserId,
                UserName = review.User.FirstName + " " + review.User.LastName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<double> GetHotelAverageRatingAsync(int hotelId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<int> GetHotelReviewCountAsync(int hotelId)
        {
            return await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .CountAsync();
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
