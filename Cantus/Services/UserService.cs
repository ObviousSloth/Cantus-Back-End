using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Cantus.Data;
using Cantus.Models;

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

        public async Task<UserDTO> CreateUserAsync(string email, string password)
        {
            var auth0User = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
            {
                Connection = "Username-Password-Authentication",
                Email = email,
                Password = password
            });

            var newUser = new UserDTO
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
