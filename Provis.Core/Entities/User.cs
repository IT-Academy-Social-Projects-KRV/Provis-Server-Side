using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Surname { get; set; }

        public DateTime CreateDate { get; set; } // DateTime UTC

        public List<Task> Tasks { get; set; } = new List<Task>();

        public List<Workspace> Workspaces { get; set; } = new List<Workspace>();
    }
}
