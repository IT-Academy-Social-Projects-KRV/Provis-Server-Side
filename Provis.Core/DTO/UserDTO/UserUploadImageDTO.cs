using Microsoft.AspNetCore.Http;

namespace Provis.Core.DTO.UserDTO
{
    public class UserUploadImageDTO
    {
        public IFormFile Image { get; set; }
    }
}
