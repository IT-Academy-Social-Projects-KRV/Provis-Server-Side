using Provis.Core.Entities.UserEntity;
using Provis.Core.Interfaces;

namespace Provis.Core.Entities.RefreshTokenEntity
{
    public class RefreshToken: IBaseEntity
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
