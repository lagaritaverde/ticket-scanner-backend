﻿namespace Home.Auth.Entities {
    public class User {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<string> AllowedAccountingGroup { get; set; }
    }
}
