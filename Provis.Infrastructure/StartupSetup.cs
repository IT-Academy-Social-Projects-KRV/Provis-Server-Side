using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories;
using Provis.Core.Interfaces.Repositories.DapperRepositories.DapperRepositoriesEntity;
using Provis.Infrastructure.Data;
using Provis.Infrastructure.Data.Repositories;
using Provis.Infrastructure.Data.Repositories.DapperRepositories;

namespace Provis.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEntityRepository<>), typeof(EntityBaseRepository<>));
            services.AddScoped<IUserWorkspaceRepository, DapperUserWorkspaceRepository>();
            services.AddScoped<ICommentRepository, DapperCommentRepository>();
            services.AddScoped<IInviteUserRepository, DapperInviteUserRepository>();
            services.AddScoped<IUserWorkspaceRepository, DapperUserWorkspaceRepository>();
            services.AddScoped<IRefreshTokenRepository, DapperRefreshTokenRepository>();
            services.AddScoped<IRoleRepository, DapperRoleRepository>();
            services.AddScoped<IStatusHistoryRepository, DapperStatusHistoryRepository>();
            services.AddScoped<IStatusRepository, DapperStatusRepository>();
            services.AddScoped<IUserRepository, DapperUserRepository>();
            services.AddScoped<IUserRoleTagRepository, DapperUserRoleTagRepository>();
            services.AddScoped<IUserTaskRepository, DapperUserTaskRepository>();
            services.AddScoped<IWorkspaceRepository, DapperWorkspaceRepository>();
            services.AddScoped<IWorkspaceTaskAttachmentRepository, DapperWorkspaceTaskAttachmentRepository>();
            services.AddScoped<IWorkspaceTaskRepository, DapperWorkspaceTaskRepository>();
        }

        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ProvisDbContext>(x => x.UseSqlServer(connectionString));
        }

        public static void AddIdentityDbContext(this IServiceCollection services)
        {
            services.AddIdentity<User,
                IdentityRole>().AddEntityFrameworkStores<ProvisDbContext>().AddDefaultTokenProviders();
        }
    }
}
