using Ardalis.Specification;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Provis.Core.DTO.SprintDTO;
using Provis.Core.Entities.SprintEntity;
using Provis.Core.Entities.WorkspaceEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using Provis.Core.Exeptions;
using Provis.Core.Interfaces.Repositories;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base;
using Provis.UnitTests.Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class SprintServiceTests
    {
        protected SprintService _sprintService;

        private Mock<IRepository<Workspace>> _workspaceRepositoryMock;
        private Mock<IRepository<WorkspaceTask>> _taskRepositoryMock;
        private Mock<IRepository<Sprint>> _sprintRepositoryMock;
        private  Mock<IMapper> _mapperMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _workspaceRepositoryMock = new Mock<IRepository<Workspace>>();
            _taskRepositoryMock = new Mock<IRepository<WorkspaceTask>>();
            _sprintRepositoryMock = new Mock<IRepository<Sprint>>();
            _mapperMock = new Mock<IMapper>();

            _sprintService = new SprintService(
                _workspaceRepositoryMock.Object,
                _taskRepositoryMock.Object,
                _sprintRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public async Task AddSprintAsync_SptintIsOn_ReturnSprintInfoDTO()
        {
            var changeSprintInfo = GetChangeSprintInfo();
            var worksace = GetWorkspace();
            var sprint = GetSprint();
            var sprintInfo = GetSprintInfo();
            var workspaceId = worksace.Id;

            SetupWorkspaceGetByKeyAsync(workspaceId, worksace);
            _mapperMock.SetupMap(changeSprintInfo, sprint);
            SetupSprintAddAsync(sprint, sprint);
            SetupSprintSaveChangeAsync();
            _mapperMock.SetupMap(sprint, sprintInfo);

            var result = await _sprintService.AddSprintAsync(changeSprintInfo, workspaceId);

            result.Should()
                .NotBeNull();

            result.Should()
                .BeEquivalentTo(sprintInfo);
        }

        [Test]
        public async Task AddSprintAsync_SptintIsNotOn_ThrowHttpException()
        {
            var changeSprintInfo = GetChangeSprintInfo();
            var worksace = GetWorkspace();
            worksace.isUseSprints = false;
            var workspaceId = worksace.Id;

            SetupWorkspaceGetByKeyAsync(workspaceId, worksace);

            Func<Task<SprintInfoDTO>> act =
                () => _sprintService.AddSprintAsync(changeSprintInfo, workspaceId);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.SprintsNotOn);
        }

        [Test]
        public async Task GetSprintById_SptintExist_ReturnSprintDetailInfoDTO()
        {
            var sprint = GetSprint();
            var sprintDetailInfo = GetSprintDetailInfo();

            SetupSprintGetByKeyAsync(sprint.Id, sprint);
            _mapperMock.SetupMap(sprint, sprintDetailInfo);

            var result = await _sprintService.GetSprintById(sprint.Id);

            result.Should()
                .NotBeNull();

            result.Should()
                .BeEquivalentTo(sprintDetailInfo);
        }

        [Test]
        public async Task GetSprintListAsync_SptintsExist_ReturnListSprintInfo()
        {
            var workspaceId = 1;
            var sprintList = GetSprintList();
            var sprintInfoList = GetSprintInfoList();

            SetupSprintGetListBySpecAsync(sprintList);
            _mapperMock.SetupMap(sprintList, sprintInfoList);

            var result = await _sprintService.GetSprintListAsync(workspaceId);

            result.Should()
                .NotBeNull();

            result.Should()
                .BeEquivalentTo(sprintInfoList);
        }

        [Test]
        public async Task OffSprintsAsync_SprintsIsOn_ReturnTaskComplited()
        {
            var workspace = GetWorkspace();

            SetupWorkspaceGetByKeyAsync(workspace.Id, workspace);
            SetupWorkspaceSaveChangeAsync();
            SetupTaskSqlQuery();

            var result = _sprintService.OffSprintsAsync(workspace.Id);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task OffSprintsAsync_SprintsAlreadyOff_ThrowHttpException()
        {
            var workspace = GetWorkspace();
            workspace.isUseSprints = false;

            SetupWorkspaceGetByKeyAsync(workspace.Id, workspace);

            Func<Task> act =
                () => _sprintService.OffSprintsAsync(workspace.Id);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.SprintsAlreadyOff);
        }

            [Test]
        public async Task OnSprintsAsync_SprintsIsOff_ThrowHttpException()
        {
            var workspace = GetWorkspace();
            workspace.isUseSprints = false;
            var sprintToInsert = new Sprint()
            {
                Name = "Sprint name",
                WorkspaceId = workspace.Id
            };

            SetupWorkspaceGetByKeyAsync(workspace.Id, workspace);
            SetupSprintAddAsync(sprintToInsert);
            SetupSprintSaveChangeAsync();
            SetupTaskSqlQuery();

            var result = _sprintService.OnSprintsAsync(workspace.Id);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;
        }

        [Test]
        public async Task OnSprintsAsync_SprintsAlreadyOn_ReturnTaskComplited()
        {
            var workspace = GetWorkspace();

            SetupWorkspaceGetByKeyAsync(workspace.Id, workspace);

            Func<Task> act =
               () => _sprintService.OnSprintsAsync(workspace.Id);

            await act.Should()
                .ThrowAsync<HttpException>()
                .Where(x => x.StatusCode == HttpStatusCode.BadRequest)
                .WithMessage(ErrorMessages.SprintsAlreadyOn);
        }

        [Test]
        public async Task UpdateSprintAsync_SprintsExist__ReturnTaskComplited()
        {
            var changeSprintInfo = GetChangeSprintInfo();
            var sprint = GetSprint();

            SetupSprintGetByKeyAsync(sprint.Id, sprint);
            SetupMap(changeSprintInfo, sprint);
            SetupSprintUpdateAsync(sprint);
            SetupSprintSaveChangeAsync();

            var result = _sprintService.UpdateSprintAsync(changeSprintInfo, sprint.Id);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            await Task.CompletedTask;

        }

        [TearDown]
        public void TearDown()
        {
            _workspaceRepositoryMock.Verify();
            _taskRepositoryMock.Verify();
            _sprintRepositoryMock.Verify();
            _mapperMock.Verify();
        }

        protected void SetupWorkspaceGetByKeyAsync(int? key, Workspace workspaceInstance)
        {
            _workspaceRepositoryMock
                .Setup(x => x.GetByKeyAsync(key ?? It.IsAny<int>()))
                .ReturnsAsync(workspaceInstance)
                .Verifiable();
        }

        protected void SetupSprintGetByKeyAsync(int? key, Sprint sprintInstance)
        {
            _sprintRepositoryMock
                .Setup(x => x.GetByKeyAsync(key ?? It.IsAny<int>()))
                .ReturnsAsync(sprintInstance)
                .Verifiable();
        }

        protected void SetupSprintAddAsync(Sprint sprint, Sprint sprintInstance)
        {
            _sprintRepositoryMock
                .Setup(x => x.AddAsync(sprint ?? It.IsAny<Sprint>()))
                .ReturnsAsync(sprintInstance)
                .Verifiable();
        }

        protected void SetupSprintAddAsync(Sprint sprintInstance)
        {
            _sprintRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Sprint>()))
                .ReturnsAsync(sprintInstance)
                .Verifiable();
        }

        protected void SetupSprintUpdateAsync(Sprint sprint)
        {
            _sprintRepositoryMock
                .Setup(x => x.UpdateAsync(sprint ?? It.IsAny<Sprint>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupSprintSaveChangeAsync()
        {
            _sprintRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupWorkspaceSaveChangeAsync()
        {
            _workspaceRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupTaskSqlQuery()
        {
            _taskRepositoryMock
                .Setup(x=>x.SqlQuery(It.IsAny<string>()))
                .ReturnsAsync(1)
                .Verifiable();
        }

        protected void SetupSprintGetListBySpecAsync(List<Sprint> listInstance)
        {
            _sprintRepositoryMock
                .Setup(x=>x.GetListBySpecAsync(It.IsAny<ISpecification<Sprint>>()))
                .ReturnsAsync(listInstance);
        }

        protected void SetupMap<TSource, TDestination>(TSource source, TDestination destination)
        {
            _mapperMock
                .Setup(x => x.Map(source ?? It.IsAny<TSource>(), destination))
                .Returns(destination)
                .Verifiable();
        }

        protected ChangeSprintInfoDTO GetChangeSprintInfo()
        {
            return new ChangeSprintInfoDTO()
            {
                Name = "name",
                Description = "description"
            };
        }

        protected Workspace GetWorkspace()
        {
            return new Workspace()
            {
                Id = 1,
                Name = "name",
                Description = "description",
                isUseSprints = true
            };
        }

        protected Sprint GetSprint()
        {
            return new Sprint()
            {
                Id = 1,
                Name = "name",
                Description = "description"
            };
        }

        protected SprintInfoDTO GetSprintInfo()
        {
            return new SprintInfoDTO()
            {
                Id = 1,
                Name = "name"
            };
        }

        protected SprintDetailInfoDTO GetSprintDetailInfo()
        {
            return new SprintDetailInfoDTO()
            {
                Id = 1,
                Name = "name"
            };
        }

        protected List<Sprint> GetSprintList()
        {
            return new List<Sprint>()
            {
                new Sprint()
                {
                    Id = 1,
                    Name = "name1"
                },

                new Sprint()
                {
                    Id = 2,
                    Name = "name2"
                }
            };
        }

        protected List<SprintInfoDTO> GetSprintInfoList()
        {
            return new List<SprintInfoDTO>()
            {
                new SprintInfoDTO()
                {
                    Id = 1,
                    Name = "name1"
                },

                new SprintInfoDTO()
                {
                    Id = 2,
                    Name = "name2"
                }
            };
        }
    }
}
