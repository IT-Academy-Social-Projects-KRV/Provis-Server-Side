using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Provis.Infrastructure.Data
{
    public class ProvisDbContext: IdentityDbContext
    {
        public ProvisDbContext(DbContextOptions<ProvisDbContext> options): base(options)
        {

        }
    }
}
