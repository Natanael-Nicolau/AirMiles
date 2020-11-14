using AIrMiles.WebApp.Common.Data;
using AIrMiles.WebApp.Common.Data.Entities;
using AIrMiles.WebApp.Common.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data
{
    public class DataSeed
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;

        public DataSeed(DataContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();


            await _userRepository.CheckRoleAsync("Admin");
            await _userRepository.CheckRoleAsync("SuperEmployee");
            await _userRepository.CheckRoleAsync("Employee");
            await _userRepository.CheckRoleAsync("Client");
            await _userRepository.CheckRoleAsync("Basic");
            await _userRepository.CheckRoleAsync("Silver");
            await _userRepository.CheckRoleAsync("Gold");

            var user = await _userRepository.GetUserByEmailAsync("milhas@yopmail.com");
            if (user == null)
            {
                #region Seed Admin
                user = new User
                {
                    FirstName = "Miles",
                    LastName = "Junior",
                    Email = "milhas@yopmail.com",
                    UserName = "milhas@yopmail.com",
                    PhoneNumber = "223232323",
                    BirthDate = new DateTime(1995, 4, 25),
                    PhotoUrl = $"~/images/Users/Default_User_Image.png"
                };

                var result = await _userRepository.AddUserAsync(user, "123123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the admin in seeder.");
                }


                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                await _userRepository.ConfirmEmailAsync(user, token);


                var isInRole = await _userRepository.IsUserInRoleAsync(user, "Admin");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "Admin");
                }
                #endregion

                #region Seed Employee
                user = new User
                {
                    FirstName = "John",
                    LastName = "Worker",
                    Email = "employee@yopmail.com",
                    UserName = "employee@yopmail.com",
                    PhoneNumber = "223232323",
                    BirthDate = new DateTime(1995, 4, 25),
                    PhotoUrl = $"~/images/Users/Default_User_Image.png"
                };

                result = await _userRepository.AddUserAsync(user, "123123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
                }


                token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                await _userRepository.ConfirmEmailAsync(user, token);


                isInRole = await _userRepository.IsUserInRoleAsync(user, "Employee");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "Employee");
                }
                #endregion

                #region Seed SuperEmployee
                user = new User
                {
                    FirstName = "Karen",
                    LastName = "Smith",
                    Email = "superemployee@yopmail.com",
                    UserName = "superemployee@yopmail.com",
                    PhoneNumber = "223232323",
                    BirthDate = new DateTime(1995, 4, 25),
                    PhotoUrl = $"~/images/Users/Default_User_Image.png"
                };

                result = await _userRepository.AddUserAsync(user, "123123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the Super Employee in seeder.");
                }


                token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                await _userRepository.ConfirmEmailAsync(user, token);


                isInRole = await _userRepository.IsUserInRoleAsync(user, "SuperEmployee");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "SuperEmployee");
                }
                #endregion

                #region Seed Client Basic
                user = new User
                {
                    FirstName = "Susan",
                    LastName = "Wolf",
                    Email = "client@yopmail.com",
                    UserName = "client@yopmail.com",
                    PhoneNumber = "223232323",
                    BirthDate = new DateTime(1995, 4, 25),
                    PhotoUrl = $"~/images/Users/Default_User_Image.png"
                };

                result = await _userRepository.AddUserAsync(user, "123123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the Client in seeder.");
                }


                token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                await _userRepository.ConfirmEmailAsync(user, token);


                isInRole = await _userRepository.IsUserInRoleAsync(user, "Client");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "Client");
                }
                isInRole = await _userRepository.IsUserInRoleAsync(user, "Basic");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "Basic");
                }
                #endregion



            }

            user = await _userRepository.GetUserByEmailAsync("client@yopmail.com");
            if (_context.Clients.Count() == 0 && user != null)
            {
                _context.Clients.Add(new Client { UserId = user.Id, BoughtMiles = 0, IsAproved = true, IsDeleted = false, ProlongedMiles = 0, RevisionMonth = DateTime.Now.Month, TransferedMiles = 0 });
                await _context.SaveChangesAsync();
            }
            if (_context.MilesTypes.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "Status", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Bonus", IsAproved = true, IsDeleted = false });
                await _context.SaveChangesAsync();
            }
            if (_context.ReservationTypes.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "1Way", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "2Ways", IsAproved = true, IsDeleted = false });
                await _context.SaveChangesAsync();
            }
            if (_context.FlightClasses.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "Discount", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Basic", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Classic", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Plus", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Executive", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Top Executive", IsAproved = true, IsDeleted = false });
                await _context.SaveChangesAsync();
            }
            if (_context.Partners.Count() == 0)
            {
                _context.Partners.Add(new Partner { CreationDate = DateTime.Now, IsStarAlliance = true, Name = "Air Miles", IsAproved = true });
                _context.Partners.Add(new Partner { CreationDate = DateTime.Now, IsStarAlliance = true, Name = "Company 1", IsAproved = true });
                _context.Partners.Add(new Partner { CreationDate = DateTime.Now, IsStarAlliance = false, Name = "Company 2", IsAproved = true });
                _context.Partners.Add(new Partner { CreationDate = DateTime.Now, IsStarAlliance = true, Name = "Company 3", IsAproved = false });
                _context.Partners.Add(new Partner { CreationDate = DateTime.Now, IsStarAlliance = false, Name = "Company 4", IsAproved = false });
                await _context.SaveChangesAsync();
            }
            if (_context.MilesRequests.Count() == 0)
            {
                _context.MilesRequests.Add(new MilesRequest { ClientId = 1, IsAproved = false, IsDeleted = false, MilesAmount = 1000, PartnerId = 1, RequestCode = "cKqdprtsQr1" });
                await _context.SaveChangesAsync();
            }
            if (_context.Airports.Count() == 0)
            {
                //TODO: Next thing
                //throw new InsufficientExecutionStackException("Please load the airports in first!");
            }
            await _context.SaveChangesAsync();
        }
    }
}
