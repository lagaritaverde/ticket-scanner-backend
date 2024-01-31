﻿namespace Home.Auth {
    public class UserSession {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> AllowedAccountingGroup { get; set; }
    }
}
