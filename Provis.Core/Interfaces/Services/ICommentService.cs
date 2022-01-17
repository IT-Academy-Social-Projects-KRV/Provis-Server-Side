using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICommentService
    {
        Task<List<CommentDTO>> GetComments(int taskId);
        Task
    }
}
