using AutoMapper;
using Azure.Storage.Blobs;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.Core.Services;
using Provis.Core.Validation;
using System.Collections.Generic;

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
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConfirmEmailService, ConfirmEmailService>();
            services.AddScoped<ITemplateService, TemplateService>();
        }

        public static void AddFileService(this IServiceCollection services, IConfiguration configuration)
        {
            var allowStoreInAzureBlobStore = configuration.GetSection("FileSettings")
                .GetValue<bool>("AllowStoreInAzureBlobStore");

            if (allowStoreInAzureBlobStore)
            {
                services.AddScoped(_ =>
                    new BlobServiceClient(configuration.GetSection("AzureBlobStorageSettings")
                        .GetValue<string>("AccessKey")));
                services.AddScoped<IFileService, AzureBlobStorageService>();
            }
            else
            {
                services.AddScoped<IFileService, FileService>();
            }
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

        public static void ConfigureClientApplicationUrl(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ClientUrl>(configuration.GetSection("ClientServer"));
        }

        public static void ConfigureValidationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var configItem =
                configuration.GetSection("RolesAccess")
                .Get<Dictionary<WorkSpaceRoles, List<WorkSpaceRoles>>>();
            services.AddSingleton<RoleAccess>(new RoleAccess() { RolesAccess = configItem });
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

        public static void ConfigureRolesAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RoleAccess>(configuration.GetSection("RolesAccess"));
        }        
    }
}
