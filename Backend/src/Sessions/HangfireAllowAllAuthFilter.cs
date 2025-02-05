using Hangfire.Dashboard;

namespace Sessions
{
    public class HangfireAllowAllAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true; // Allow all requests
    }
}
