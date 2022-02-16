using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Resources;
using Provis.Core.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class CommentService : ICommentService
    {
        protected readonly UserManager<User> _userManager;
        protected readonly IRepository<User> _userRepository;
        protected readonly IRepository<UserWorkspace> _userWorkspaceRepository;
        protected readonly IRepository<Workspace> _workspaceRepository;
        protected readonly IRepository<WorkspaceTask> _taskRepository;
        protected readonly IRepository<Comment> _commentRepository;
        protected readonly IMapper _mapper;

        public CommentService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Comment> comment,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _userRepository = user;
            _taskRepository = task;
            _commentRepository = comment;
            _mapper = mapper;
        }

        public async Task<CommentListDTO> AddCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            Comment comment = new()
            {
                DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                UserId = userId
            };
            _mapper.Map(commentDTO, comment);

            var commentInfo = await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return _mapper.Map(commentInfo, new CommentListDTO());
        }

        public async Task<List<CommentListDTO>> GetCommentListsAsync(int taskId)
        {
            var specification = new Comments.CommentTask(taskId);
            var commentList = await _commentRepository.GetListBySpecAsync(specification);

            var commentListToReturn = _mapper.Map<List<CommentListDTO>>(commentList);

            return commentListToReturn;
        }

        public async Task EditCommentAsync(EditCommentDTO editComment, string creatorId)
        {
            var comment = await _commentRepository.GetByKeyAsync(editComment.CommentId);
            comment.CommentNullChecking();

            if (comment.UserId != creatorId)
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                    ErrorMessages.NotPermissionEditComment);
            }

            comment.DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
            comment.CommentText = editComment.CommentText;

            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int id, string userId, int workspaceId)
        {
            var comment = await _commentRepository.GetByKeyAsync(id);
            comment.CommentNullChecking();

            var specification = new UserWorkspaces.WorkspaceMember(userId, workspaceId);
            var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

            if (!(comment.UserId == userId ||
                userWorkspace.RoleId == (int)WorkSpaceRoles.OwnerId))
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                   ErrorMessages.NotPermissionDeleteComment);
            }

            await _commentRepository.DeleteAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }
    }
}
