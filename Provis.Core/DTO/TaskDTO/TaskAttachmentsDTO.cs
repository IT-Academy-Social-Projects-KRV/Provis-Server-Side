using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAttachmentsDTO
    {
        public IFormFile Attachment { get; set; }
        public int TaskId { get; set; }
        public int workspaceId { get; set; }

    }
}
