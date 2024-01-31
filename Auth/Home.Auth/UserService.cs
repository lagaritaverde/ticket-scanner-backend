using Home.Auth.Entities;

namespace Home.Auth {
    public class UserService {

        public UserService() {

        }

        public async Task<string> Create(User user) {
            return await Task.FromResult(string.Empty);
        }

        public async Task<string> Get(string userId) {
            return await Task.FromResult(string.Empty);
        }
    }
}
