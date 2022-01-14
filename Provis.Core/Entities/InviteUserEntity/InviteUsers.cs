using Ardalis.Specification;

namespace Provis.Core.Entities.InviteUserEntity
{
    public class InviteUsers
    {
        internal class InviteList : Specification<InviteUser>
        {
            public InviteList(string userId)
            {
                Query
                    .Where(u => u.ToUserId == userId)
                    .Include(w => w.Workspace)
                    .Include(u => u.FromUser)
                    .OrderBy(d => d.Date);
            }

            public InviteList(int workspaceId)
            {
                Query
                    .Include(x => x.FromUser)
                    .Include(x => x.ToUser)
                    .Where(x => x.WorkspaceId == workspaceId && x.IsConfirm == null);
            }

            public InviteList(string inviteUserId, int workspaceId)
            {
                Query
                    .Include(x => x.FromUser)
                    .Include(x => x.ToUser)
                    .Where(x => x.ToUserId == inviteUserId &&
                        x.WorkspaceId == workspaceId);
            }
        }

        internal class ActiveInvites : Specification<InviteUser>
        {
            public ActiveInvites(string userId)
            {
                Query
                    .Where(u => u.ToUserId == userId && u.IsConfirm == null);
            }
        }
    }
}
