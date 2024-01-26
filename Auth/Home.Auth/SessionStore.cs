using System.Collections.Concurrent;

namespace Home.Auth {
    public class SessionStore {

        private ConcurrentDictionary<string, UserSession> sessions = new();

        public SessionStore() {

        }

        public void Add(string sessionId, UserSession session) {
            sessions[sessionId] = session;
        }

        public void Remove(string sessionId) {
            sessions.Remove(sessionId, out var session);
        }
        public void Clear() {
            sessions.Clear();
        }

        public UserSession? Get(string sessionId) {
            if (sessions.TryGetValue(sessionId, out var session)) return session;
            return null;
        }
    }
}
