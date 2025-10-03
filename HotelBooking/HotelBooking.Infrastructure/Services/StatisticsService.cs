using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Services
{
    public class StatisticsService
    {
        private readonly IConfiguration _configuration;

        public StatisticsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<object> GetBookingStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sql = @"
                SELECT 
                    COUNT(*) as TotalBookings,
                    SUM(TotalPrice) as TotalRevenue,
                    AVG(TotalPrice) as AverageBookingValue,
                    COUNT(DISTINCT UserId) as UniqueCustomers
                FROM Bookings 
                WHERE CreatedAt BETWEEN @StartDate AND @EndDate 
                AND Status = 'Confirmed'";

            var stats = await connection.QueryFirstAsync(sql, new { StartDate = startDate, EndDate = endDate });
            return stats;
        }
    }
}
