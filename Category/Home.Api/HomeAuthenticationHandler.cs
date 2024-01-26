using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Home.Api {

    public class AuthenticationOptions : AuthenticationSchemeOptions {

    }

    public class HomeAuthenticationHandler : AuthenticationHandler<AuthenticationOptions> {
        private readonly AuthClient authClient;

        public HomeAuthenticationHandler(AuthClient authClient, IOptionsMonitor<AuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder) {
            this.authClient = authClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {

            if (!this.Request.Headers.Authorization.Any()) {
                return AuthenticateResult.Fail("Session required");
            }

            var value = this.Request.Headers.Authorization[0];

            var token = value.Split(" ")[1];

            var session = await authClient.Get(token);

            if (session is null) {
                return AuthenticateResult.Fail("Invalid session");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, session.UserId) ,
                new Claim(ClaimTypes.Name, session.UserName)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
            var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
            return AuthenticateResult.Success(ticket);

        }
    }
}
