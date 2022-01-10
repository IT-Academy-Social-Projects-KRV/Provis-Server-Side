using Provis.Core.Entities.UserTaskEntity;
using Provis.Core.Interfaces;
using System.Collections.Generic;

namespace Provis.Core.Entities.UserRoleTagEntity
{
    public class UserRoleTag : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}
