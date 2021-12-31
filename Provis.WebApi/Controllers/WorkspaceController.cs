﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.userDTO;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using Provis.WebApi.Policy;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Provis.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        protected readonly IWorkspaceService _workspaceService;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [Authorize]
        [HttpPost]
        [Route("addworkspace")]
        public async Task<IActionResult> AddWorkspaceAsync([FromBody] WorkspaceCreateDTO createDTO)
        {
            await _workspaceService.CreateWorkspaceAsync(createDTO, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{id}/deny")]
        public async Task<IActionResult> DenyInviteUserAsync(int id)
        {
            await _workspaceService.DenyInviteAsync(id, UserId);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("invite/{id}/accept")]
        public async Task<IActionResult> AcceptInviteUserAsync(int id)
        {
            await _workspaceService.AcceptInviteAsync(id, UserId);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("inviteuser")]
        public async Task<IActionResult> SendInviteToUser([FromBody] InviteUserDTO inviteUser)
        {
            await _workspaceService.SendInviteAsync(inviteUser, UserId);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("getworlspacelist")]
        public async Task<IActionResult> GetWorkspaceAsync()
        {
            var getList = await _workspaceService.GetWorkspaceListAsync(UserId);
            return Ok(getList);
        }

        [HttpPut]
        [Authorize]
        [WorkspaceRoles(new WorkSpaceRoles[] { WorkSpaceRoles.OwnerId, WorkSpaceRoles.ManagerId })]
        [Route("changerole")]
        public async Task<IActionResult> GetUserChangeRoleAsync([FromBody] ChangeRoleDTO userChangeRoleDTO)
        {
            var changeRole = await _workspaceService.ChangeUserRoleAsync(UserId, userChangeRoleDTO);

            return Ok(changeRole);
        }
    }
}
