namespace Provis.Core.DTO.UserDTO
{
    public class UserChangePasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string Code { get; set; }
    }
}
