﻿using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.Core.Validation;

namespace Provis.Core
{
    public static class StartupSetup
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IWorkspaceService, WorkspaceService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddScoped<ISmtpService, SmtpService>();
<<<<<<< HEAD
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConfirmEmailService, ConfirmEmailService>();
=======
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IFileService, FileService>();
>>>>>>> efcbdbe... Add configure for image and file settings.
        }

        public static void AddFluentValitation(this IServiceCollection services)
        {
            services.AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<UserLogValidation>());
        }

        public static void ConfigJwtOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtOptions>(config);
        }

        public static void ConfigureMailSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("EmailSettings"));
        }

        public static void ConfigureImageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ImageSettings>(configuration.GetSection("ImageSettings"));
        }

        public static void ConfigureFileSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FileSettings>(configuration.GetSection("FileSettings"));
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
