namespace HotelBooking.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 0,      // Ожидает подтверждения
        Confirmed = 1,    // Подтверждён
        Cancelled = 2,   // Отменён
        Completed = 3    // Завершён
    }
}
