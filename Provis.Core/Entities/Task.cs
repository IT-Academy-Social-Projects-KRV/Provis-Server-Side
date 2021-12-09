using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities
{
    public class Task : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfEnd { get; set; }

        public int StatusId { get; set; }

        public Status Status { get; set; }

        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public string TaskCreaterId { get; set; }

        public User User { get; set; }
    }
}
