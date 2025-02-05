using Hangfire.Dashboard;

namespace Sessions
{
    public class HangfireBasicAuthFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;

        public HangfireBasicAuthFilter(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var header = httpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic "))
                return false;

            var encodedCredentials = header.Substring("Basic ".Length).Trim();
            var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            var parts = credentials.Split(':');

            return parts.Length == 2 && parts[0] == _username && parts[1] == _password;
        }
    }
}
