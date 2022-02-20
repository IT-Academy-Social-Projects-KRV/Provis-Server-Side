using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.CommentsDTO
{
    public class CommentAttachmentInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}
