using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.CommentDTO;
using Provis.Core.DTO.CommentsDTO;
using Provis.Core.Entities.CommentEntity;
using Provis.Core.Entities.UserEntity;
using Provis.Core.Entities.UserWorkspaceEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Services;
using Provis.UnitTests.Base.TestData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Specification;
using Provis.UnitTests.Base;
using Provis.UnitTests.Resources;
using System.Net;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class CommentServiceTests
    {
        protected CommentService _commentService;

        protected Mock<UserManager<User>> _userManagerMock;
        protected Mock<IRepository<User>> _userRepositoryMock;
        protected Mock<IRepository<UserWorkspace>> _userWorkspaceRepositoryMock;
        protected Mock<IRepository<Workspace>> _workspaceRepositoryMock;
        protected Mock<IRepository<WorkspaceTask>> _taskRepositoryMock;
        protected Mock<IRepository<Comment>> _commentRepositoryMock;
        protected Mock<IMapper> _mapperMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userManagerMock = UserManagerMock.GetUserManager<User>();
            _workspaceRepositoryMock = new Mock<IRepository<Workspace>>();
            _userWorkspaceRepositoryMock = new Mock<IRepository<UserWorkspace>>();
            _userRepositoryMock = new Mock<IRepository<User>>();
            _taskRepositoryMock = new Mock<IRepository<WorkspaceTask>>();
            _commentRepositoryMock = new Mock<IRepository<Comment>>();
            _mapperMock = new Mock<IMapper>();

            _commentService = new CommentService(
                _userRepositoryMock.Object,
                _taskRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _userWorkspaceRepositoryMock.Object,
                _workspaceRepositoryMock.Object,
                _userManagerMock.Object,
                _mapperMock.Object);
        }

        [Test]
        [TestCase("1")]
        public async Task AddCommentAsync_ValidComment_ReturnCommentListDTO(string userId)
        {
            Comment comment = new()
            {
                DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero),
                UserId = userId
            };
            CreateCommentDTO createCommentDTO = new()
            {
                CommentText = "Text",
                TaskId = 1,
                WorkspaceId = 1
            };
            
            comment.CommentText = createCommentDTO.CommentText;
            comment.TaskId = createCommentDTO.TaskId;

            _mapperMock.Setup(x => 
                x.Map(It.IsAny<CreateCommentDTO>(), It.IsAny<Comment>()))
                    .Returns(comment);
            SetupCommentAddAsync(comment);
            SetupCommentSaveChangesAsync();

            comment.Id = 1;
            comment.User = new User() { UserName = "Username1" };

            CommentListDTO commentListDTO = new()
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                DateTime = comment.DateOfCreate,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserName = comment.User.UserName
            };

            _mapperMock.Setup(x =>
               x.Map(It.IsAny<Comment>(), It.IsAny<CommentListDTO>()))
                   .Returns(commentListDTO);

            var result = await _commentService.AddCommentAsync(createCommentDTO, userId);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(commentListDTO);
            await Task.CompletedTask;
        }

        [Test]
        [TestCase(1)]
        public async Task GetCommentListsAsync_TaskIdValid_ReturnListOfComments(int taskId)
        {
            var comments = CommentTestData.GetCommentsList();
            var commentsDTO = CommentTestData.GetCommentsListDTOs();

            SetupGetCommentsListBySpecAsync(comments);
            _mapperMock.SetupMap(comments, commentsDTO);

            var result = await _commentService.GetCommentListsAsync(taskId);

            result.Should().NotBeNull();
            result.Should().Equal(commentsDTO);
            await Task.CompletedTask;
        }

        [Test]
        [TestCase("1")]
        public async Task EditCommentAsync_UserIsCreator_ReturnTaskComplete(string creatorId)
        {
            var comment = CommentTestData.GetComment();
            var editCommentDTO = CommentTestData.GetEditCommentDTO();

            SetupCommentGetByKeyASync(comment);

            comment.DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
            comment.CommentText = editCommentDTO.CommentText;

            SetupCommentUpdateAsync();
            SetupCommentSaveChangesAsync();

            var result = _commentService.EditCommentAsync(editCommentDTO, creatorId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
            await Task.CompletedTask;
        }

        [Test]
        [TestCase("2")]
        public async Task EditCommentAsync_UserIsNotCreator_ThrowHttpException(string creatorId)
        {
            var comment = CommentTestData.GetComment();
            var editCommentDTO = CommentTestData.GetEditCommentDTO();

            SetupCommentGetByKeyASync(comment);

            comment.DateOfCreate = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
            comment.CommentText = editCommentDTO.CommentText;

            Func<Task> act = () => _commentService.EditCommentAsync(editCommentDTO, creatorId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermissionEditComment);
        }

        [Test]
        [TestCase(1, "1", 1)]
        public async Task DeleteCommentAsync_UserIsHavePermission_ReturnTaskComplete(int id, string userId, int workspaceId)
        {
            var userWorkspaceMock = CommentTestData.GetUserWorkspace();
            var comment = CommentTestData.GetComment();

            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspaceMock);
            SetupCommentGetByKeyASync(comment);
            SetupDeleteCommentAsync(comment);
            SetupCommentSaveChangesAsync();

            var result = _commentService.DeleteCommentAsync(id, userId, workspaceId);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
            await Task.CompletedTask;
        }

        [Test]
        [TestCase(1, "2", 1)]
        public async Task DeleteCommentAsync_UserIsNotPermission_ThrowHttpException(int id, string userId, int workspaceId)
        {
            var userWorkspaceMock = CommentTestData.GetUserWorkspace();
            var comment = CommentTestData.GetComment();

            SetupUserWorkspaceGetFirstBySpecAsync(userWorkspaceMock);
            SetupCommentGetByKeyASync(comment);

            Func<Task> act = async () => await _commentService.DeleteCommentAsync(id, userId, workspaceId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.Forbidden)
                .WithMessage(ErrorMessages.NotPermissionDeleteComment);
        }

        [TearDown]
        public void TearDown()
        {
            _userRepositoryMock.Verify();
            _taskRepositoryMock.Verify();
            _commentRepositoryMock.Verify();
            _userWorkspaceRepositoryMock.Verify();
            _workspaceRepositoryMock.Verify();
            _userManagerMock.Verify();
            _mapperMock.Verify();
        }

        protected void SetupDeleteCommentAsync(Comment comment)
        {
            _commentRepositoryMock
                .Setup(x => x.DeleteAsync(comment ?? It.IsAny<Comment>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupUserWorkspaceGetFirstBySpecAsync(UserWorkspace userWorkspace)
        {
            _userWorkspaceRepositoryMock
                .Setup(x => x.GetFirstBySpecAsync(It.IsAny<ISpecification<UserWorkspace>>()))
                .ReturnsAsync(userWorkspace)
                .Verifiable();
        }
        protected void SetupCommentSaveChangesAsync()
        {
            _commentRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.FromResult(1))
                .Verifiable();
        }
        protected void SetupCommentUpdateAsync()
        {
            _commentRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupCommentAddAsync(Comment comment)
        {
            _commentRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(comment)
                .Verifiable();
        }

        protected void SetupCommentGetByKeyASync(Comment comment)
        {
            _commentRepositoryMock
                .Setup(x => x.GetByKeyAsync(It.IsAny<int>()))
                .ReturnsAsync(comment)
                .Verifiable();
        }

        protected void SetupGetCommentsListBySpecAsync(IEnumerable<Comment> comments)
        {
            _commentRepositoryMock
                .Setup(x => x.GetListBySpecAsync(It.IsAny<ISpecification<Comment>>()))
                .ReturnsAsync(comments);
        }
    }
}
