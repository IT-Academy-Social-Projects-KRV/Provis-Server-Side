using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.CommentsDTO
{
    public class CommentAttachmentsDTO
    {
        public IFormFile Attachment { get; set; }
        public int CommentId { get; set; }
        public int WorkspaceId { get; set; }

    }
}
