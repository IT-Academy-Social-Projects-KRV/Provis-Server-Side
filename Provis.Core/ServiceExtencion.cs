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
    public static class ServiceExtencion
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            
        }
    }
}
