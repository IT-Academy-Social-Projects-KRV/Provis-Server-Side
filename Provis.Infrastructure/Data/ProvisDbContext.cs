using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Provis.Core.Entities;

namespace Provis.Infrastructure.Data
{
    public class ProvisDbContext: IdentityDbContext
    {
        public ProvisDbContext(DbContextOptions<ProvisDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Workspace> Workspaces { get; set; }
    }
}
