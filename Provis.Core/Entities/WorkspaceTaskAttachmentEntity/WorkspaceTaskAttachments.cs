using Ardalis.Specification;
using System;
using System.Linq;

namespace Provis.Core.Entities.WorkspaceTaskAttachmentEntity
{
    public class WorkspaceTaskAttachments
    {
        internal class TaskAttachments : Specification<WorkspaceTaskAttachment>
        {
            public TaskAttachments(int taskId)
            {
                Query
                    .Where(x => x.TaskId == taskId)
                    .OrderBy(x => x.AttachmentPath);
            }
        }
        internal class TaskAttachmentInfo : Specification<WorkspaceTaskAttachment>
        {
            public TaskAttachmentInfo(int attachmentId)
            {
                Query
                    .Where(x => x.Id == attachmentId);                    
            }
        }
    }
}
