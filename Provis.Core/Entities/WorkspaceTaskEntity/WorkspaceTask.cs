﻿using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Provis.Core.Entities.WorkspaceTaskEntity
{
    public class WorkspaceTask : IBaseEntity
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

        public string TaskCreatorId { get; set; }
        public User TaskCreator { get; set; }

        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<StatusHistory> StatusHistories { get; set; } = new List<StatusHistory>();
        public List<WorkspaceTaskAttachment> Attachments { get; set; } = new List<WorkspaceTaskAttachment>();

    }
}
