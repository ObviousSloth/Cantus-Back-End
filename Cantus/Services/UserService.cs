using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Cantus.Data;
using Cantus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cantus.Services
{
    public class UserService
    {
        private readonly IManagementApiClient _managementApiClient;
        private readonly UserDbContext _dbContext;

        public UserService(IManagementApiClient managementApiClient, UserDbContext dbContext)
        {
            _managementApiClient = managementApiClient;
            _dbContext = dbContext;
        }
     
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.Where(u => u.Auth0Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(UserToDTO(user));
        }



        public async Task<CantusUser> CreateUserAsync(string email, string password)
        {
            var auth0User = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
            {
                Connection = "Username-Password-Authentication",
                Email = email,
                Password = password
            });

            var newUser = new CantusUser
            {
                Auth0Id = auth0User.UserId,
                Email = auth0User.Email,
                EmailVerified = auth0User.EmailVerified ?? false
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return newUser;
        }

        // Add other user management methods as needed
    }
}
