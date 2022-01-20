using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICommentService
    {
        Task<List<CommentListDTO>> GetCommentListsAsync(int taskId);
        Task AddCommentAsync(CommentCreateDTO commentDTO, string userId);
        Task EditCommentAsync(CommentEditDTO editCommentDTO, string userID);
        Task DeleteCommentAsync(int id, string userId, int workspaceId);
    }
}
