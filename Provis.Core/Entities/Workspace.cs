using Provis.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Provis.Core.Entities
{
    public class Workspace : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateOfCreate { get; set; }

        public List<User> Users { get; set; } = new List<User>();

        public List<UserWorkspace> UserWorkspaces { get; set; } = new List<UserWorkspace>();
        public List<Task> Tasks { get; set; } = new List<Task>();
    }
}
