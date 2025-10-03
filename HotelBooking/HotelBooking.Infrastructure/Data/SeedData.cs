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
                    new Hotel
                    {
                        Name = "Grand Hotel",
                        Address = "Madrid, Trastee st. 1",
                        Description = "Luxury hotel in the city center",
                        ImageUrl = "https://via.placeholder.com/400x250/3B82F6/FFFFFF?text=Grand+Hotel",
                        Rooms = new List<Room>
                        {
                            new Room { Number = "101", PricePerNight = 500, Capacity = 2, Description = "Standard Room with city view", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Room+101" },
                            new Room { Number = "102", PricePerNight = 750, Capacity = 3, Description = "Deluxe Room with balcony", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Room+102" },
                            new Room { Number = "201", PricePerNight = 1200, Capacity = 2, Description = "Luxury Suite", ImageUrl = "https://via.placeholder.com/300x200/60A5FA/FFFFFF?text=Suite+201" }
                        }
                    },
                    new Hotel
                    {
                        Name = "Business Hotel",
                        Address = "New York, Brayton Beach 25",
                        Description = "Comfortable hotel for business travelers",
                        ImageUrl = "https://via.placeholder.com/400x250/10B981/FFFFFF?text=Business+Hotel",
                        Rooms = new List<Room>
                        {
                            new Room { Number = "101", PricePerNight = 3500, Capacity = 1, Description = "Single Room for business travelers", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+101" },
                            new Room { Number = "102", PricePerNight = 4500, Capacity = 2, Description = "Double Room with workspace", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+102" },
                            new Room { Number = "103", PricePerNight = 6000, Capacity = 2, Description = "Executive Room", ImageUrl = "https://via.placeholder.com/300x200/34D399/FFFFFF?text=Room+103" }
                        }
                    },
                    new Hotel
                    {
                        Name = "Seaside Resort",
                        Address = "Gavai, Kurort avenyu 50",
                        Description = "Beautiful resort by the sea",
                        ImageUrl = "https://via.placeholder.com/400x250/F59E0B/FFFFFF?text=Seaside+Resort",
                        Rooms = new List<Room>
                        {
                            new Room { Number = "101", PricePerNight = 8000, Capacity = 4, Description = "Family Room with sea view", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Room+101" },
                            new Room { Number = "201", PricePerNight = 15000, Capacity = 2, Description = "Premium Suite with jacuzzi", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Suite+201" },
                            new Room { Number = "301", PricePerNight = 20000, Capacity = 3, Description = "Presidential Suite", ImageUrl = "https://via.placeholder.com/300x200/FBBF24/FFFFFF?text=Presidential+Suite" }
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