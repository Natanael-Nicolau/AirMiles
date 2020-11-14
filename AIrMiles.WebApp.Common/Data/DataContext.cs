using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        #region Production
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightClass> FlightClasses { get; set; }
        public DbSet<Mile> Miles { get; set; }
        public DbSet<MilesRequest> MilesRequests { get; set; }
        public DbSet<MilesType> MilesTypes { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationType> ReservationTypes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        #endregion

        #region Temporary
        #endregion




        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Transaction>()
                .Property(t => t.Price)
                .HasColumnType("decimal(8,2)");

            builder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasColumnType("decimal(8,2)");

            builder.Entity<Flight>()
                .Property(f => f.BasePrice)
                .HasColumnType("decimal(8,2)");

            builder.Entity<Reservation>()
                .Property(r => r.Price)
                .HasColumnType("decimal(8,2)");

            builder.Entity<FlightClass>()
                .Property(f => f.PriceMultiplier)
                .HasColumnType("numeric(3,2)");

            //Disables Cascade
            var cascadeFKs = builder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
        }
    }
}
