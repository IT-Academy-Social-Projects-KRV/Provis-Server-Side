using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskAttachmentInfoDTO
    {
        public int Id { get; set; }
        public string AttachmentUrl { get; set; }
    }
}
