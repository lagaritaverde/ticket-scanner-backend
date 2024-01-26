using Microsoft.AspNetCore.Mvc;

namespace Home.Auth.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase {
        private readonly SessionStore sessionStore;
        private readonly ILogger<LoginController> _logger;

        public LoginController(SessionStore sessionStore, ILogger<LoginController> logger) {
            this.sessionStore = sessionStore;
            _logger = logger;
        }

        [HttpPost(Name = "Login")]
        public LogedUser Login(LoginRequest loginRequest) {

            var token = Guid.NewGuid().ToString();

            sessionStore.Add(token, new UserSession() {
                UserId = Guid.NewGuid().ToString(),
                UserName = loginRequest.User
            });

            return new LogedUser() {
                User = loginRequest.User,
                Token = token,
            };
        }

        public class LogedUser {
            public string User { get; set; }
            public string Token { get; set; }
        }

        public class LoginRequest {
            public string User { get; set; }
            public string Password { get; set; }
        }
    }
}
