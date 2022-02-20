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
using Provis.Core.Helpers.Mails;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Resources;
using Provis.Core.Roles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using System.Net;
using Provis.Core.Helpers;
using Microsoft.Extensions.Options;

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
        private readonly IOptions<AttachmentSettings> _attachmentSettings;
        private readonly IOptions<ImageSettings> _imageSettings;
        private readonly IFileService _fileService;

        public CommentService(IRepository<User> user,
            IRepository<WorkspaceTask> task,
            IRepository<Comment> comment,
            IRepository<UserWorkspace> userWorkspace,
            IRepository<Workspace> workspace,
            UserManager<User> userManager,
            IRepository<CommentAttachment> commentAttachment,
            IFileService fileService,
            IOptions<AttachmentSettings> attachmentSettings,
            IOptions<ImageSettings> imageSettings,
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
            _attachmentSettings = attachmentSettings;
            _imageSettings = imageSettings;
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
                    throw new HttpException(HttpStatusCode.BadRequest, ErrorMessages.CannotGetFileContentType);
                }
            }
            return listToReturn;
        }

        public async Task<DownloadFile> GetCommentAttachmentAsync(int attachmentId)
        {
            var specification = new CommentAttachments.CommentAttachmentInfo(attachmentId);
            var attachment = await _commentAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound, ErrorMessages.AttachmentNotFound);

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }

        public async Task DeleteCommentAttachmentAsync(int attachmentId)
        {
            var specification = new CommentAttachments.CommentAttachmentInfo(attachmentId);
            var attachment = await _commentAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound, ErrorMessages.AttachmentNotFound);

            if (attachment.AttachmentPath != null)
            {
                await _fileService.DeleteFileAsync(attachment.AttachmentPath);
            }

            await _commentAttachmentRepository.DeleteAsync(attachment);
            await _commentAttachmentRepository.SaveChangesAsync();
        }

        public async Task<CommentAttachmentInfoDTO> SendCommentAttachmentsAsync(CommentAttachmentsDTO commentAttachmentsDTO)
        {
            var specification = new CommentAttachments.CommentAttachmentsList(commentAttachmentsDTO.CommentId);
            var result = await _commentAttachmentRepository.GetListBySpecAsync(specification);

            var listAttachmentsAlready = result.ToList();

            if (listAttachmentsAlready.Count == _attachmentSettings.Value.MaxCount)
                throw new HttpException(HttpStatusCode.BadRequest,
                    $"You have exceeded limit of {_attachmentSettings.Value.MaxCount} attachments");

            var file = commentAttachmentsDTO.Attachment;

            string newPath = await _fileService.AddFileAsync(file.OpenReadStream(),
                _attachmentSettings.Value.Path, file.FileName);

            CommentAttachment commentAttachment = new()
            {
                AttachmentPath = newPath,
                CommentId = commentAttachmentsDTO.CommentId
            };

            await _commentAttachmentRepository.AddAsync(commentAttachment);

            await _commentAttachmentRepository.SaveChangesAsync();

            var res = _mapper.Map<CommentAttachmentInfoDTO>(commentAttachment);
            res.ContentType = commentAttachmentsDTO.Attachment.ContentType;

            return res;
        }

        public async Task<DownloadFile> GetCommentAttachmentPreviewAsync(int attachmentId)
        {
            var specification = new CommentAttachments.CommentAttachmentInfo(attachmentId);
            var attachment = await _commentAttachmentRepository.GetFirstBySpecAsync(specification);

            _ = attachment ?? throw new HttpException(HttpStatusCode.NotFound,
                ErrorMessages.AttachmentNotFound);

            var provider = new FileExtensionContentTypeProvider();

            if (provider.TryGetContentType(attachment.AttachmentPath, out string contentType) &&
                !contentType.StartsWith(_imageSettings.Value.Type))
            {
                throw new HttpException(HttpStatusCode.BadRequest,
                    ErrorMessages.NotPreview);
            }

            var file = await _fileService.GetFileAsync(attachment.AttachmentPath);

            return file;
        }
    }
}
