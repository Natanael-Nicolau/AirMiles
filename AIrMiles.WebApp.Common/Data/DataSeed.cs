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
                user = new User
                {
                    FirstName = "Miles",
                    LastName = "Júnior",
                    Email = "milhas@yopmail.com",
                    UserName = "milhas@yopmail.com",
                    PhoneNumber = "223232323",
                    BirthDate = new DateTime(1995, 4, 25),
                    PhotoUrl = $"~/images/Users/Default_User_Image.png"
                };

                var result = await _userRepository.AddUserAsync(user, "123123");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
                }


                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                await _userRepository.ConfirmEmailAsync(user, token);


                var isInRole = await _userRepository.IsUserInRoleAsync(user, "Admin");
                if (!isInRole)
                {
                    await _userRepository.AddUsertoRoleAsync(user, "Admin");
                }

            }

            if (_context.MilesTypes.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "Status", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Bonus", IsAproved = true, IsDeleted = false });
            }
            if (_context.ReservationTypes.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "1Way", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "2Ways", IsAproved = true, IsDeleted = false });
            }
            if (_context.FlightClasses.Count() == 0)
            {
                _context.MilesTypes.Add(new MilesType { Description = "Discount", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Basic", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Classic", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Plus", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Executive", IsAproved = true, IsDeleted = false });
                _context.MilesTypes.Add(new MilesType { Description = "Top Executive", IsAproved = true, IsDeleted = false });
            }
            if (_context.Airports.Count() == 0)
            {
                //TODO: Next thing
                //throw new InsufficientExecutionStackException("Please load the airports in first!");
            }

        }
    }
}
