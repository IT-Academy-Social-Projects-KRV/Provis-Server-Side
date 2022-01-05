using Provis.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provis.Core.Entities
{
    public class UserTask : IBaseEntity
    {
        public int TaskId { get; set; }

        public Task Task { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public bool IsUserDeleted { get; set; }

        public int UserRoleTagId { get; set; }

        public UserRoleTag UserRoleTag { get; set; }
    }
}
