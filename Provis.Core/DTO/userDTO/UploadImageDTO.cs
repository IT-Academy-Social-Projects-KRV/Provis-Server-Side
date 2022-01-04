using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.userDTO
{
    public class UploadImageDTO
    {
        public IFormFile Image { get; set; }
    }
}
