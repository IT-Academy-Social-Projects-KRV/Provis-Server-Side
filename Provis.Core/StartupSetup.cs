using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.Core.Validation;
using System;

namespace Provis.Core
{
    public static class StartupSetup
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<ISmtpService, SmtpService>();
        }

        public static void AddFluentValitation(this IServiceCollection services)
        {
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<UserLogValidation>());
        }

        public static void ConfigJwtOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtOptions>(config);
        }
    }
}
