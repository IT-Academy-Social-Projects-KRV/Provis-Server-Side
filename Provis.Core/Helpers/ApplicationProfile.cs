﻿using AutoMapper;
using Provis.Core.DTO.userDTO;
using Provis.Core.DTO.workspaceDTO;
using Provis.Core.Entities;

namespace Provis.Core.Helpers
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<WorkspaceCreateDTO, Workspace>();
            
            CreateMap<User, UserPersonalInfoDTO>()
                .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, act => act.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Username, act => act.MapFrom(src => src.UserName));
            
            CreateMap<UserWorkspace, WorkspaceInfoDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.WorkspaceId))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Workspace.Name))
                .ForMember(x => x.Role, act => act.MapFrom(srs => srs.Role.Name));
            
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
        }
    }
}
