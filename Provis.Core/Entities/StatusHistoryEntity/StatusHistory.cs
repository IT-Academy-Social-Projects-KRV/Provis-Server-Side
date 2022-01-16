using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities.StatusHistoryEntity
{
    public class StatusHistory : IBaseEntity
    {
        public int Id { get; set; }

        public DateTime DateOfChange { get; set; }

        public int TaskId { get; set; }
        public WorkspaceTask Task { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
