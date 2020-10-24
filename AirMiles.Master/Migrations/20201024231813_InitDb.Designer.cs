﻿// <auto-generated />
using System;
using AirMiles.Master.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AirMiles.Master.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20201024231813_InitDb")]
    partial class InitDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Airport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("IATA");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<decimal>("Latitude");

                    b.Property<decimal>("Longitude");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BirtDate");

                    b.Property<int>("BoughtMiles");

                    b.Property<int>("GifterId");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ProlongedMiles");

                    b.Property<int>("RevisionMonth");

                    b.Property<int>("TransferedMiles");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GifterId");

                    b.HasIndex("UserId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.CreditCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BillingAddress");

                    b.Property<string>("CVV");

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.ToTable("CreditCards");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Flight", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BaseMilesPrice");

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(8,2)");

                    b.Property<int>("EndAirportId");

                    b.Property<int>("FlightCompanyId");

                    b.Property<DateTime>("FlightEnd");

                    b.Property<DateTime>("FlightStart");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("StartAirportId");

                    b.HasKey("Id");

                    b.HasIndex("EndAirportId");

                    b.HasIndex("FlightCompanyId");

                    b.HasIndex("StartAirportId");

                    b.ToTable("Flights");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.FlightClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<decimal>("PriceMultiplier")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 38, scale: 17)))
                        .HasColumnType("numeric(3,2)");

                    b.HasKey("Id");

                    b.ToTable("FlightClasses");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Mile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<DateTime>("ExpirationDate");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("MilesTypeId");

                    b.Property<int>("Qtd");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("MilesTypeId");

                    b.ToTable("Miles");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.MilesType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.ToTable("MilesTypes");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsStarAlliance");

                    b.Property<string>("Name");

                    b.Property<string>("WebsiteUrl");

                    b.HasKey("Id");

                    b.ToTable("Partners");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientId");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(8,2)");

                    b.Property<DateTime>("ReservationDate");

                    b.Property<int>("ReservationTypeId");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ReservationTypeId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.ReservationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.HasKey("Id");

                    b.ToTable("ReservationTypes");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName");

                    b.Property<int>("FlightClassId");

                    b.Property<int>("FlightId");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(8,2)");

                    b.Property<int>("ReservationId");

                    b.Property<string>("Seat");

                    b.HasKey("Id");

                    b.HasIndex("FlightClassId");

                    b.HasIndex("FlightId");

                    b.HasIndex("ReservationId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClientID");

                    b.Property<int>("CreditCardId");

                    b.Property<string>("Description");

                    b.Property<bool>("IsAproved");

                    b.Property<bool>("IsCreditCard");

                    b.Property<bool>("IsDeleted");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(8,2)");

                    b.Property<DateTime>("TransactionDate");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ClientID");

                    b.HasIndex("CreditCardId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PhotoUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Client", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.Client", "Gifter")
                        .WithMany()
                        .HasForeignKey("GifterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Flight", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.Airport", "EndAirport")
                        .WithMany()
                        .HasForeignKey("EndAirportId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.Partner", "FlightCompany")
                        .WithMany()
                        .HasForeignKey("FlightCompanyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.Airport", "StartAirport")
                        .WithMany()
                        .HasForeignKey("StartAirportId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Mile", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.MilesType", "MilesType")
                        .WithMany()
                        .HasForeignKey("MilesTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Reservation", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.ReservationType", "ReservationType")
                        .WithMany()
                        .HasForeignKey("ReservationTypeId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Ticket", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.FlightClass", "FlightClass")
                        .WithMany()
                        .HasForeignKey("FlightClassId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.Flight", "Flight")
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.Reservation", "Reservation")
                        .WithMany()
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("AirMiles.Master.Data.Entities.Transaction", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AirMiles.Master.Data.Entities.CreditCard", "CreditCard")
                        .WithMany()
                        .HasForeignKey("CreditCardId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AirMiles.Master.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("AirMiles.Master.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}