using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Surname { get; set; }

        public DateTime CreateDate { get; set; } // DateTime UTC
    }
}
