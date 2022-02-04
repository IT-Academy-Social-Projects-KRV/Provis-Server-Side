using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Repositories
{
    public interface ISaveChanges
    {
        Task<int> SaveChangesAsync();
    }
}
