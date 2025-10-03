using HotelBooking.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Interfaces
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelDto>> GetHotelsAsync();
        Task<HotelDto> GetHotelByIdAsync(int id);
        Task<IEnumerable<RoomDto>> SearchRoomsAsync(string city, DateTime checkIn, DateTime checkOut, int guests);
        Task<RoomDto> GetRoomByIdAsync(int roomId);
        Task CreateHotelAsync(HotelDto hotelDto);
        Task UpdateHotelAsync(HotelDto hotelDto);
        Task DeleteHotelAsync(int id);
    }
}
