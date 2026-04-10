using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;

namespace HotelBooking.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly string? _telegramBotToken;
        private readonly string? _telegramChatId;
        private readonly bool _isTelegramConfigured;

        public EmailService()
        {
            _telegramBotToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
            _telegramChatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
            _isTelegramConfigured = !string.IsNullOrEmpty(_telegramBotToken) && !string.IsNullOrEmpty(_telegramChatId);
        }

        public async Task SendBookingConfirmationAsync(BookingDto booking, string userEmail)
        {
            var message = $@"
🏨 *Новое бронирование!*

👤 *Гость:* {booking.UserName}
📧 *Email:* {userEmail}
📅 *Заезд:* {booking.CheckInDate:dd.MM.yyyy}
📅 *Выезд:* {booking.CheckOutDate:dd.MM.yyyy}
💰 *Сумма:* {booking.TotalPrice} грн
🛏️ *Номер:* {booking.RoomNumber}
🏨 *Отель:* {booking.HotelName}
            ".Trim();

            await SendToTelegramAsync(message);
        }

        public async Task SendBookingCancellationAsync(BookingDto booking, string userEmail)
        {
            var message = $@"
❌ *Бронирование отменено!*

👤 *Гость:* {booking.UserName}
📧 *Email:* {userEmail}
📅 *Заезд:* {booking.CheckInDate:dd.MM.yyyy}
📅 *Выезд:* {booking.CheckOutDate:dd.MM.yyyy}
🛏️ *Номер:* {booking.RoomNumber}
🏨 *Отель:* {booking.HotelName}
            ".Trim();

            await SendToTelegramAsync(message);
        }

        public async Task SendBookingReminderAsync(BookingDto booking, string userEmail)
        {
            var message = $@"
🔔 *Напоминание о бронировании!*

👤 *Гость:* {booking.UserName}
📅 *Заезд:* {booking.CheckInDate:dd.MM.yyyy}
📅 *Выезд:* {booking.CheckOutDate:dd.MM.yyyy}
🛏️ *Номер:* {booking.RoomNumber}
🏨 *Отель:* {booking.HotelName}
            ".Trim();

            await SendToTelegramAsync(message);
        }

        private async Task SendToTelegramAsync(string message)
        {
            if (!_isTelegramConfigured)
            {
                Console.WriteLine("Telegram not configured. Message: " + message);
                return;
            }

            try
            {
                var url = $"https://api.telegram.org/bot{_telegramBotToken}/sendMessage";

                var payload = new
                {
                    chat_id = _telegramChatId,
                    text = message,
                    parse_mode = "Markdown"
                };

                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Telegram error: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Telegram exception: {ex.Message}");
            }
        }
    }
}

