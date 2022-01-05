using AutoMapper;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;

namespace Provis.Core.Helpers
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<WorkspaceCreateDTO, Workspace>();

            CreateMap<WorkspaceUpdateDTO, Workspace>()
                .ForMember(dest => dest.Id, act => act.MapFrom(src => src.WorkspaceId))
                .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Description))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name));

            CreateMap<User, UserPersonalInfoDTO>()
                .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, act => act.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Username, act => act.MapFrom(src => src.UserName));

            CreateMap<UserWorkspace, WorkspaceInfoDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.WorkspaceId))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Workspace.Name))
                .ForMember(x => x.Role, act => act.MapFrom(srs => srs.Role.Name))
                .ForMember(x => x.Description, act => act.MapFrom(srs => srs.Workspace.Description));
            
            CreateMap<InviteUser, UserInviteInfoDTO > ()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
                .ForMember(x => x.Date, act => act.MapFrom(srs => srs.Date))
                .ForMember(x => x.IsConfirm, act => act.MapFrom(srs => srs.IsConfirm))
                .ForMember(x => x.WorkspaceName, act => act.MapFrom(srs => srs.Workspace.Name))
                .ForMember(x => x.FromUserName, act => act.MapFrom(srs => srs.FromUser.Name))
                .ForMember(x => x.ToUserId, act => act.MapFrom(srs => srs.ToUserId));

            CreateMap<UserWorkspace, ChangeRoleDTO>()
                .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId))
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.RoleId, act => act.MapFrom(srs => srs.RoleId));  

            CreateMap<InviteUser, WorkspaceInviteInfoDTO>()
                .ForMember(x => x.Date, act => act.MapFrom(srs => srs.Date))
                .ForMember(x => x.FromUserName, act => act.MapFrom(srs => srs.FromUser.UserName))
                .ForMember(x => x.ToUserName, act => act.MapFrom(srs => srs.ToUser.UserName))
                .ForMember(x => x.ToUserEmail, act => act.MapFrom(srs => srs.ToUser.Email))
                .ForMember(x => x.InviteId, act => act.MapFrom(srs => srs.Id));

            CreateMap<TaskCreateDTO, Task>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Description, act => act.MapFrom(srs => srs.Description))
                .ForMember(x => x.DateOfEnd, act => act.MapFrom(srs => srs.DateOfEnd))
                .ForMember(x => x.StatusId, act => act.MapFrom(srs => srs.StatusId))
                .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId));

            CreateMap<UserTask, TaskDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.TaskId))
                .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.Task.DateOfEnd))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Task.Name))
                .ForMember(x => x.WorkerRoleId, act => act.MapFrom(srs => srs.UserRoleTagId));

            CreateMap<UserChangeInfoDTO, User>();
        }
    }
}
