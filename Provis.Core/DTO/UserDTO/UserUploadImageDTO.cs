﻿using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.UserDTO
{
    public class UserUploadImageDTO
    {
        public  int WorkspaceId { get; set; }
        public IFormFile Image { get; set; }
    }
}
