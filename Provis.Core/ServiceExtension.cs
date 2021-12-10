using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Provis.Core
{
    public static class ServiceExtension
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();         
        }

        public static void IdentityDbContext(this IServiceCollection services)
        {         
            //services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"))
        }
    }
}
