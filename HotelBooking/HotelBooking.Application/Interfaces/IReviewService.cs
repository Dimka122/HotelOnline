using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Application.DTOs;

namespace HotelBooking.Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetHotelReviewsAsync(int hotelId);
        Task<IEnumerable<ReviewDto>> GetUserReviewsAsync(string userId);
        Task<ReviewDto> CreateReviewAsync(string userId, int hotelId, int rating, string comment);
        Task<double> GetHotelAverageRatingAsync(int hotelId);
        Task<int> GetHotelReviewCountAsync(int hotelId);
        Task DeleteReviewAsync(int reviewId);
    }
}
