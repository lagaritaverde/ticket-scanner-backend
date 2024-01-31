using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Home.Auth {

    public class AuthenticationOptions : AuthenticationSchemeOptions {

    }

    public class HomeAuthenticationHandler : AuthenticationHandler<AuthenticationOptions> {
        private readonly SessionStore sessionStore;

        public HomeAuthenticationHandler(SessionStore sessionStore, IOptionsMonitor<AuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder) {
            this.sessionStore = sessionStore;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() {

            if (!Request.Headers.Authorization.Any()) {
                return Task.FromResult(AuthenticateResult.Fail("Session required"));
            }

            var value = Request.Headers.Authorization[0];

            var token = value.Split(" ")[1];

            var session = sessionStore.Get(token);

            if (session is null) {
                return Task.FromResult(AuthenticateResult.Fail("Invalid session"));
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, session.UserId) ,
                new Claim(ClaimTypes.Name, session.UserName)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);

        }
    }
}
