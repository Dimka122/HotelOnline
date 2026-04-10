using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Web.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IHotelService _hotelService;
        private readonly UserManager<User> _userManager;

        public ReviewsController(
            IReviewService reviewService,
            IHotelService hotelService,
            UserManager<User> userManager)
        {
            _reviewService = reviewService;
            _hotelService = hotelService;
            _userManager = userManager;
        }

        // GET: /Hotels/5/Reviews
        public async Task<IActionResult> Index(int hotelId)
        {
            var reviews = await _reviewService.GetHotelReviewsAsync(hotelId);
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            
            var avgRating = await _reviewService.GetHotelAverageRatingAsync(hotelId);
            var reviewCount = await _reviewService.GetHotelReviewCountAsync(hotelId);

            ViewBag.Hotel = hotel;
            ViewBag.AverageRating = avgRating;
            ViewBag.ReviewCount = reviewCount;

            return View(reviews);
        }

        // GET: /Reviews/Create/5
        [Authorize]
        public async Task<IActionResult> Create(int hotelId)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
            if (hotel == null)
            {
                return NotFound();
            }

            ViewBag.Hotel = hotel;
            return View();
        }

        // POST: /Reviews/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int hotelId, int selectedRating, string comment)
        {
            try
            {
                if (selectedRating < 1 || selectedRating > 5)
                {
                    ModelState.AddModelError("", "Пожалуйста, выберите оценку от 1 до 5 звезд");
                    var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                    ViewBag.Hotel = hotel;
                    return View();
                }

                var userId = _userManager.GetUserId(User);
                await _reviewService.CreateReviewAsync(userId, hotelId, selectedRating, comment);
                
                TempData["Success"] = "Отзыв успешно добавлен!";
                return RedirectToAction("Index", new { hotelId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                
                var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                ViewBag.Hotel = hotel;
                return View();
            }
        }

        // GET: /Reviews/MyReviews
        [Authorize]
        public async Task<IActionResult> MyReviews()
        {
            var userId = _userManager.GetUserId(User);
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return View(reviews);
        }

        // POST: /Reviews/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            var review = reviews.FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            await _reviewService.DeleteReviewAsync(id);
            TempData["Success"] = "Отзыв удалён";
            
            return RedirectToAction("MyReviews");
        }
    }
}
