using Microsoft.EntityFrameworkCore;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Data;

namespace HotelBooking.Infrastructure.Services
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;

        public HotelService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<HotelDto> GetHotelByIdAsync(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hotel == null)
                return null;

            return new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                Description = hotel.Description,
                ImageUrl = hotel.ImageUrl,
                RoomCount = hotel.Rooms.Count,
                MinPrice = hotel.Rooms.Any() ? hotel.Rooms.Min(r => r.PricePerNight) : 0,
                MaxPrice = hotel.Rooms.Any() ? hotel.Rooms.Max(r => r.PricePerNight) : 0
            };
        }

        public async Task<IEnumerable<RoomDto>> SearchRoomsAsync(string city, DateTime checkIn, DateTime checkOut, int guests)
        {
            var availableRooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.Bookings)
                .Where(r => r.Hotel.Address.Contains(city) &&
                           r.Capacity >= guests &&
                           !r.Bookings.Any(b =>
                               b.Status == "Confirmed" &&
                               ((checkIn >= b.CheckInDate && checkIn < b.CheckOutDate) ||
                                (checkOut > b.CheckInDate && checkOut <= b.CheckOutDate) ||
                                (checkIn <= b.CheckInDate && checkOut >= b.CheckOutDate))))
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

        public async Task CreateHotelAsync(HotelDto hotelDto)
        {
            var hotel = new Hotel
            {
                Name = hotelDto.Name,
                Address = hotelDto.Address,
                Description = hotelDto.Description,
                ImageUrl = hotelDto.ImageUrl
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(HotelDto hotelDto)
        {
            var hotel = await _context.Hotels.FindAsync(hotelDto.Id);
            if (hotel != null)
            {
                hotel.Name = hotelDto.Name;
                hotel.Address = hotelDto.Address;
                hotel.Description = hotelDto.Description;
                hotel.ImageUrl = hotelDto.ImageUrl;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RoomDto> GetRoomByIdAsync(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                return null;

            return new RoomDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                HotelName = room.Hotel.Name,
                Number = room.Number,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                Description = room.Description,
                ImageUrl = room.ImageUrl
            };
        }
    }
}