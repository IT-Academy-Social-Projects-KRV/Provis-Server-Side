using Provis.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Provis.Core.Entities
{
    public class Workspace : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateOfCreate { get; set; }

        public ICollection Users { get; set; }

        public ICollection Roles { get; set; }

        public Workspace()
        {
            Users = new List<User>();
            Roles = new List<Role>();
        }
    }
}
