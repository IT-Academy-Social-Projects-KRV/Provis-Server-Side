using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.CommentAttachmentEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Provis.Core.Entities.CommentEntity
{
    public class Comment : IBaseEntity
    {
        public int Id { get; set; }

        public string CommentText { get; set; }
        public DateTime DateOfCreate { get; set; }

        public int TaskId { get; set; }
        public WorkspaceTask Task { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public List<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();
    }
}
