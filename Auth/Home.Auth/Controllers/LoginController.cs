using Home.Auth.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Home.Auth.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase {
        private readonly SessionStore sessionStore;
        private readonly UserDbContext userDbContext;
        private readonly ILogger<LoginController> _logger;

        public LoginController(SessionStore sessionStore, UserDbContext userDbContext, ILogger<LoginController> logger) {
            this.sessionStore = sessionStore;
            this.userDbContext = userDbContext;
            _logger = logger;
        }

        [HttpPost(Name = "Login")]
        [Produces<LogedUser>]
        public async Task<IActionResult> Login(LoginRequest loginRequest) {

            var user = await userDbContext.Users
                .SingleOrDefaultAsync(x => x.UserName == loginRequest.User && x.Password == loginRequest.Password);

            if (user == null) {
                return Forbid();
            }

            var token = Guid.NewGuid().ToString();

            sessionStore.Add(token, new UserSession() {
                UserId = user.Id,
                UserName = user.UserName,
                AllowedAccountingGroup = user.AllowedAccountingGroup,
                Email = user.Email
            });

            return Ok(new LogedUser() {
                User = loginRequest.User,
                Token = token,
                Email = user.Email,
                AllowedAccountingGroup = user.AllowedAccountingGroup
            });
        }

        public class LogedUser {
            public string User { get; set; }
            public string Token { get; set; }
            public string Email { get; set; }
            public List<string> AllowedAccountingGroup { get; set; }
        }

        public class LoginRequest {
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
