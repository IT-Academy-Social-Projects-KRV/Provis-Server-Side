using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provis.Core.Entities
{
    public class Task : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfEnd { get; set; }

        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public Status Status { get; set; }

        public int WorkspaceId { get; set; }

        [ForeignKey("WorkspaceId")]
        public Workspace Workspace { get; set; }

        public string TaskCreaterId { get; set; }

        [ForeignKey("TaskCreaterId")]
        public User UserCreator { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}
