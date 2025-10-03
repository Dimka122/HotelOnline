namespace HotelBooking.Web.Helpers
{
    public static class CurrencyHelper
    {
        public static string FormatDollars(decimal amount)
        {
            return $"${amount}";
        }

        public static string FormatDollars(decimal amount, int decimals)
        {
            return $"${Math.Round(amount, decimals)}";
        }
    }
}
