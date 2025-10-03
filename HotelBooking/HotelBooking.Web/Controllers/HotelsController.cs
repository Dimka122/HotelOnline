using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Web.Controllers
{
    public class HotelsController : Controller
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        public async Task<IActionResult> Index()
        {
            var hotels = await _hotelService.GetHotelsAsync();
            return View(hotels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string city, DateTime checkIn, DateTime checkOut, int guests)
        {
            var rooms = await _hotelService.SearchRoomsAsync(city, checkIn, checkOut, guests);
            return View("SearchResults", rooms);
        }

        public async Task<IActionResult> Details(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [Authorize(Roles = Roles.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(HotelDto hotelDto)
        {
            if (ModelState.IsValid)
            {
                await _hotelService.CreateHotelAsync(hotelDto);
                return RedirectToAction(nameof(Index));
            }
            return View(hotelDto);
        }
    }
}