using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;

namespace Provis.UnitTests.Base
{
    public class UserManagerMock
    {
        public static Mock<UserManager<TUser>> GetUserManager<TUser>()
            where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var passwordHasher = new Mock<IPasswordHasher<TUser>>();

            IList<IUserValidator<TUser>> userValidators = new List<IUserValidator<TUser>>
            {
                new UserValidator<TUser>()
            };

            IList<IPasswordValidator<TUser>> passwordValidators = new List<IPasswordValidator<TUser>>
            {
                new PasswordValidator<TUser>()
            };

            userValidators.Add(new UserValidator<TUser>());
            passwordValidators.Add(new PasswordValidator<TUser>());

            var userManager = new Mock<UserManager<TUser>>(
                store.Object,
                null,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                null,
                null,
                null,
                null);

            return userManager;
        }
    }
}
