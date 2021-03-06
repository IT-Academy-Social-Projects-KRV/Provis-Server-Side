using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Provis.Core.DTO.WorkspaceDTO;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers;
using Provis.Core.Helpers.Mails;
using Provis.Core.Helpers.Mails.ViewModels;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Provis.Core.Entities.UserTaskEntity;
using App.Metrics;
using Provis.Core.Metrics;
using System.Net;
using Provis.Core.Resources;
using Microsoft.EntityFrameworkCore;

namespace Provis.Core.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        protected readonly IEmailSenderService _emailSendService;
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<InviteUser> _inviteUserRepository;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<Role> _userRoleRepository;
        protected readonly IRepository<UserTask> _userTaskRepository;
        protected readonly IMapper _mapper;
        protected readonly RoleAccess _roleAccess;
        protected readonly ITemplateService _templateService;
        protected readonly IOptions<ClientUrl> _clientUrl;
        private readonly IMetrics _metrics;
        private readonly ISprintService _sprintService;

        public WorkspaceService(IRepository<User> user,
            UserManager<User> userManager,
            IRepository<Workspace> workspace,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<InviteUser> inviteUser,
            IRepository<Role> userRoleRepository,
            IEmailSenderService emailSenderService,
            IRepository<UserTask> userTasksRepository,
            IMapper mapper,
            RoleAccess roleAccess,
            ITemplateService templateService,
            IOptions<ClientUrl> options,
            IMetrics metrics,
            ISprintService sprintService)
        {
            _userRepository = user;
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _inviteUserRepository = inviteUser;
            _emailSendService = emailSenderService;
            _mapper = mapper;
            _roleAccess = roleAccess;
            _userRoleRepository = userRoleRepository;
            _templateService = templateService;
            _userTaskRepository = userTasksRepository;
            _clientUrl = options;
            _metrics = metrics;
            _sprintService = sprintService;
        }
        public async Task CreateWorkspaceAsync(WorkspaceCreateDTO workspaceDTO, string userid)
        {
            Workspace workspace = new()
            {
                DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                Name = workspaceDTO.Name,
                Description = workspaceDTO.Description
            };
            await _workspaceRepository.AddAsync(workspace);
            await _workspaceRepository.SaveChangesAsync();

            UserWorkspace userWorkspace = new()
            {
                UserId = userid,
                WorkspaceId = workspace.Id,
                RoleId = (int)WorkSpaceRoles.OwnerId
            };

            _metrics.Measure.Counter.Increment(WorkspaceMetrics.MembersCountByWorkspaceRole,
              MetricTagsConstructor.MembersCountByWorkspaceRole(workspace.Id, (int)WorkSpaceRoles.OwnerId));

            await _userWorkspaceRepository.AddAsync(userWorkspace);
            await _userWorkspaceRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task UpdateWorkspaceAsync(WorkspaceUpdateDTO workspaceUpdateDTO)
        {
            var workspaceRec = await _workspaceRepository.GetByKeyAsync(workspaceUpdateDTO.WorkspaceId);
            workspaceRec.WorkspaceNullChecking();

            _mapper.Map(workspaceUpdateDTO, workspaceRec);

            await _workspaceRepository.UpdateAsync(workspaceRec);

            await _workspaceRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task SendInviteAsync(WorkspaceInviteUserDTO inviteDTO, string ownerId)
        {
            var owner = await _userManager.FindByIdAsync(ownerId);

            if (owner.Email == inviteDTO.UserEmail)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.SendInviteYourself);
            }

            var inviteUser = await _userManager.FindByEmailAsync(inviteDTO.UserEmail);
            inviteUser.UserNullChecking();

            var workspace = await _workspaceRepository.GetByKeyAsync(inviteDTO.WorkspaceId);
            workspace.WorkspaceNullChecking();

            var inviteUserListSpecification = new InviteUsers.InviteList(inviteUser.Id, workspace.Id);
            var userWorkspaceInviteSpecification = new UserWorkspaces.WorkspaceMember(inviteUser.Id, workspace.Id);

            if (await _inviteUserRepository.AnyBySpecAsync(inviteUserListSpecification, x => x.IsConfirm.Value == true)
                && await _userWorkspaceRepository.GetFirstBySpecAsync(userWorkspaceInviteSpecification) != null)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.UserAcceptedInvite);
            }

            if (await _inviteUserRepository.AnyBySpecAsync(inviteUserListSpecification, x => x.IsConfirm == null))
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.UserAlreadyHasInvite);
            }

            InviteUser user = new InviteUser
            {
                Date = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                FromUser = owner,
                ToUser = inviteUser,
                Workspace = workspace,
                IsConfirm = null
            };

            await _inviteUserRepository.AddAsync(user);
            await _inviteUserRepository.SaveChangesAsync();

            await _emailSendService.SendEmailAsync(new MailRequest
            {
                ToEmail = inviteDTO.UserEmail,
                Subject = "Workspace invitation",
                Body = await _templateService.GetTemplateHtmlAsStringAsync("Mails/WorkspaceInvite",
                    new WorkspaceInvite()
                    {
                        Owner = owner.UserName,
                        WorkspaceName = workspace.Name,
                        UserName = inviteUser.UserName,
                        Uri = _clientUrl.Value.ApplicationUrl
                    })
            });

            await Task.CompletedTask;
        }

        public async Task DenyInviteAsync(int id, string userid)
        {
            var inviteUserRec = await _inviteUserRepository.GetByKeyAsync(id);
            inviteUserRec.InviteNullChecking();

            if (inviteUserRec.ToUserId != userid)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.CannotDenyInvite);
            }

            inviteUserRec.IsConfirm ??= false;

            await _inviteUserRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task<List<WorkspaceInfoDTO>> GetWorkspaceListAsync(string userid)
        {
            var specification = new UserWorkspaces.WorkspaceList(userid);
            var listWorkspace = await _userWorkspaceRepository.GetListBySpecAsync(specification);

            var listWorkspaceToReturn = _mapper.Map<List<WorkspaceInfoDTO>>(listWorkspace);

            return listWorkspaceToReturn;
        }

        public async Task AcceptInviteAsync(int inviteId, string userid)
        {
            var inviteUserRec = await _inviteUserRepository.GetByKeyAsync(inviteId);
            inviteUserRec.InviteNullChecking();

            if (inviteUserRec.ToUserId != userid)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.CannotAcceptInvite);
            }

            if (inviteUserRec.IsConfirm == true)
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.InviteAlreadyAccerted);
            }

            inviteUserRec.IsConfirm = true;

            var userTaskSpecification = new UserTasks.UserTaskList(userid, inviteUserRec.WorkspaceId, null);
            var userTasks = await _userTaskRepository.GetListBySpecAsync(userTaskSpecification);

            if (userTasks != null)
            {
                foreach (var userTask in userTasks)
                {
                    userTask.Item2.IsUserDeleted = false;
                }
            }

            UserWorkspace userWorkspace = new()
            {
                UserId = userid,
                WorkspaceId = inviteUserRec.WorkspaceId,
                RoleId = (int)WorkSpaceRoles.MemberId
            };

            _metrics.Measure.Counter.Increment(WorkspaceMetrics.MembersCountByWorkspaceRole,
              MetricTagsConstructor.MembersCountByWorkspaceRole(inviteUserRec.WorkspaceId, (int)WorkSpaceRoles.MemberId));

            await _userWorkspaceRepository.AddAsync(userWorkspace);
            await _inviteUserRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task<WorkspaceChangeRoleDTO> ChangeUserRoleAsync(string userId, WorkspaceChangeRoleDTO userChangeRole)
        {
            var modifierSpecification = new UserWorkspaces.WorkspaceMember(userId, userChangeRole.WorkspaceId);
            var modifier = await _userWorkspaceRepository.GetFirstBySpecAsync(modifierSpecification);

            var targetSpecification = new UserWorkspaces.WorkspaceMember(userChangeRole.UserId, userChangeRole.WorkspaceId);
            var target = await _userWorkspaceRepository.GetFirstBySpecAsync(targetSpecification);

            target.User.UserNullChecking();

            var roleId = (WorkSpaceRoles)modifier.RoleId;

            var targetCurrentRoleId = target.RoleId;

            if (_roleAccess.RolesAccess.ContainsKey(roleId) &&
                _roleAccess.RolesAccess[roleId]
                    .Any(p => p == (WorkSpaceRoles)target.RoleId) &&
                _roleAccess.RolesAccess[roleId]
                    .Any(p => p == (WorkSpaceRoles)userChangeRole.RoleId)
                )
            {
                try
                {
                    target.RoleId = userChangeRole.RoleId;
                    await _userWorkspaceRepository.SetOriginalRowVersion(target, userChangeRole.RowVersion);
                    await _userWorkspaceRepository.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new HttpException(HttpStatusCode.Conflict, ErrorMessages.ConcurrencyCheck);
                }

                _metrics.Measure.Counter.Decrement(WorkspaceMetrics.MembersCountByWorkspaceRole,
                    MetricTagsConstructor.MembersCountByWorkspaceRole(userChangeRole.WorkspaceId, targetCurrentRoleId));

                _metrics.Measure.Counter.Increment(WorkspaceMetrics.MembersCountByWorkspaceRole,
                    MetricTagsConstructor.MembersCountByWorkspaceRole(userChangeRole.WorkspaceId, userChangeRole.RoleId));

                return _mapper.Map<WorkspaceChangeRoleDTO>(target);
            }
            else
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }
        }

        public async Task<WorkspaceInfoDTO> GetWorkspaceInfoAsync(int workspaceId, string userId)
        {
            var specification = new UserWorkspaces.WorkspaceInfo(userId, workspaceId);
            var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);
            userWorkspace.UserWorkspaceNullChecking();

            var workspace = _mapper.Map<WorkspaceInfoDTO>(userWorkspace);

            return workspace;
        }

        public async Task<WorkspaceDescriptionDTO> GetWorkspaceDescriptionAsync(int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);
            workspace.WorkspaceNullChecking();

            var workspaceToReturn = _mapper.Map<WorkspaceDescriptionDTO>(workspace);

            return workspaceToReturn;
        }

        public async Task<List<WorkspaceInviteInfoDTO>>
            GetWorkspaceActiveInvitesAsync(int workspaceId, string userId)
        {
            var specification = new InviteUsers.InviteList(workspaceId);
            var invitesList = await _inviteUserRepository.GetListBySpecAsync(specification);

            var listToReturn = _mapper.Map<List<WorkspaceInviteInfoDTO>>(invitesList);

            return listToReturn;
        }

        public async Task<List<WorkspaceMemberDTO>> GetWorkspaceMembersAsync(int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);
            workspace.WorkspaceNullChecking();

            var specification = new UserWorkspaces.WorkspaceMemberList(workspaceId);
            var workspaceMembers = await _userWorkspaceRepository.GetListBySpecAsync(specification);

            var workspaceMembersToReturn = _mapper.Map<List<WorkspaceMemberDTO>>(workspaceMembers);

            return workspaceMembersToReturn;
        }

        public async Task DeleteFromWorkspaceAsync(int workspaceId, string userId)
        {
            var userWorkspSpecification = new UserWorkspaces.WorkspaceMember(userId, workspaceId);
            var userWorksp = await _userWorkspaceRepository.GetFirstBySpecAsync(userWorkspSpecification);

            if (userWorksp.RoleId == (int)WorkSpaceRoles.OwnerId)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.OwnerCannotLeaveWorkspace);
            }

            var userTaskSpecification = new UserTasks.UserTaskList(userId, workspaceId, null);
            var userTasks = await _userTaskRepository.GetListBySpecAsync(userTaskSpecification);

            if (userTasks != null)
            {
                foreach (var userTask in userTasks)
                {
                    userTask.Item2.IsUserDeleted = true;
                    _metrics.Measure.Counter.Decrement(WorkspaceMetrics.TaskRolesCountByWorkspace,
                        MetricTagsConstructor.TaskRolesCountByWorkspace(workspaceId, userTask.Item2.UserRoleTagId));
                }
            }

            _metrics.Measure.Counter.Decrement(WorkspaceMetrics.MembersCountByWorkspaceRole,
                   MetricTagsConstructor.MembersCountByWorkspaceRole(workspaceId, userWorksp.RoleId));

            await _userWorkspaceRepository.DeleteAsync(userWorksp);
            await _workspaceRepository.SaveChangesAsync();
        }

        public async Task CancelInviteAsync(int id, int workspaceId, string userId)
        {
            var invite = await _inviteUserRepository.GetByKeyAsync(id);

            if (invite == null || invite.IsConfirm != null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.InviteNotFound);
            }

            var specification = new UserWorkspaces.WorkspaceMember(userId, workspaceId);
            var user = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

            if((WorkSpaceRoles)user.RoleId == WorkSpaceRoles.OwnerId || invite.FromUserId ==userId)
            {
                await _inviteUserRepository.DeleteAsync(invite);
                await _inviteUserRepository.SaveChangesAsync();
            }
            else
            {
                throw new HttpException(HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermission);
            }

            await Task.CompletedTask;
        }

        public async Task<List<WorkspaceRoleDTO>> GetAllowedRoles()
        {
            var result = await _userRoleRepository.GetAllAsync();

            return result.Select(x => _mapper.Map<WorkspaceRoleDTO>(x)).ToList();
        }

        public async Task<List<WorkspaceDetailMemberDTO>> GetDetailMemberAsyns(int workspaceId)
        {
            var specification = new UserWorkspaces.WorkspaceMemberList(workspaceId);
            var memberList = await _userWorkspaceRepository.GetListBySpecAsync(specification);

            var memberListToReturn = _mapper.Map<List<WorkspaceDetailMemberDTO>>(memberList);

            return memberListToReturn;
        }

        public async Task SetUsingSprintsAsync(int workspaceId, bool isUseSptints)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);
            workspace.isUseSprints = isUseSptints;

            await _workspaceRepository.UpdateAsync(workspace);
            await _workspaceRepository.SaveChangesAsync();
        }
    }
}
