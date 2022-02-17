using Ardalis.Specification;
using System;
using System.Linq;

namespace Provis.Core.Entities.CommentAttachmentEntity
{
    public class CommentAttachments
    {
        internal class CommentAttachmentsList : Specification<CommentAttachment>
        {
            public CommentAttachmentsList(int commentId)
            {
                Query
                    .Where(x => x.CommentId == commentId)
                    .OrderBy(x => x.AttachmentPath);
            }
        }
        internal class CommentAttachmentInfo : Specification<CommentAttachment>
        {
            public CommentAttachmentInfo(int attachmentId)
            {
                Query
                    .Where(x => x.Id == attachmentId);
            }
        }
    }
}
