using Dapper;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using HotelBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace HotelBooking.Infrastructure.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HotelService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public Task CreateHotelAsync(HotelDto hotelDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteHotelAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HotelDto> GetHotelByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HotelDto>> GetHotelsAsync()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Select(h => new HotelDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    Address = h.Address,
                    Description = h.Description,
                    ImageUrl = h.ImageUrl,
                    RoomCount = h.Rooms.Count,
                    MinPrice = h.Rooms.Any() ? h.Rooms.Min(r => r.PricePerNight) : 0,
                    MaxPrice = h.Rooms.Any() ? h.Rooms.Max(r => r.PricePerNight) : 0
                })
                .ToListAsync();

            return hotels;
        }

        public async Task<IEnumerable<RoomDto>> SearchRoomsAsync(string city, DateTime checkIn, DateTime checkOut, int guests)
        {
            var availableRooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Bookings)
                .Where(r => r.Hotel.Address.Contains(city) &&
                           r.Capacity >= guests &&
                           !r.Bookings.Any(b =>
                               (checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                               (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                               (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate) &&
                               b.Status == "Confirmed"))
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    HotelName = r.Hotel.Name,
                    Number = r.Number,
                    PricePerNight = r.PricePerNight,
                    Capacity = r.Capacity,
                    Description = r.Description,
                    ImageUrl = r.ImageUrl,
                    IsAvailable = true
                })
                .ToListAsync();

            return availableRooms;
        }

        public Task UpdateHotelAsync(HotelDto hotelDto)
        {
            throw new NotImplementedException();
        }

        // Другие методы реализации...
    }
}
