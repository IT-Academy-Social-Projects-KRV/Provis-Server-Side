using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base.TestData;
using System.IO;
using System.Threading.Tasks;
using Provis.Core.Exeptions.FileExceptions;
using System;
using Provis.Core.ApiModels;

namespace Provis.UnitTests.Core.Services
{
    [TestFixture]
    public class LocalStorateServiceTests
    {
        protected LocaleStorageService _localeStorageService;

        protected Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        protected Mock<IOptions<FileSettings>> _fileSettingsMock;

        protected Mock<ILocaleStorageService> _localeStorageServiceMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _fileSettingsMock = new Mock<IOptions<FileSettings>>();

            _localeStorageService = new LocaleStorageService(
                _webHostEnvironmentMock.Object,
                _fileSettingsMock.Object);

            _localeStorageServiceMock = new Mock<ILocaleStorageService>();
        }

        [Test]
        public async Task AddFileAsync_Successful()
        {
            var file = GetAddedFile();

            SetupCreateDirectoryAsync(file.folderPath);
            SetupWebRootPath(nameof(StorageTypes.Locale));
            SetupFileSettings(GetSuccessfulSettings());

            var result = await _localeStorageService.AddFileAsync(file.stream, file.folderPath, file.fileName);
            file.stream.Dispose();

            DeleteFileDirectory();

            result.Should().NotBeNull();
            result.Should().Be($"{nameof(StorageTypes.Locale)}:{file.folderPath}\\{file.fileName}");
        }

        [Test]
        public async Task AddFileAsync_Throw_Exception_Stream_Null()
        {
            var file = GetAddedFile();
            file.stream.Dispose();
            file.stream = null;

            Func<Task<string>> result = () => _localeStorageService.AddFileAsync(
                file.stream, file.folderPath, file.fileName);

            await result.Should().ThrowAsync<FileException>();
        }

        [Test]
        public async Task AddFileAsync_Throw_Exception_AllowCreateFolderPath_False()
        {
            var file = GetAddedFile();
            var settings = GetSuccessfulSettings();
            settings.AllowCreateFolderPath = false;

            SetupCreateDirectoryAsync(file.folderPath);
            SetupWebRootPath(nameof(StorageTypes.Locale));
            SetupFileSettings(settings);

            Func<Task<string>> result = () => _localeStorageService.AddFileAsync(
                file.stream, file.folderPath, file.fileName);

            await result.Should()
                .ThrowAsync<FileFolderNotExistException>();
        }

        [Test]
        public async Task GetFileAsync_Throw_File_Not_Found()
        {
            SetupWebRootPath(nameof(StorageTypes.Locale));

            Func<Task<DownloadFile>> result = () => _localeStorageService.GetFileAsync(GetFilePath("TestFile.txt"));

            await result.Should()
                .ThrowAsync<Provis.Core.Exeptions.FileExceptions.FileNotFoundException>();
        }

        [Test]
        public async Task GetFileAsync_Cannot_Get_File_Content()
        {
            CreateTestFile("TestFile");
            var filePath = GetFilePath("TestFile");

            SetupWebRootPath(nameof(StorageTypes.Locale));

            Func<Task<DownloadFile>> result = () => _localeStorageService.GetFileAsync(filePath);

            await result.Should()
                .ThrowAsync<CannotGetFileContentTypeException>();
            DeleteFileDirectory();
        }

        [Test]
        public async Task GetFileAsync_Successful()
        {
            CreateTestFile("TestFile.txt");
            var filePath = GetFilePath("TestFile.txt");

            SetupWebRootPath(nameof(StorageTypes.Locale));

            var result = await _localeStorageService.GetFileAsync(filePath);
            result.Content.Close();

            result.Should().NotBeNull();
            DeleteFileDirectory();
        }

        [Test]
        public void DeleteFileAsync_Successful()
        {
            var file = CreateTestFile("TestFile.txt");
            SetupWebRootPath(nameof(StorageTypes.Locale));

            var result = _localeStorageService.DeleteFileAsync(file);

            result.Should().NotBeNull();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            DeleteFileDirectory();
        }

        [Test]
        public async Task DeleteFileAsync_File_NotFound()
        {
            CreateTestFile("TestFile.txt");
            SetupWebRootPath(nameof(StorageTypes.Locale));

            var result = _localeStorageService.DeleteFileAsync("WrongPass");

            result.Should().NotBeNull();

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();

            DeleteFileDirectory();
        }

        [TearDown]
        public void TearDown()
        {
            _webHostEnvironmentMock.Verify();
            _fileSettingsMock.Verify();
        }

        protected void SetupCreateDirectoryAsync(string folderPath)
        {
            _localeStorageServiceMock
                .Setup(x => x.CreateDirectoryAsync(folderPath))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupWebRootPath(string webRoot)
        {
            _webHostEnvironmentMock
                .Setup(x => x.WebRootPath)
                .Returns(webRoot)
                .Verifiable();
        }

        protected void SetupFileSettings(FileSettings fileSettings)
        {
            _fileSettingsMock.Setup(x => x.Value)
                .Returns(fileSettings)
                .Verifiable();
        }

        protected (Stream stream, string folderPath, string fileName) GetAddedFile()
        {
            return (FileTestData.GetTestFormFile("file", "content")
                .OpenReadStream(), "files", "file.txt");
        }

        protected FileSettings GetSuccessfulSettings()
        {
            return new FileSettings() 
            { 
                AllowChangeName = true, 
                AllowCreateFolderPath = true, 
                AllowStoreInAzureBlobStore = false 
            };
        }

        protected string GetDBFolderPath()
        {
            return $"{StorageTypes.Locale}:files";
        }

        protected string GetFilePath(string fileName)
        {
            return $"files\\{fileName}";
        }

        protected void DeleteFileDirectory()
        {
            Directory.Delete("Locale\\files", recursive: true);
        }

        protected string CreateTestFile(string fileName)
        {
            string path = "Locale\\files";

            Directory.CreateDirectory(path);
            using (FileStream stream = new($"{path}\\{fileName}", FileMode.Create))
            {
                stream.Close();
            }

            return path;
        }
    }
}
