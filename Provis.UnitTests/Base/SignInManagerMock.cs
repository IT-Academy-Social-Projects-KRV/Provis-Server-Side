using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Provis.UnitTests.Base
{
    public class SignInManagerMock
    {
        public static Mock<SignInManager<TUser>> GetSignInManager<TUser>()
            where TUser : class
        {
            var userManager = UserManagerMock.GetUserManager<TUser>();
            var ctxAccessor = new Mock<IHttpContextAccessor>();
            var mockClaimsPrinFact = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            var mockOpts = new Mock<IOptions<IdentityOptions>>();
            //var mockLogger = new Mock<ILogger<SignInManager<TUser>>>();

            return new Mock<SignInManager<TUser>>(
                userManager.Object, 
                ctxAccessor.Object, 
                mockClaimsPrinFact.Object, 
                mockOpts.Object, 
                null, 
                null, 
                null);
        }
    }
}
