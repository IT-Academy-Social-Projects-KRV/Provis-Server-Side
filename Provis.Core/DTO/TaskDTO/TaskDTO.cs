using System;

namespace Provis.Core.DTO.TaskDTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Deadline { get; set; }
        public int? StoryPoints { get; set; }
        public int? WorkerRoleId { get; set; }
        public int CommentCount { get; set; }
        public int MemberCount {  get; set; }
        public string CreatorUsername { get; set; }
    }
}
