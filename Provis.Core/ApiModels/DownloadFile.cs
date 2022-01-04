using System.IO;

namespace Provis.Core.ApiModels
{
    public class DownloadFile
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }
    }
}
