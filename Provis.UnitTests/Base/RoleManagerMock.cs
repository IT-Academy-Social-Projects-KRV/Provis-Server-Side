using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace Provis.UnitTests.Base
{
    class RoleManagerMock
    {
        public static Mock<RoleManager<TRole>> GetRoleManager<TRole>()
            where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var logger = new Mock<ILogger<RoleManager<TRole>>>();

            IList<IRoleValidator<TRole>> roleValidators = new List<IRoleValidator<TRole>>
            {
                new RoleValidator<TRole>()
            };

            return new Mock<RoleManager<TRole>>(
                store.Object,
                roleValidators,
                keyNormalizer.Object,
                errors.Object,
                logger.Object);
        }
    }
}
