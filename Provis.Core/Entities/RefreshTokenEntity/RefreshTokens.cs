using Ardalis.Specification;

namespace Provis.Core.Entities.RefreshTokenEntity
{
    public class RefreshTokens
    {
        internal class SerchRefreshToken: Specification<RefreshToken>
        {
            public SerchRefreshToken(string refreshToken)
            {
                Query
                    .Where(x => x.Token == refreshToken);
            }
        }
    }
}
