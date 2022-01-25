﻿using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Interfaces;
using System;

namespace Provis.Core.Entities.CommentEntity
{
    public class Comment : IBaseEntity
    {
        public int Id { get; set; }

        public string CommentText { get; set; }
        public DateTimeOffset DateOfCreate { get; set; }

        public int TaskId { get; set; }
        public WorkspaceTask Task { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
