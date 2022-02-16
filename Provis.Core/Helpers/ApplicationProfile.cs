using AutoMapper;
using Azure.Storage.Blobs.Models;
using Provis.Core.ApiModels;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.DTO.UserDTO;
using Provis.Core.DTO.WorkspaceDTO;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.RoleEntity;
using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserRoleTagEntity;
using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Entities.WorkspaceTaskAttachmentEntity;
using Provis.Core.Entities.CommentAttachmentEntity;
using System.IO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.DTO.CommentsDTO;
using System;
using Provis.Core.Entities.EventEntity;
using Provis.Core.DTO.CalendarDTO;

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
                .ForMember(x => x.Role, act => act.MapFrom(srs => srs.RoleId));

            CreateMap<Workspace, WorkspaceDescriptionDTO>();

            CreateMap<InviteUser, UserInviteInfoDTO > ()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
                .ForMember(x => x.Date, act => act.MapFrom(srs => srs.Date))
                .ForMember(x => x.IsConfirm, act => act.MapFrom(srs => srs.IsConfirm))
                .ForMember(x => x.WorkspaceName, act => act.MapFrom(srs => srs.Workspace.Name))
                .ForMember(x => x.FromUserName, act => act.MapFrom(srs => srs.FromUser.Name))
                .ForMember(x => x.ToUserId, act => act.MapFrom(srs => srs.ToUserId));

            CreateMap<UserWorkspace, WorkspaceChangeRoleDTO>()
                .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId))
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.RoleId, act => act.MapFrom(srs => srs.RoleId));

            CreateMap<UserWorkspace, WorkspaceMemberDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.UserName, act => act.MapFrom(srs => srs.User.UserName))
                .ForMember(x => x.Role, act => act.MapFrom(srs => srs.RoleId));

            CreateMap<InviteUser, WorkspaceInviteInfoDTO>()
                .ForMember(x => x.Date, act => act.MapFrom(srs => srs.Date))
                .ForMember(x => x.FromUserName, act => act.MapFrom(srs => srs.FromUser.UserName))
                .ForMember(x => x.ToUserName, act => act.MapFrom(srs => srs.ToUser.UserName))
                .ForMember(x => x.ToUserEmail, act => act.MapFrom(srs => srs.ToUser.Email))
                .ForMember(x => x.InviteId, act => act.MapFrom(srs => srs.Id));

            CreateMap<TaskCreateDTO, WorkspaceTask>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Description, act => act.MapFrom(srs => srs.Description))
                .ForMember(x => x.DateOfEnd, act => act.MapFrom(srs => srs.DateOfEnd))
                .ForMember(x => x.StatusId, act => act.MapFrom(srs => srs.StatusId))
                .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId))
                .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.StoryPoints));

            CreateMap<UserTask, TaskDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.TaskId))
                .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.Task.DateOfEnd))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Task.Name))
                .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.Task.StoryPoints))
                .ForMember(x => x.WorkerRoleId, act => act.MapFrom(srs => srs.UserRoleTagId));

            CreateMap<Tuple<int, UserTask, int, int, string>, TaskDTO>()
               .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Item2.Task.Id))
               .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.Item2.Task.DateOfEnd))
               .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Item2.Task.Name))
               .ForMember(x => x.WorkerRoleId, act => act.MapFrom(srs => srs.Item2.UserRoleTagId))
               .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.Item2.Task.StoryPoints))
               .ForMember(x => x.CommentCount, act => act.MapFrom(srs => srs.Item3))
               .ForMember(x => x.MemberCount, act => act.MapFrom(src => src.Item4))
               .ForMember(x => x.CreatorUsername, act => act.MapFrom(src => src.Item5));

            CreateMap<WorkspaceTask, TaskDTO>()
               .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
               .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.DateOfEnd))
               .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
               .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.StoryPoints));

            CreateMap<Tuple<int, WorkspaceTask, int, int, string>, TaskDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Item2.Id))
                .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.Item2.DateOfEnd))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Item2.Name))
                .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.Item2.StoryPoints))
                .ForMember(x => x.CommentCount, act => act.MapFrom(srs => srs.Item3))
                .ForMember(x => x.MemberCount, act => act.MapFrom(src => src.Item4))
                .ForMember(x => x.CreatorUsername, act => act.MapFrom(src => src.Item5));

            CreateMap<UserChangeInfoDTO, User>();

            CreateMap<BlobDownloadInfo, DownloadFile>()
                .ForMember(x => x.ContentType, act => act.MapFrom(srs => srs.Details.ContentType))
                .ForMember(x => x.Content, act => act.MapFrom(srs => srs.Content));

            CreateMap<UserRoleTag, TaskRoleDTO>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Id,  act => act.MapFrom(srs => srs.Id));

            CreateMap<Status, TaskStatusDTO>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id));

            CreateMap<Role, WorkspaceRoleDTO>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id));

            CreateMap<TaskChangeInfoDTO, WorkspaceTask>()
              .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
              .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId))
              .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
              .ForMember(x => x.Description, act => act.MapFrom(srs => srs.Description))
              .ForMember(x => x.DateOfEnd, act => act.MapFrom(srs => srs.Deadline))
              .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.StoryPoints));

            CreateMap<StatusHistory, TaskStatusHistoryDTO>()
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.UserName, act => act.MapFrom(srs => srs.User.UserName))
                .ForMember(x => x.StatusId, act => act.MapFrom(srs => srs.StatusId))
                .ForMember(x => x.Status, act => act.MapFrom(srs => srs.Status.Name))
                .ForMember(x => x.DateOfChange, act => act.MapFrom(srs => srs.DateOfChange));

            CreateMap<UserWorkspace, WorkspaceDetailMemberDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.User.Name))
                .ForMember(x => x.Surname, act => act.MapFrom(srs => srs.User.Surname))
                .ForMember(x => x.UserName, act => act.MapFrom(srs => srs.User.UserName))
                .ForMember(x => x.Email, act => act.MapFrom(srs => srs.User.Email))
                .ForMember(x => x.Role, act => act.MapFrom(srs => srs.RoleId));

            CreateMap<WorkspaceTask, TaskInfoDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
                .ForMember(x => x.Name, act => act.MapFrom(srs => srs.Name))
                .ForMember(x => x.Description, act => act.MapFrom(srs => srs.Description))
                .ForMember(x => x.Deadline, act => act.MapFrom(srs => srs.DateOfEnd))
                .ForMember(x => x.StatusId, act => act.MapFrom(srs => srs.StatusId))
                .ForMember(x => x.StoryPoints, act => act.MapFrom(srs => srs.StoryPoints))
                .ForMember(x => x.AssignedUsers, act => act.MapFrom(srs => srs.UserTasks));

            CreateMap<WorkspaceTaskAttachment, TaskAttachmentInfoDTO>()
                .ForMember(x => x.Name, act => act.MapFrom(srs=> Path.GetFileName(srs.AttachmentPath)));

            CreateMap<CommentAttachment, CommentAttachmentInfoDTO>()
                .ForMember(x => x.Name, act => act.MapFrom(srs => Path.GetFileName(srs.AttachmentPath)));

            CreateMap<UserTask, TaskAssignedUsersDTO>()
                .ForMember(x => x.UserName, act => act.MapFrom(srs => srs.User.UserName))
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.RoleTagId, act => act.MapFrom(srs => srs.UserRoleTagId));

            CreateMap<Comment, CommentListDTO>()
                .ForMember(x => x.Id, act => act.MapFrom(srs => srs.Id))
                .ForMember(x => x.CommentText, act => act.MapFrom(srs => srs.CommentText))
                .ForMember(x => x.DateTime, act => act.MapFrom(srs => srs.DateOfCreate))
                .ForMember(x => x.TaskId, act => act.MapFrom(srs => srs.TaskId))
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.UserId))
                .ForMember(x => x.UserName, act => act.MapFrom(srs => srs.User.UserName));

            CreateMap<CreateCommentDTO, Comment>()
                .ForMember(x => x.CommentText, act => act.MapFrom(srs => srs.CommentText))
                .ForMember(x => x.TaskId, act => act.MapFrom(srs => srs.TaskId));

            CreateMap<TaskAssignDTO, UserTask>()
                .ForMember(x => x.TaskId, act => act.MapFrom(srs => srs.Id))
                .ForMember(x => x.UserId, act => act.MapFrom(srs => srs.AssignedUser.UserId))
                .ForMember(x => x.UserRoleTagId, act => act.MapFrom(srs => srs.AssignedUser.RoleTagId));

            CreateMap<EventCreateDTO, Event>()
                .ForMember(x => x.EventName, act => act.MapFrom(srs => srs.EventName))
                .ForMember(x => x.EventMessage, act => act.MapFrom(srs => srs.EventMessage))
                .ForMember(x => x.DateOfStart, act => act.MapFrom(srs => srs.DateOfStart))
                .ForMember(x => x.DateOfEnd, act => act.MapFrom(srs => srs.DateOfEnd))
                .ForMember(x => x.WorkspaceId, act => act.MapFrom(srs => srs.WorkspaceId));
        }
    }
}
