using Provis.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Provis.Core.Entities
{
    public class UserRoleTag : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }
}
