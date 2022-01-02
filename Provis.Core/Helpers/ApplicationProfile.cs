using AutoMapper;
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

            CreateMap<InviteUser, WorkspaceInviteInfoDTO>()
                .ForMember(x => x.Date, act => act.MapFrom(srs => srs.Date))
                .ForMember(x => x.FromUserName, act => act.MapFrom(srs => srs.FromUser.Name))
                .ForMember(x => x.ToUserName, act => act.MapFrom(srs => srs.ToUser.Name))
                .ForMember(x => x.ToUserEmail, act => act.MapFrom(srs => srs.ToUser.Email));

            CreateMap<UserChangeInfoDTO, User>();
        }
    }
}
