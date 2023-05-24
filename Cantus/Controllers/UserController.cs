using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cantus.Models;
using Cantus.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cantus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IManagementApiClient _managementApiClient;
        private readonly UserDbContext _dbContext;

        public UserController(IManagementApiClient managementApiClient, UserDbContext dbContext)
        {
            _managementApiClient = managementApiClient;
            _dbContext = dbContext;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }

            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _dbContext.Users.Where(u => u.Auth0Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(UserToDTO(user));
        }

        //GET by id: /api/Users/{id}
        [HttpGet("/api/[controller]/{id}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserById(long id)
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(UserToDTO(user));
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //Auth0 POST endpoint, don't change the parameters of this function! Use null for extra user values.
        [HttpPost]
        public async Task<ActionResult> PostUser(string email, string username, string auth0Id)
        {
            // validate that auth0Id exists in Auth0
            var auth0User = await _managementApiClient.Users.GetAsync(auth0Id);
            if (auth0User == null)
            {
                // If the user is not found in Auth0, return a bad request
                return BadRequest("Invalid auth0Id");
            }

            // validate that email exists in Auth0
            var usersByEmail = await _managementApiClient.Users.GetUsersByEmailAsync(email);
            if (usersByEmail == null || usersByEmail.Count == 0)
            {
                // If no users are found with the given email in Auth0, return a bad request
                return BadRequest("Invalid email");
            }

            // create new user object with properties from input
            CantusUser newUser = new()
            {
                Email = email,
                Username = username,
                Auth0Id = auth0Id,
     
            };

            // add the new user object to the context
            _dbContext.Users.Add(newUser);

            // save changes to the context
            await _dbContext.SaveChangesAsync();

            // return a CreatedAtAction result with the new user object
            return CreatedAtAction("GetUser", new { id = newUser.Id }, newUser);
        }

        // PUT: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> PutUser(UserDTO userDto)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            CantusUser user = await _dbContext.Users.Where(u => u.Auth0Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }
            //get user from auth0 DB and assign fields
            var authUser = await _managementApiClient.Users.GetAsync(user.Auth0Id);
            var emailRequest = new UserUpdateRequest();
            emailRequest.Email = userDto.Email;
            var usernameRequest = new UserUpdateRequest();
            usernameRequest.FullName = userDto.Username;

            user.Email = userDto.Email;
            user.Username = userDto.Username;
          

            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();

                //update user email in auth0 DB
                await _managementApiClient.Users.UpdateAsync(user.Auth0Id, emailRequest);
                //update user username in auth0 DB

                await _managementApiClient.Users.UpdateAsync(user.Auth0Id, usernameRequest);
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static UserDTO UserToDTO(CantusUser user) => new UserDTO
        {
            Email = user.Email,
            Username = user.Username,
         
        };
    }
}
