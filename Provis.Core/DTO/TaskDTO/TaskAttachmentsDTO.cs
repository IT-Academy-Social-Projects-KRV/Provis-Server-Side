using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAttachmentsDTO
    {
        public List<IFormFile> Attachments { get; set; }
        public int TaskId { get; set; }
        public int workspaceId { get; set; }

    }
}
