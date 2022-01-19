using Microsoft.Extensions.Options;
using Provis.Core.ApiModels;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using System.IO;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IOptions<FileSettings> _fileSettings;
        private readonly IAzureBlobStorageService _azureBlobStorageService;
        private readonly ILocaleStorageService _localeStorageService;

        public FileService(IOptions<FileSettings> fileSettings,
            IAzureBlobStorageService azureBlobStorageService,
            ILocaleStorageService localeStorageService)
        {
            _fileSettings = fileSettings;
            _azureBlobStorageService = azureBlobStorageService;
            _localeStorageService = localeStorageService;
        }

        public async Task<string> AddFileAsync(Stream stream, string folderPath, string fileName)
        {
            if(_fileSettings.Value.AllowStoreInAzureBlobStore)
            {
                return await _azureBlobStorageService.AddFileAsync(stream, folderPath, fileName);
            }
            else
            {
                return await _localeStorageService.AddFileAsync(stream, folderPath, fileName);
            }
        }

        public async Task<DownloadFile> GetFileAsync(string dbPath)
        {
            DownloadFile file = null;
            var storedFilePath = dbPath.Split(":");

            if(storedFilePath[0] == StorageTypes.AzureBlob.ToString())
            {
                file = await _azureBlobStorageService.GetFileAsync(storedFilePath[1]);
            }
            else if(storedFilePath[0] == StorageTypes.Locale.ToString())
            {
                file = await _localeStorageService.GetFileAsync(storedFilePath[1]);
            }

            return file;
        }

        public async Task DeleteFileAsync(string dbPath)
        {
            var storedFilePath = dbPath.Split(":");

            if (storedFilePath[0] == StorageTypes.AzureBlob.ToString())
            {
                await _azureBlobStorageService.DeleteFileAsync(storedFilePath[1]);
            }
            else if (storedFilePath[0] == StorageTypes.Locale.ToString())
            {
                await _localeStorageService.DeleteFileAsync(storedFilePath[1]);
            }
        }
    }
}
