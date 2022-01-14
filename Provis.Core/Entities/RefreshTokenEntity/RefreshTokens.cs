using Ardalis.Specification;

namespace Provis.Core.Entities.RefreshTokenEntity
{
    public class RefreshTokens
    {
        internal class SearchRefreshToken: Specification<RefreshToken>
        {
            public SearchRefreshToken(string refreshToken)
            {
                Query
                    .Where(x => x.Token == refreshToken);
            }
        }
    }
}
