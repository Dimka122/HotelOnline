using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles if they don't exist
            if (!await roleManager.RoleExistsAsync(Roles.Administrator))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Administrator));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Client))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Client));
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@hotelbooking.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Administrator);
                }
            }

            // Create test client user
            var clientEmail = "client@test.com";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);

            if (clientUser == null)
            {
                clientUser = new User
                {
                    UserName = clientEmail,
                    Email = clientEmail,
                    FirstName = "John",
                    LastName = "Client",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(clientUser, "Client123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(clientUser, Roles.Client);
                }
            }

            // Seed sample hotels and rooms if they don't exist
            if (!context.Hotels.Any())
            {
                var hotels = new List<Hotel>
                {
                    // Обновите адреса отелей
new Hotel
{
    Name = "Grand Hotel",
    Address = "New York, 5th Avenue, 123",
    Description = "Luxury hotel in the city center",
    ImageUrl = "https://www.menslife.com/upload/iblock/4f1/marina_bay_sands_hotel_casino_skypark_singapore_from_waterfront_esplanade.jpg",
    Rooms = new List<Room>
    {
        new Room { Number = "101", PricePerNight = 150, Capacity = 2, Description = "Standard Room with city view", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Room+101" },
        new Room { Number = "102", PricePerNight = 220, Capacity = 3, Description = "Deluxe Room with balcony", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Room+102" },
        new Room { Number = "201", PricePerNight = 350, Capacity = 2, Description = "Luxury Suite", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Suite+201" }
    }
},
new Hotel
{
    Name = "Business Hotel",
    Address = "Chicago, Michigan Avenue, 456",
    Description = "Comfortable hotel for business travelers",
    ImageUrl = "https://via.placeholder.com/400x250/10B981/FFFFFF?text=Business+Hotel",
    Rooms = new List<Room>
    {
        new Room { Number = "101", PricePerNight = 100, Capacity = 1, Description = "Single Room for business travelers", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+101" },
        new Room { Number = "102", PricePerNight = 130, Capacity = 2, Description = "Double Room with workspace", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+102" },
        new Room { Number = "103", PricePerNight = 180, Capacity = 2, Description = "Executive Room", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+103" }
    }
},
new Hotel
{
    Name = "Seaside Resort",
    Address = "Miami, Ocean Drive, 789",
    Description = "Beautiful resort by the sea",
    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRtamRgMUwfshF0lNyeOf7r2SeoLpWfPoLRrA&s",
    Rooms = new List<Room>
    {
        new Room { Number = "101", PricePerNight = 240, Capacity = 4, Description = "Family Room with sea view", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Room+101" },
        new Room { Number = "201", PricePerNight = 450, Capacity = 2, Description = "Premium Suite with jacuzzi", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Suite+201" },
        new Room { Number = "301", PricePerNight = 600, Capacity = 3, Description = "Presidential Suite", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Presidential+Suite" }
    }
},
new Hotel
{
    Name = "Mountain View Lodge",
    Address = "Denver, Mountain Road, 321",
    Description = "Cozy lodge with mountain views",
    ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTjOjAAHxzzpJuvlc8ASF8xmt-nuiyaCY0FYg&s",
    Rooms = new List<Room>
    {
        new Room { Number = "101", PricePerNight = 120, Capacity = 2, Description = "Standard Mountain View Room", ImageUrl = "https://via.placeholder.com/300x200/A78BFA/FFFFFF?text=Room+101" },
        new Room { Number = "201", PricePerNight = 200, Capacity = 4, Description = "Family Suite with fireplace", ImageUrl = "https://via.placeholder.com/300x200/A78BFA/FFFFFF?text=Suite+201" }
    }
}
                };

                await context.Hotels.AddRangeAsync(hotels);
                await context.SaveChangesAsync();
            }

            // Create sample bookings for testing
            if (!context.Bookings.Any() && clientUser != null)
            {
                var room = await context.Rooms.FirstAsync();
                var booking = new Booking
                {
                    UserId = clientUser.Id,
                    RoomId = room.Id,
                    CheckInDate = DateTime.Now.AddDays(7),
                    CheckOutDate = DateTime.Now.AddDays(10),
                    TotalPrice = room.PricePerNight * 3,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow
                };

                await context.Bookings.AddAsync(booking);
                await context.SaveChangesAsync();
            }
        }
    }
}