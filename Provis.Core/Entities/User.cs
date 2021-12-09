using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Surname { get; set; }
    }
}
