using Home.Auth.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Home.Auth.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase {
        private readonly UserDbContext userDbContext;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(UserDbContext userDbContext, ILogger<RegisterController> logger) {
            this.userDbContext = userDbContext;
            _logger = logger;
        }

        [HttpPost(Name = "Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest) {

            var users = await userDbContext.Users
                .Where(x => x.UserName == registerRequest.UserName || x.Email == registerRequest.Email)
                .ToArrayAsync();

            if (users.Any(x => x.UserName == registerRequest.UserName)) {
                return BadRequest("User already exist");
            }

            if (users.Any(x => x.Email == registerRequest.Email)) {
                return BadRequest("Email already exist");
            }

            await userDbContext.Users.AddAsync(new Entities.User() {
                Id = Guid.NewGuid().ToString(),
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                AllowedAccountingGroup = new List<string>()
            });

            await userDbContext.SaveChangesAsync();

            return Ok();
        }

        public class RegisterRequest {
            public string UserName { get; set; }
            public string Email { get; set; }

            public string Password { get; set; }
        }
    }
}
