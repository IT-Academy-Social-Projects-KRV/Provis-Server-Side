using System;

namespace Provis.Core.DTO.SprintDTO
{
    public class SprintDetailInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? DateOfStart { get; set; }
        public DateTimeOffset? DateOfEnd { get; set; }
    }
}
