using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Provis.Core.ApiModels;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.CommentAttachmentEntity;
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
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;

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
        protected readonly IRepository<CommentAttachment> _commentAttachmentRepository;
        protected readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CommentService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Comment> comment,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IRepository<CommentAttachment> commentAttachment,
            IFileService fileService,
            IMapper mapper)
        {
            _userManager = userManager;
            _workspaceRepository = workspace;
            _userWorkspaceRepository = userWorkspace;
            _userRepository = user;
            _taskRepository = task;
            _commentRepository = comment;
            _commentAttachmentRepository = commentAttachment;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<CommentListDTO> AddCommentAsync(CreateCommentDTO commentDTO, string userId)
        {
            Comment comment = new()
            {
                DateOfCreate = DateTime.UtcNow,
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

            var comment = await _commentRepository.GetByKeyAsync(id);

            if (!(comment.UserId == userId || 
                userWorkspace.RoleId == (int)WorkSpaceRoles.OwnerId))
            {
                throw new HttpException(System.Net.HttpStatusCode.Forbidden,
                    "Only cretor or workspace owner can delete comments");
            }

            await _commentRepository.DeleteAsync(comment);
            await _commentRepository.SaveChangesAsync();
        }

        public async Task<List<CommentAttachmentInfoDTO>> GetCommentAttachmentsAsync(int commentId)
        {
            var specification = new CommentAttachments.CommentAttachmentsList(commentId);
            var listAttachments = await _commentAttachmentRepository.GetListBySpecAsync(specification);
            
            var listToReturn = listAttachments.Select(x => _mapper.Map<CommentAttachmentInfoDTO>(x)).ToList();
            var provider = new FileExtensionContentTypeProvider();
            foreach (var item in listToReturn)
            {
                if (provider.TryGetContentType(item.Name, out string contentType))
                {
                    item.ContentType = contentType;
                }
                else
                {
                    throw new HttpException(HttpStatusCode.BadRequest, "Can`t get content type");
                }
            }
            return listToReturn;
        }

        public async Task<DownloadFile> GetCommentAttachmentAsync(int attachmentId)
        {
            var specification = new CommentAttachments.CommentAttachmentInfo(attachmentId);
            var attachment = await _commentAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound, "Attachment not found");

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }

        public async Task DeleteCommentAttachmentAsync(int attachmentId)
        {
            var specification = new CommentAttachments.CommentAttachmentInfo(attachmentId);
            var attachment = await _commentAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound, "Attachment not found");

            if (attachment.AttachmentPath != null)
            {
                await _fileService.DeleteFileAsync(attachment.AttachmentPath);
            }

            await _commentAttachmentRepository.DeleteAsync(attachment);
            await _commentAttachmentRepository.SaveChangesAsync();
        }

        public Task<CommentAttachmentInfoDTO> SendCommentAttachmentsAsync(CommentAttachmentsDTO commentAttachmentsDTO)
        {
            throw new NotImplementedException();
        }

        public Task<DownloadFile> GetCommentAttachmentPreviewAsync(int attachmentId)
        {
            throw new NotImplementedException();
        }
    }
}
