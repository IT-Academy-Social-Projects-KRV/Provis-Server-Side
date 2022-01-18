﻿using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICommentService
    {
        Task<List<CommentListDTO>> GetCommentsAsync(int taskId);
        Task CommentAsync(CreateCommentDTO commentDTO, string userId);
        Task EditCommentAsync(EditCommentDTO editCommentDTO, string userID);
        Task DeleteCommentAsync(int id, string userId, int workspaceId);
    }
}