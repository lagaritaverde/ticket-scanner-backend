using Microsoft.AspNetCore.Mvc;

namespace Home.Auth.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {
        private readonly SessionStore sessionStore;
        private readonly ILogger<AuthController> _logger;

        public AuthController(SessionStore sessionStore, ILogger<AuthController> logger) {
            this.sessionStore = sessionStore;
            _logger = logger;
        }

        [HttpGet(Name = "GetSession")]
        [Produces<UserSession>]
        public IActionResult GetSession(string sessionId) {
            var session = sessionStore.Get(sessionId);
            if (session == null) return NotFound();

            return Ok(session);
        }
    }
}
