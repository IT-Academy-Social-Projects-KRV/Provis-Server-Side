using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using Provis.Core.Entities.WorkspaceTaskEntity;

namespace Provis.Core.Entities.WorkspaceTaskAttachmentEntity
{
    public class WorkspaceTaskAttachment : IBaseEntity
    {
        public int Id { get; set; } 

        public string AttachmentUrl { get; set; }

        public int TaskId { get; set; }
        public WorkspaceTask Task { get; set; }
    }
}
