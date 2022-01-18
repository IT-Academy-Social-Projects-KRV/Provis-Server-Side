using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class CommentService : ICommentService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<WorkspaceTask> _taskRepository;
        protected readonly IRepository<Comment> _commentRepository;
        protected readonly IMapper _mapper;

        public CommentService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Comment> comment,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = user;
            _taskRepository = task;
            _commentRepository = comment;
            _mapper = mapper;
        }

        public async Task CommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            Comment comment = new()
            {
                CommentText = commentDTO.CommentText,
                DateOfCreate = DateTime.UtcNow,
                TaskId = commentDTO.TaskId,
                UserId = userId
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task<List<CommentListDTO>> GetCommentsAsync(int taskId)
        {
            var specification = new Comments.CommentTask(taskId);
            var commentList = await _commentRepository.GetListBySpecAsync(specification);

            var listCommentsToReturn = _mapper.Map<List<CommentListDTO>>(commentList);

            return listCommentsToReturn;
        }

        public async Task EditCommentAsync(EditCommentDTO editComment, string creatorId)
        {
            var comment = await _commentRepository.GetByKeyAsync(editComment.CommentId);

            if (comment.UserId != creatorId)
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                    "Only creator can edit his comment");
            }

            comment.DateOfCreate = DateTime.Now;
            comment.CommentText = editComment.CommentText;

            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }
    }
}
