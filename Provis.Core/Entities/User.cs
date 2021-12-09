using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Surname { get; set; }

        public DateTime CreateDate { get; set; } // DateTime UTC

        public ICollection Workspaces { get; set; }

        public ICollection Roles { get; set; }

        public User()
        {
            Workspaces = new List<Workspace>();
            Roles = new List<Role>();
        }
    }
}
