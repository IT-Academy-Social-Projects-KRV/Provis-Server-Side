﻿using FluentValidation;
using Provis.Core.DTO.userDTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Provis.Core.Entities;

namespace Provis.Core.Validation
{
    public class UserChangeInfoValidation : AbstractValidator<UserChangeInfoDTO>
    {
        private static UserManager<User> _userManager;
        public UserChangeInfoValidation(UserManager<User> manager)
        {
            _userManager = manager;

            RuleFor(user => user.Name)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.Surname)
                .NotNull()
                .Length(3, 50);

            RuleFor(user => user.Username)
                .NotNull()
                .Length(3, 50)
                .MustAsync(IsUniqueUserName).WithMessage("{PropertyName} already exists.");
        }
        private async Task<bool> IsUniqueUserName(string username, CancellationToken cancellationToken)
        {
            var userObject = await _userManager.FindByNameAsync(username);
            return userObject == null;
        }
    }
}
