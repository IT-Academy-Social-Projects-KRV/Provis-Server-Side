using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.UserDTO;
using Provis.Core.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfirmEmailService _confirmEmailService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;
        public UserController(IUserService userService, IConfirmEmailService confirmEmailService)
        {
            _userService = userService;
            _confirmEmailService = confirmEmailService;
        }

        [HttpGet]
        [Authorize]
        [Route("info")]
        public async Task<IActionResult> GetUserPersonalInfoAsync()
        {
            var userInfo = await _userService.GetUserPersonalInfoAsync(UserId);

            return Ok(userInfo);
        }

        [HttpPut]
        [Authorize]
        [Route("info")]
        public async Task ChangeInfoAsync([FromBody] UserChangeInfoDTO userChangeInfoDTO)
        {
            await _userService.ChangeInfoAsync(UserId, userChangeInfoDTO);
        }

        [HttpGet]
        [Authorize]
        [Route("invites")]
        public async Task<IActionResult> GetUserInviteInfoAsync()
        {
            var userInviteList = await _userService.GetUserInviteInfoListAsync(UserId);

            return Ok(userInviteList);
        }

        [HttpGet]
        [Authorize]
        [Route("active-invite")]
        public async Task<IActionResult> GetUserActiveInviteAsync()
        {
            var activeInvite = await _userService.IsActiveInviteAsync(UserId);

            return Ok(activeInvite);
        }

        [HttpGet]
        [Authorize]
        [Route("confirm-email")]
        public async Task<IActionResult> SendConfirmMailAsync()
        {
            await _confirmEmailService.SendConfirmMailAsync(UserId);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] UserConfirmEmailDTO confirmEmailDTO)
        {
            await _confirmEmailService.ConfirmEmailAsync(UserId, confirmEmailDTO);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("image")]
        public async Task<IActionResult> UpdateImageAsync([FromForm] UserUploadImageDTO uploadImage)
        {
            await _userService.UpdateUserImageAsync(uploadImage.Image, UserId);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("image")]
        public async Task<FileResult> GetImageAsync()
        {
            //We don't use IDisposable.Dispose here from type DownloadFile
            //because it will invoke from FileStreamResult
            var file = await _userService.GetUserImageAsync(UserId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [HttpGet]
        [Authorize]
        [Route("image/user/{userId}")]
        public async Task<FileResult> GetImageAsync(string userId)
        {
            var file = await _userService.GetUserImageAsync(userId);

            return File(file.Content, file.ContentType, file.Name);
        }

        [HttpPost]
        [Authorize]
        [Route("two-factor-auntification/enable")]
        public async Task<IActionResult> Change2faStatusAsync([FromBody] UserChange2faStatusDTO statusDTO)
        {
            await _userService.ChangeTwoFactorVerificationStatusAsync(UserId, statusDTO);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("two-factor-auntification/enable")]
        public async Task<IActionResult> CheckIsTwoFactorVerificationAsync()
        {
            var result = await _userService.CheckIsTwoFactorVerificationAsync(UserId);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("two-factor-auntification/code")]
        public async Task<IActionResult> SendTwoFactorCodeAsync()
        {
            await _userService.SendTwoFactorCodeAsync(UserId);

            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("password")]
        public async Task<IActionResult> SetPasswordAsync([FromBody] UserSetPasswordDTO userSetPasswordDTO)
        {
            await _userService.SetPasswordAsync(UserId, userSetPasswordDTO);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        [Route("password/check")]
        public async Task<IActionResult> IsHavePasswordAsync()
        {
            return Ok(await _userService.IsHavePasswordAsync(UserId));
        }
    }
}
