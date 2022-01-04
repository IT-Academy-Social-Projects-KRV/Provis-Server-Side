using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Entities;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Provis.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly SignInManager<User> _signInManager;
        protected readonly IJwtService _jwtService;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IRepository<RefreshToken> _refreshTokenRepository;
        protected readonly IEmailSenderService _emailSenderService;

        public AuthenticationService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            RoleManager<IdentityRole> roleManager,
            IRepository<RefreshToken> refreshTokenRepository,
            IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _refreshTokenRepository = refreshTokenRepository;
            _emailSenderService = emailSenderService;
        }

        public async Task<UserAutorizationDTO> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.Unauthorized, "Incorrect login or password!");
            }

            if(!await _userManager.CheckPasswordAsync(user, password))
            {
                throw new HttpException(System.Net.HttpStatusCode.Unauthorized, "Incorrect login or password!");
            }

            if(await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return await GenerateTwoStepVerificationCode(user);
            }
           
            return await GenerateUserTokens(user);
        }

        private async Task<UserAutorizationDTO> GenerateUserTokens(User user)
        {
            var claims = _jwtService.SetClaims(user);

            var token = _jwtService.CreateToken(claims);
            var refeshToken = await CreateRefreshToken(user);

            var tokens = new UserAutorizationDTO()
            {
                Token = token,
                RefreshToken = refeshToken
            };

            return tokens;
        }

        private async Task<string> CreateRefreshToken(User user)
        {
            var refeshToken = _jwtService.CreateRefreshToken();

            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x => x.UserId == user.Id);

            RefreshToken rt = new RefreshToken()
            {
                Token = refeshToken,
                UserId = user.Id
            };

            await _refreshTokenRepository.AddAsync(rt);
            await _refreshTokenRepository.SaveChangesAsync();

            return refeshToken;
        }

        private async Task<UserAutorizationDTO> GenerateTwoStepVerificationCode(User user)
        {
            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);

            if(!providers.Contains("Email"))
            {
                throw new HttpException(System.Net.HttpStatusCode.Unauthorized, "Invalid 2-Step Verification Provider");
            }

            var twoFactorToken = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var message = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Provis authentication code",
                Body = $"<div><h1>Your code:</h1> <label>{twoFactorToken}</label></div>"
            };

            await _emailSenderService.SendEmailAsync(message);

            return new UserAutorizationDTO() { Is2StepVerificationRequired = true, Provider = "Email" };
        }

        public async Task<UserAutorizationDTO> LoginTwoStepAsync(UserTwoFactorDTO twoFactorDTO)
        {
            var user = await _userManager.FindByEmailAsync(twoFactorDTO.Email);

            if(user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid Request");
            }

            var validVerification = await _userManager.VerifyTwoFactorTokenAsync(user, twoFactorDTO.Provider, twoFactorDTO.Token);

            if(!validVerification)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid Token Verification");
            }

            return await GenerateUserTokens(user);
        }

        public async Task RegistrationAsync(User user, string password, string roleName)
        {
            user.CreateDate = DateTime.UtcNow;
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errorMessage.Append(error.ToString() + " ");
                }
            }

            var findRole = await _roleManager.FindByNameAsync(roleName);

            if (findRole == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<UserAutorizationDTO> RefreshTokenAsync(UserAutorizationDTO userTokensDTO)
        {
            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x=>x.Token == userTokensDTO.RefreshToken);

            if(refeshTokenFromDb == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "Invalid refrash token");
            }

            var claims = _jwtService.GetClaimsFromExpiredToken(userTokensDTO.Token);
            var newToken = _jwtService.CreateToken(claims);
            var newRefreshToken = _jwtService.CreateRefreshToken();

            refeshTokenFromDb.Token = newRefreshToken;
            await _refreshTokenRepository.UpdateAsync(refeshTokenFromDb);
            await _refreshTokenRepository.SaveChangesAsync();

            var tokens = new UserAutorizationDTO()
            { 
                Token = newToken,
                RefreshToken = newRefreshToken
            };

            return tokens;
        }

        public async Task LogoutAsync(UserAutorizationDTO userTokensDTO)
        {
            var refeshTokenFromDb = await _refreshTokenRepository.Query().FirstOrDefaultAsync(x => x.Token == userTokensDTO.RefreshToken);

            if (refeshTokenFromDb == null)
            {
                return;
            }

            await _refreshTokenRepository.DeleteAsync(refeshTokenFromDb);
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
