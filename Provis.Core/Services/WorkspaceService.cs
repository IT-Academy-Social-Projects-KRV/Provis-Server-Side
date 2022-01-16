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
        protected readonly IMapper _mapper;
        protected readonly RoleAccess _roleAccess;
        protected readonly ITemplateService _templateService;
        protected readonly ClientUrl _clientUrl;

        public WorkspaceService(IRepository<User> user,
            UserManager<User> userManager,
            IRepository<Workspace> workspace,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<InviteUser> inviteUser,
            IRepository<Role> userRoleRepository,
            IEmailSenderService emailSenderService,
            IMapper mapper,
            RoleAccess roleAccess,
            ITemplateService templateService,
            IOptions<ClientUrl> options)
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
            _clientUrl = options.Value;
        }
        public async Task CreateWorkspaceAsync(WorkspaceCreateDTO workspaceDTO, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            Workspace workspace = new Workspace()
            {
                DateOfCreate = DateTime.UtcNow,
                Name = workspaceDTO.Name,
                Description = workspaceDTO.Description
            };
            await _workspaceRepository.AddAsync(workspace);
            await _workspaceRepository.SaveChangesAsync();

            UserWorkspace userWorkspace = new UserWorkspace()
            {
                UserId = user.Id,
                WorkspaceId = workspace.Id,
                RoleId = (int)WorkSpaceRoles.OwnerId
            };
            await _userWorkspaceRepository.AddAsync(userWorkspace);
            await _userWorkspaceRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task UpdateWorkspaceAsync(WorkspaceUpdateDTO workspaceUpdateDTO, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            _ = user ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "User with Id not exist");

            var workspaceRec = await _workspaceRepository.GetByKeyAsync(workspaceUpdateDTO.WorkspaceId);

            _ = workspaceRec ?? throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Workspace with with Id not found");

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
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You cannot send invite to your account");
            }

            var inviteUser = await _userManager.FindByEmailAsync(inviteDTO.UserEmail);

            if (inviteUser == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with this Email not exist");
            }

            var workspace = await _workspaceRepository.GetByKeyAsync(inviteDTO.WorkspaceId);

            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Not found this workspace");
            }

            var inviteUserListSpecification = new InviteUsers.InviteList(inviteUser.Id, workspace.Id);
            if (await _inviteUserRepository.AnyBySpecAsync(inviteUserListSpecification, x=>x.IsConfirm == null))
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                    "User already has invite, wait for a answer");
            }

            var userWorkspaceInviteSpecification = new UserWorkspaces.WorkspaceMember(inviteUser.Id, workspace.Id);
            if (await _inviteUserRepository.AnyBySpecAsync(inviteUserListSpecification, x => x.IsConfirm.Value == true)
                && await _userWorkspaceRepository.GetFirstBySpecAsync(userWorkspaceInviteSpecification) != null)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest,
                    "This user already accepted your invite");
            }

            InviteUser user = new InviteUser
            {
                Date = DateTime.UtcNow,
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
                        Uri = _clientUrl.ApplicationUrl
                    })
            });

            await Task.CompletedTask;
        }

        public async Task DenyInviteAsync(int id, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var inviteUserRec = await _inviteUserRepository.GetByKeyAsync(id);

            if (inviteUserRec == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Invite with with Id not found");
            }

            if (inviteUserRec.ToUserId != userid)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You cannot deny this invite");
            }

            inviteUserRec.IsConfirm ??= false;

            await _inviteUserRepository.SaveChangesAsync();

            await Task.CompletedTask;
        }

        public async Task<List<WorkspaceInfoDTO>> GetWorkspaceListAsync(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with Id not exist");
            }

            var specification = new UserWorkspaces.WorkspaceList(userid);
            var listWorkspace = await _userWorkspaceRepository.GetListBySpecAsync(specification);

            var listWorkspaceToReturn = _mapper.Map<List<WorkspaceInfoDTO>>(listWorkspace);

            return listWorkspaceToReturn;
        }

        public async Task AcceptInviteAsync(int inviteId, string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with this Id doesn't exist");
            }

            var inviteUserRec = await _inviteUserRepository.GetByKeyAsync(inviteId);

            if (inviteUserRec == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Invite with this Id not found");
            }

            if (inviteUserRec.ToUserId != userid)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "That's not yours invite!");
            }

            if (inviteUserRec.IsConfirm == true)
            {
                throw new HttpException(System.Net.HttpStatusCode.BadRequest, "You have already confirmed your invitation!");
            }

            inviteUserRec.IsConfirm = true;

            UserWorkspace userWorkspace = new()
            {
                UserId = user.Id,
                WorkspaceId = inviteUserRec.WorkspaceId,
                RoleId = (int)WorkSpaceRoles.MemberId
            };

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

            if (modifier.UserId == null)
                throw new HttpException(
                    System.Net.HttpStatusCode.Forbidden,
                    "User with this Id doesn't exist");

            if (target.UserId == null)
                throw new HttpException(
                    System.Net.HttpStatusCode.NotFound,
                    "User with this Id doesn't exist");

            var roleId = (WorkSpaceRoles)modifier.RoleId;


            if (_roleAccess.RolesAccess.ContainsKey(roleId) &&
                _roleAccess.RolesAccess[roleId]
                    .Any(p => p == (WorkSpaceRoles)target.RoleId) &&
                _roleAccess.RolesAccess[roleId]
                    .Any(p => p == (WorkSpaceRoles)userChangeRole.RoleId)
                )
            {
                target.RoleId = userChangeRole.RoleId;
                await _userWorkspaceRepository.SaveChangesAsync();
                return _mapper.Map<WorkspaceChangeRoleDTO>(target);
            }
            else
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden, "You haven't permission to change this Role");
            }
        }

        public async Task<WorkspaceInfoDTO> GetWorkspaceInfoAsync(int workspaceId, string userId)
        {
            var specification = new UserWorkspaces.WorkspaceInfo(userId, workspaceId);
            var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

            if (userWorkspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "Workspace with this Id doesn't exist");
            }

            var workspace = _mapper.Map<WorkspaceInfoDTO>(userWorkspace);

            return workspace;
        }

        public async Task<WorkspaceDescriptionDTO> GetWorkspaceDescriptionAsync(int workspaceId)
        {
            var workspace = await _workspaceRepository.GetByKeyAsync(workspaceId);

            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "Workspace with this Id doesn't exist or you hasn't permissions");
            }

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

            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                    "Workspace with current Id not found");
            }

            var specification = new UserWorkspaces.WorkspaceMemberList(workspaceId);
            var workspaceMembers = await _userWorkspaceRepository.GetListBySpecAsync(specification);

            var workspaceMembersToReturn = _mapper.Map<List<WorkspaceMemberDTO>>(workspaceMembers);

            return workspaceMembersToReturn;
        }

        public async Task DeleteFromWorkspaceAsync(int workspaceId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var specification = new UserWorkspaces.WorkspaceMember(userId, workspaceId);
            var userWorksp = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

            var userTasks = user.UserTasks.Where(o => o.Task.WorkspaceId == workspaceId);

            if (userTasks != null)
            {
                foreach (var userTask in userTasks)
                {
                    userTask.IsUserDeleted = true;
                }
            }

            await _userWorkspaceRepository.DeleteAsync(userWorksp);
            await _workspaceRepository.SaveChangesAsync();
        }

        public async Task CancelInviteAsync(int id, int workspaceId, string userId)
        {
            var invite = await _inviteUserRepository.GetByKeyAsync(id);

            if (invite == null || invite.IsConfirm != null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound,
                "Invite with Id not found or it already answered");
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
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                            "You don't have permission to do this");
            }

            await Task.CompletedTask;
        }

        public async Task<List<WorkspaceRoleDTO>> GetAllowedRoles()
        {
            var result = await _userRoleRepository.GetAllAsync();

            return result.Select(x => _mapper.Map<WorkspaceRoleDTO>(x)).ToList();
        }
    }
}
