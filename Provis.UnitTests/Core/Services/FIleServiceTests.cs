using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Provis.Core.ApiModels;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using Provis.Core.Services;
using Provis.UnitTests.Base.TestData;
using System.IO;
using System.Threading.Tasks;

namespace Provis.UnitTests.Core.Services
{
    public class FIleServiceTests
    {
        protected FileService _fileService;

        protected Mock<IOptions<FileSettings>> _fileSettingsMock;
        protected Mock<IAzureBlobStorageService> _azureBlobStorageServiceMock;
        protected Mock<ILocaleStorageService> _localeStorageServiceMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fileSettingsMock = new Mock<IOptions<FileSettings>>();
            _azureBlobStorageServiceMock = new Mock<IAzureBlobStorageService>();
            _localeStorageServiceMock = new Mock<ILocaleStorageService>();

            _fileService = new FileService(_fileSettingsMock.Object,
                _azureBlobStorageServiceMock.Object,
                _localeStorageServiceMock.Object);
        }

        [Test]
        public async Task AddFileAsync_To_Azure_Successful()
        {
            var fileSettings = new FileSettings()
            {
                AllowChangeName = true,
                AllowCreateFolderPath = true,
                AllowStoreInAzureBlobStore = true
            };
            var (stream, folderPath, fileName) = GetAddedFile();

            SetupFileSettings(fileSettings);
            SetupAddFileAzureAsync(stream, folderPath, fileName);

            var result = await _fileService.AddFileAsync(stream, folderPath, fileName);
            stream.Dispose();

            result.Should().NotBeNull();
            result.Should().Be($"{folderPath}/{fileName}");
        }

        [Test]
        public async Task AddFileAsync_To_LocalStorage_Successful()
        {
            var fileSettings = new FileSettings()
            {
                AllowChangeName = true,
                AllowCreateFolderPath = true,
                AllowStoreInAzureBlobStore = false
            };
            var (stream, folderPath, fileName) = GetAddedFile();

            SetupFileSettings(fileSettings);
            SetupAddFileLocalAsync(stream, folderPath, fileName);

            var result = await _fileService.AddFileAsync(stream, folderPath, fileName);
            stream.Dispose();

            result.Should().NotBeNull();
            result.Should().Be($"{folderPath}/{fileName}");
        }

        [Test]
        public async Task GetFileAsync_From_Azure_Successful()
        {
            var downloadFile = GetDownloadFile();
            var dbPath = GetDBPath(StorageTypes.AzureBlob);

            SetupGetFileAsyncAzure(downloadFile, GetFilePath());

            var result = await _fileService.GetFileAsync(dbPath);
            result.Should().NotBeNull();
            result.Should().Be(downloadFile);
        }

        [Test]
        public async Task GetFileAsync_From_LocalStorage_Successful()
        {
            var downloadFile = GetDownloadFile();
            var dbPath = GetDBPath(StorageTypes.Locale);

            SetupGetFileAsyncLocalStorage(downloadFile, GetFilePath());

            var result = await _fileService.GetFileAsync(dbPath);
            result.Should().NotBeNull();
            result.Should().Be(downloadFile);
        }

        [Test]
        public async Task GetFileAsync_WrongPath_Returns_Null()
        {
            var path = string.Empty;
            var result = await _fileService.GetFileAsync(path);

            result.Should().BeNull();
        }

        [Test]
        public void DeleteFileAsync_From_Azure_Successful()
        {
            var dbPath = GetDBPath(StorageTypes.AzureBlob);

            SetupDeleteFileAsyncAzure(GetFilePath());

            var result = _fileService.DeleteFileAsync(dbPath);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Test]
        public void DeleteFileAsync_From_Local_Successful()
        {
            var dbPath = GetDBPath(StorageTypes.Locale);

            SetupDeleteFileAsyncLocal(GetFilePath());

            var result = _fileService.DeleteFileAsync(dbPath);

            result.IsCompleted.Should().BeTrue();
            result.IsCompletedSuccessfully.Should().BeTrue();
        }

        [TearDown]
        public void TearDown()
        {
            _azureBlobStorageServiceMock.Verify();
            _localeStorageServiceMock.Verify();
            _fileSettingsMock.Verify();
        }

        protected void SetupFileSettings(FileSettings fileSettings)
        {
            _fileSettingsMock
                .Setup(x => x.Value)
                .Returns(fileSettings)
                .Verifiable();
        }

        protected void SetupAddFileAzureAsync(Stream stream, string folderPath, 
            string fileName)
        {
            _azureBlobStorageServiceMock
                .Setup(x => x.AddFileAsync(stream,
                                           folderPath,
                                           fileName))
                .ReturnsAsync($"{folderPath}/{fileName}")
                .Verifiable();
        }

        protected void SetupAddFileLocalAsync(Stream stream, string folderPath,
           string fileName)
        {
            _localeStorageServiceMock
                .Setup(x => x.AddFileAsync(stream,
                                           folderPath,
                                           fileName))
                .ReturnsAsync($"{folderPath}/{fileName}")
                .Verifiable();
        }

        protected void SetupGetFileAsyncAzure(DownloadFile downloadFile, string filePath)
        {
            _azureBlobStorageServiceMock
                .Setup(x => x.GetFileAsync(filePath))
                .ReturnsAsync(downloadFile)
                .Verifiable();
        }

        protected void SetupGetFileAsyncLocalStorage(DownloadFile downloadFile, string filePath)
        {
            _localeStorageServiceMock
                .Setup(x => x.GetFileAsync(filePath))
                .ReturnsAsync(downloadFile)
                .Verifiable();
        }

        protected void SetupDeleteFileAsyncAzure(string filePath)
        {
            _azureBlobStorageServiceMock
                .Setup(x => x.DeleteFileAsync(filePath))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected void SetupDeleteFileAsyncAzureNotVerifiable(string filePath)
        {
            _azureBlobStorageServiceMock
                .Setup(x => x.DeleteFileAsync(filePath))
                .Returns(Task.CompletedTask);
        }

        protected void SetupDeleteFileAsyncLocal(string filePath)
        {
            _localeStorageServiceMock
                .Setup(x => x.DeleteFileAsync(filePath))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        protected (Stream stream, string folderPath, string fileName) GetAddedFile()
        {
            return (FileTestData.GetTestFormFile("file", "content")
                .OpenReadStream(), "attachments", "file");
        }

        protected string GetDBPath(StorageTypes storageType)
        {
            return $"{storageType}:attachments\\file";
        }

        protected DownloadFile GetDownloadFile()
        {
            return FileTestData.GetTestDownloadFile("file", "type", "content");
        }

        protected string GetFilePath()
        {
            return "attachments\\file";
        }
    }
}
