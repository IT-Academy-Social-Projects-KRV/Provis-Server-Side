namespace Provis.Core.DTO.UserDTO
{
    public class UserTwoFactorDTO
    {
        public string Email { get; set; }

        public string Provider { get; set; }

        public string Token { get; set; }
    }
}
