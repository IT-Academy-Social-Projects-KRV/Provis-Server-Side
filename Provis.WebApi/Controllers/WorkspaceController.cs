﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Interfaces.Services;
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
            await _workspaceService.CreateWorkspace(createDTO, UserId);

            return Ok();
        }

        [HttpGet]
        [Route("getworlspacelist")]
        public async Task<WorkspaceListDTO> GetWorkspaceAsync()
        {
            var getList = await _workspaceService.GetWorkspaceList(UserId);

            return getList;
        }
    }
}
