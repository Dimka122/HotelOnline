using System.Threading.Tasks;
using HotelBooking.Application.DTOs;

namespace HotelBooking.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(BookingDto booking, string userEmail);
        Task SendBookingCancellationAsync(BookingDto booking, string userEmail);
        Task SendBookingReminderAsync(BookingDto booking, string userEmail);
    }
}
