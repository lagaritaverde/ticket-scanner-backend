namespace Home.Api {
    public class AuthClient {
        private readonly HttpClient httpClient;

        public AuthClient(HttpClient httpClient) {
            this.httpClient = httpClient;
        }

        public async Task<UserSession?> Get(string sessionId) {

            var response = await this.httpClient.GetAsync("Auth?sessionId=" + sessionId);

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<UserSession>();
        }

        public class UserSession {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public List<string> AllowedAccountingGroup { get; set; }
        }
    }
}
