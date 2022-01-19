using Provis.Core.Entities.InviteUserEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;

namespace Provis.Core.Helpers.Mails
{
    public static class ExtensionMethods
    {
        public static void UserNullChecking(this User user)
        {
            if (user == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User with this Id doesn't exist");
            }
        }

        public static void WorkspaceNullChecking(this Workspace workspace)
        {
            if (workspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Workspace with this Id doesn't exist");
            }
        }

        public static void UserWorkspaceNullChecking(this UserWorkspace userWorkspace)
        {
            if (userWorkspace == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "User doesn't exist in this workspace");
            }
        }

        public static void InviteNullChecking(this InviteUser userInvite)
        {
            if (userInvite == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Invite with this Id doesn't exist");
            }
        }

        public static void TaskNullChecking(this WorkspaceTask workspaceTask)
        {
            if (workspaceTask == null)
            {
                throw new HttpException(System.Net.HttpStatusCode.NotFound, "Task with this Id doesn't exist");
            }
        }
    }
}
