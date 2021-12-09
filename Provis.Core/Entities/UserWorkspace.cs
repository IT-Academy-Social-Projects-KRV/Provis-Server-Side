using Provis.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provis.Core.Entities
{
    public class UserWorkspace : IBaseEntity
    {
        public int WorkspaceId { get; set; }

        public Workspace Workspace { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }
}
