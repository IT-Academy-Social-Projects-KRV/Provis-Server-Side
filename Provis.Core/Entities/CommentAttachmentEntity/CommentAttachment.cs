using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using Provis.Core.Entities.CommentEntity;

namespace Provis.Core.Entities.CommentAttachmentEntity
{
    public class CommentAttachment : IBaseEntity
    {
        public int Id { get; set; }
        public string AttachmentPath { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
