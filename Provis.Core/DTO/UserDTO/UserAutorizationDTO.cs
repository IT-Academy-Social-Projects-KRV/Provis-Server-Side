namespace Provis.Core.DTO.userDTO
{
    public class UserAutorizationDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Provider { get; set; }
        public bool Is2StepVerificationRequired { get; set; }
    }
}
