using Provis.Core.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Provis.Core.Entities
{
    public class InviteUser : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public bool? IsConfirm { get; set; }

        public int WorkspaceId { get; set; }

        [ForeignKey("WorkspaceId")]
        public Workspace Workspace { get; set; }

        public string FromUserId { get; set; }

        [ForeignKey("FromUserId")]
        public User FromUser { get; set; }

        public string ToUserId { get; set; }

        [ForeignKey("ToUserId")]
        public User ToUser { get; set; }
    }
}
