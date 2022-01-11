using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.UserDTO
{
    public class UploadImageDTO
    {
        public IFormFile Image { get; set; }
    }
}
