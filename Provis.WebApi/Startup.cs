using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Provis.Core;
using Provis.Infrastructure;
using Provis.WebApi.Middleweres;
using Provis.WebApi.ServiceExtension;

namespace Provis.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext(Configuration.GetConnectionString("DefaultConnection"));
            services.AddIdentityDbContext();

            services.AddRepositories();
            services.AddCustomServices();
            services.AddFluentValitation();
            services.ConfigJwtOptions(Configuration.GetSection("JwtOptions"));
            services.ConfigureMailSettings(Configuration);
            services.ConfigureImageSettings(Configuration);
            services.ConfigureFileSettings(Configuration);
            services.AddAutoMapper();

            services.AddSwagger();
            services.AddPolicyServices();
            services.AddJwtAuthentication(Configuration);
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Provis.WebApi v1"));
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();

            app.UseCors(c =>
            {
                c.AllowAnyOrigin();
                c.AllowAnyHeader();
                c.AllowAnyMethod();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
