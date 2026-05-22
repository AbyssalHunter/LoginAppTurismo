using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace appTurismo.Helpers
{
    public class CustomSupabaseSessionHandler : IGotrueSessionPersistence<Session>
    {
        private const string SessionKey = "SUPABASE_SESSION_TOKEN";

        public void DestroySession()
        {
            Preferences.Default.Remove(SessionKey);
        }

        public Session? LoadSession()
        {
            if (!Preferences.Default.ContainsKey(SessionKey))
                return null;

            var json = Preferences.Default.Get<string?>(SessionKey, null);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var session = JsonConvert.DeserializeObject<Session>(json);
                // Return null if token is expired completely
                if (session?.CreatedAt.AddSeconds(session.ExpiresIn) <= DateTime.UtcNow)
                {
                    return null;
                }
                return session;
            }
            catch
            {
                return null;
            }
        }

        public void SaveSession(Session session)
        {
            var json = JsonConvert.SerializeObject(session);
            Preferences.Default.Set(SessionKey, json);
        }
    }
}