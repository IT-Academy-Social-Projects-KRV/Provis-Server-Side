using AutoMapper;
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
        }
    }
}
