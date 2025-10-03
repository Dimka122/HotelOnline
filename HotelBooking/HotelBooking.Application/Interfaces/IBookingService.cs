using HotelBooking.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(string userId, int roomId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task CancelBookingAsync(int bookingId);
    }
}
