using Provis.Core.Entities.EventEntity;
using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.SprintEntity;
using Provis.Core.DTO.TaskDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserEventsEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Resources;
using System.Net;

namespace Provis.Core.Helpers.Mails
{
    public static class ExtensionMethods
    {
        public static void UserNullChecking(this User user)
        {
            if (user == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.UserNotFound);
            }
        }

        public static void WorkspaceNullChecking(this Workspace workspace)
        {
            if (workspace == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.WorkspaceNotFound);
            }
        }

        public static void UserWorkspaceNullChecking(this UserWorkspace userWorkspace)
        {
            if (userWorkspace == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.UserNotMember);
            }
        }

        public static void InviteNullChecking(this InviteUser userInvite)
        {
            if (userInvite == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.InviteNotFound);
            }
        }

        public static void TaskNullChecking(this WorkspaceTask workspaceTask)
        {
            if (workspaceTask == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.TaskNotFound);
            }
        }

        public static void SprintNullChecking(this Sprint sprint)
        {
            if (sprint == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.TaskNotFound);
            }
        }

        public static bool TaskDataIsUpdated(this WorkspaceTask workspaceTask,
            TaskChangeInfoDTO taskChangeInfoDTO)
        {
            return workspaceTask.Name != taskChangeInfoDTO.Name
               || workspaceTask.Description != taskChangeInfoDTO.Description
               || workspaceTask.DateOfEnd != taskChangeInfoDTO.Deadline
               || workspaceTask.StoryPoints != taskChangeInfoDTO.StoryPoints;
        }

        public static void CommentNullChecking(this Comment comment)
        {
            if(comment == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.CommentNotFound);
            }
        }

        public static void EventNullChecking(this Event events)
        {
            if (events == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.EventNotFound);
            }
        }

        public static void UserEventNullChecking(this UserEvent userEvent)
        {
            if (userEvent == null)
            {
                throw new HttpException(HttpStatusCode.NotFound,
                    ErrorMessages.EventNotFound);
            }
        }
    }
}
