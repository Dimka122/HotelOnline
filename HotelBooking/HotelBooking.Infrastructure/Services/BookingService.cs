using Microsoft.EntityFrameworkCore;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingDto> CreateBookingAsync(string userId, int roomId, DateTime checkIn, DateTime checkOut)
        {
            // Check if room exists and is available
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                throw new ArgumentException("Room not found");

            // Check if room is available for the selected dates
            var isAvailable = await IsRoomAvailableAsync(roomId, checkIn, checkOut);
            if (!isAvailable)
                throw new InvalidOperationException("Room is not available for the selected dates");

            // Calculate total price
            var nights = (checkOut - checkIn).Days;
            var totalPrice = room.PricePerNight * nights;

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                TotalPrice = totalPrice,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomNumber = room.Number,
                HotelName = room.Hotel.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room.Number,
                    HotelName = b.Room.Hotel.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return bookings;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room.Number,
                    HotelName = b.Room.Hotel.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UserEmail = b.User.Email // Добавим email пользователя для админки
                })
                .ToListAsync();

            return bookings;
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<BookingDto> GetBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return null;

            return new BookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomNumber = booking.Room.Number,
                HotelName = booking.Room.Hotel.Name,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };
        }

        private async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var overlappingBookings = await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                           b.Status == "Confirmed" &&
                           ((checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                            (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                            (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate)))
                .CountAsync();

            return overlappingBookings == 0;
        }
    }
}
