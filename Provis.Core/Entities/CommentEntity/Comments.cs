using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Provis.Core.Entities.CommentEntity
{
    public class Comments
    {
        internal class CommentTask : Specification<Comment>
        {
            public CommentTask(int taskId)
            {
                Query
                    .Include(x => x.User)
                    .Where(x => x.TaskId == taskId);
            }
        }
    }
}
