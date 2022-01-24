using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ICreateDirectory
    {
        Task CreateDirectoryAsync(string folderPath);
    }
}
