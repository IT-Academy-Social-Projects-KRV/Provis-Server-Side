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
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
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

        public async Task AddCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            Comment comment = new()
            {
                DateOfCreate = DateTime.UtcNow,
                UserId = userId
            };
            _mapper.Map(commentDTO, comment);

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
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

            if (comment.UserId != creatorId)
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                    "Only creator can edit his comment");
            }

            comment.DateOfCreate = DateTime.UtcNow;
            comment.CommentText = editComment.CommentText;

            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(int id, string userId, int workspaceId)
        {
            var specification = new UserWorkspaces.WorkspaceMember(userId, workspaceId);
            var userWorkspace = await _userWorkspaceRepository.GetFirstBySpecAsync(specification);

            var taskSpecification = new WorkspaceTasks.TaskByComments(id);
            var task = await _taskRepository.GetFirstBySpecAsync(taskSpecification);

            var comment = await _commentRepository.GetByKeyAsync(id);

            if (!(comment.UserId == userId || 
                userWorkspace.RoleId == (int)WorkSpaceRoles.OwnerId || 
                task.TaskCreatorId == userId))
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                    "Only cretor or workspace owner can delete comments");
            }

            await _commentRepository.DeleteAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }
    }
}
