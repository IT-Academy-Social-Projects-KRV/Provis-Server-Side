using Provis.Core.ApiModels;
using System.IO;
using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> AddFileAsync(Stream stream, string folderName, string fileName);
        Task<DownloadFile> GetFileAsync(string path);
        Task DeleteFileAsync(string path);
    }
}
