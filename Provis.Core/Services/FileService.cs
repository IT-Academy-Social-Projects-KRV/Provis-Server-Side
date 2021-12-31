using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Provis.Core.ApiModels;
using Provis.Core.Exeptions.FileExceptions;
using Provis.Core.Helpers;
using Provis.Core.Interfaces.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Provis.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<FileSettings> _fileSettings;

        public FileService(IWebHostEnvironment webHostEnvironment, IOptions<FileSettings> fileSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _fileSettings = fileSettings;
        }

        public async Task<string> AddFileAsync(Stream stream, string folderPath, string fileName)
        {
            if (stream == null)
            {
                throw new FileIsEmptyException(fileName);
            }

            await CreateDirectoryAsync(folderPath);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            string uniqueFileName = CreateName(fileName, uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var dbPath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return dbPath;
        }

        public Task<DownloadFile> GetFileAsync(string dbPath)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, dbPath);

            if (!File.Exists(path))
            {
                throw new Exeptions.FileExceptions.FileNotFoundException(path);
            }

            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                throw new CannotGetFileContentTypeException(path);
            }

            DownloadFile fileInfo = new DownloadFile()
            {
                ContentType = contentType,
                Name = Path.GetFileName(path),
                Content = new FileStream(path, FileMode.Open)
            };

            return Task.FromResult(fileInfo);
        }

        public async Task DeleteFileAsync(string dbPath)
        {
            string deletePath = Path.Combine(_webHostEnvironment.WebRootPath, dbPath);
            var file = new FileInfo(deletePath);
            if (file.Exists)
            {
                await Task.Factory.StartNew(() => file.Delete());
            }
        }

        private string CreateName(string fileName, string folderPath)
        {
            var path = Path.Combine(folderPath, fileName);

            bool isFileExsist = File.Exists(path);

            if (isFileExsist)
            {
                if (!_fileSettings.Value.AllowChangeName)
                {
                    throw new FileNameAlreadyExistException(path);
                }
                else
                {
                    return $"{Path.GetFileNameWithoutExtension(path)}_" +
                        $"{DateTime.Now.ToString("yyyyMMddTHHmmssfff")}" +
                        $"{Path.GetExtension(path)}";
                }
            }

            return fileName;
        }

        public Task CreateDirectoryAsync(string folderPath)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            if (!Directory.Exists(path))
            {
                if (!_fileSettings.Value.AllowCreateFolderPath)
                {
                    throw new FileFolderNotExistException(path);
                }
                else
                {
                    Directory.CreateDirectory(path);
                }
            }

            return Task.CompletedTask;
        }
    }
}
