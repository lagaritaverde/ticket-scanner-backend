using static Home.Api.AuthClient;

namespace Home.Api {
    public class TicketUser {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public IReadOnlyList<string> AllowedAccountingGroup { get; private set; }

        public static TicketUser FromUserSession(UserSession session) {
            return new TicketUser() {
                UserId = session.UserId,
                UserName = session.UserName,
                Email = session.Email,
                AllowedAccountingGroup = session.AllowedAccountingGroup,
            };
        }
    }

    public class UserContext {
        public TicketUser User { get; set; }
    }
}
