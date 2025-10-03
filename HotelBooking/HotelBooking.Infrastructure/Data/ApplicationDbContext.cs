using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Hotel>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
                entity.Property(h => h.Address).IsRequired().HasMaxLength(200);
                entity.HasMany(h => h.Rooms)
                      .WithOne(r => r.Hotel)
                      .HasForeignKey(r => r.HotelId);
            });

            builder.Entity<Room>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Number).IsRequired().HasMaxLength(10);
                entity.Property(r => r.PricePerNight).HasColumnType("decimal(18,2)");
                entity.HasMany(r => r.Bookings)
                      .WithOne(b => b.Room)
                      .HasForeignKey(b => b.RoomId);
            });

            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(b => b.Status).HasMaxLength(20);

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Bookings)
                      .HasForeignKey(b => b.UserId);
            });
        }
    }
}
