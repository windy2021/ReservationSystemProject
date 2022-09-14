using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ReservationSystemProject.Areas.Admin.Models.Reservation;

namespace ReservationSystemProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<ReservationType> ReservationTypes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<TableReservation> TableReservations { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Person>().Property(p => p.FullName).IsRequired();
            builder.Entity<Person>().Property(p => p.Email).IsRequired();
            builder.Entity<Person>().Property(p => p.MobileNumber).IsRequired();

            //Data Seeding
            builder.Entity<Restaurant>().HasData(new Restaurant { Id = 1, Email = "resturant@email.com", Name = "Resturant", ABN = 1234567890, Address = "42 Baloney Boulevard, Meatballia 2034", PhoneNumber = "0404 323 454" });

            builder.Entity<Area>().HasData(
                new Area { Id = 1, Name = "Main", RestaurantId = 1 },
                new Area { Id = 2, Name = "Outside", RestaurantId = 1 },
                new Area { Id = 3, Name = "Balcony", RestaurantId = 1 }
                );

            Table[] tables = new Table[30];
            for (int i = 1; i <= 10; i++)
            {
                tables[i - 1] = new Table { Id = i, Name = $"M{i - 1}", AreaId = 1 };
            }
            for (int i = 1; i <= 10; i++)
            {
                tables[i + 9] = new Table { Id = i + 10, Name = $"O{i - 1}", AreaId = 2 };
            }
            for (int i = 1; i <= 10; i++)
            {
                tables[i + 19] = new Table { Id = i + 20, Name = $"B{i - 1}", AreaId = 3 };
            }
            builder.Entity<Table>().HasData(tables);


            builder.Entity<Sitting>().HasData(
                new Sitting { Id = 1, Name = "Breakfast", Start = new DateTime(2021, 10, 7, 7, 0, 0), End = new DateTime(2021, 10, 7, 11, 0, 0), Capacity = 30, RestaurantId = 1 },
                new Sitting { Id = 2, Name = "Lunch", Start = new DateTime(2021, 10, 7, 12, 0, 0), End = new DateTime(2021, 10, 7, 16, 0, 0), Capacity = 30, RestaurantId = 1 },
                new Sitting { Id = 3, Name = "Dinner", Start = new DateTime(2021, 10, 7, 18, 0, 0), End = new DateTime(2021, 10, 7, 23, 0, 0), Capacity = 30, RestaurantId = 1 }
            );

            builder.Entity<ReservationType>().HasData(
                new ReservationType { Id = 1, Name = "Online" },
                new ReservationType { Id = 2, Name = "InPerson" },
                new ReservationType { Id = 3, Name = "Email" },
                new ReservationType { Id = 4, Name = "Phone" }
            );

            builder.Entity<ReservationStatus>().HasData(
                new ReservationStatus { Id = 1, Name = "Pending" },
                new ReservationStatus { Id = 2, Name = "Confirmed" },
                new ReservationStatus { Id = 3, Name = "Seated" },
                new ReservationStatus { Id = 4, Name = "Completed" },
                new ReservationStatus { Id = 5, Name = "Cancelled" }
            );  

            builder.Entity<Person>().HasData(
                new Person { Id = 1, FullName = "Member One", Email = "m1@email.com", MobileNumber = "0404 252 343" },
                new Person { Id = 2, FullName = "Member Two", Email = "m2@email.com", MobileNumber = "0404 252 343" },
                new Person { Id = 3, FullName = "Member Three", Email = "m3@email.com", MobileNumber = "0404 252 343" }
            );

            builder.Entity<Reservation>().HasData(
                new Reservation { Id = 1, Start = new DateTime(2021, 10, 7, 7, 0, 0), Duration = 60, Guests = 1, PersonId = 1, SittingId = 1, ReservationTypeId = 1, ReservationStatusId = 1 },
                new Reservation { Id = 2, Start = new DateTime(2021, 10, 7, 9, 0, 0), Duration = 60, Guests = 1, PersonId = 1, SittingId = 1, ReservationTypeId = 1, ReservationStatusId = 2 },
                new Reservation { Id = 3, Start = new DateTime(2021, 10, 7, 12, 0, 0), Duration = 60, Guests = 1, PersonId = 2, SittingId = 2, ReservationTypeId = 1, ReservationStatusId = 1 },
                new Reservation { Id = 4, Start = new DateTime(2021, 10, 7, 14, 0, 0), Duration = 60, Guests = 1, PersonId = 2, SittingId = 2, ReservationTypeId = 1, ReservationStatusId = 3 },
                new Reservation { Id = 5, Start = new DateTime(2021, 10, 7, 18, 0, 0), Duration = 60, Guests = 1, PersonId = 3, SittingId = 3, ReservationTypeId = 1, ReservationStatusId = 1 },
                new Reservation { Id = 6, Start = new DateTime(2021, 10, 7, 20, 0, 0), Duration = 60, Guests = 1, PersonId = 3, SittingId = 3, ReservationTypeId = 1, ReservationStatusId = 4 }
            );

            TableReservation[] tableres = new TableReservation[6];
            for (int i = 1; i <= tableres.Length; i++)
            {
                tableres[i - 1] = new TableReservation { Id = i, ReservationId = i, TableId = i };
            }
            builder.Entity<TableReservation>().HasData(tableres);
        }
        public DbSet<ReservationSystemProject.Areas.Admin.Models.Reservation.Details> Details { get; set; }
    }
}
