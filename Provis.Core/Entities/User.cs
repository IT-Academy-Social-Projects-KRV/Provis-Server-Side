using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public DateTime CreateDate { get; set; } // DateTime UTC

        public List<Task> Tasks { get; set; } = new List<Task>();

        public List<Workspace> Workspaces { get; set; } = new List<Workspace>();

        public List<UserWorkspace> UserWorkspaces { get; set; } = new List<UserWorkspace>();

        public List<UsersTasks> UsersTasks { get; set; } = new List<UsersTasks>();

        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
