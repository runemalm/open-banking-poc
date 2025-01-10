using System.Reflection;

namespace DDD.Infrastructure.Repositories.EF.Configurations
{
    public static class EfDbContextConfiguration
    {
        public static readonly List<Assembly> AdditionalAssemblies = new();
    }
}
