using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HotelBooking.Application.Interfaces;
using HotelBooking.Application.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Web.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IHotelService _hotelService;
        private readonly UserManager<User> _userManager;

        public BookingsController(
            IBookingService bookingService,
            IHotelService hotelService,
            UserManager<User> userManager)
        {
            _bookingService = bookingService;
            _hotelService = hotelService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Create(int roomId)
        {
            var room = await _hotelService.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                return NotFound();
            }

            var model = new CreateBookingDto
            {
                RoomId = roomId
            };

            ViewBag.Room = room;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateBookingDto bookingDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    var booking = await _bookingService.CreateBookingAsync(
                        userId,
                        bookingDto.RoomId,
                        bookingDto.CheckInDate,
                        bookingDto.CheckOutDate);

                    return RedirectToAction("MyBookings");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Если ошибка, перезагружаем страницу с данными
            var room = await _hotelService.GetRoomByIdAsync(bookingDto.RoomId);
            ViewBag.Room = room;
            return View(bookingDto);
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }

        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> AllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return View(bookings);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction("MyBookings");
        }
    }
}