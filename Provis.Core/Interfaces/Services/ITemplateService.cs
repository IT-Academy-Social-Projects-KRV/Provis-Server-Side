using System.Threading.Tasks;

namespace Provis.Core.Interfaces.Services
{
    public interface ITemplateService
    {
        Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model);
    }
}
