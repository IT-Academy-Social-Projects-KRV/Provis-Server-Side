using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Interfaces.Repositories;
using Provis.Infrastructure.Data.Repositories;

namespace Provis.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        }
    }
}
