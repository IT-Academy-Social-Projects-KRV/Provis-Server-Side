using System;
using System.IO;

namespace Provis.Core.ApiModels
{
    public class DownloadFile : IDisposable
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Content { get; set; }

        public void Dispose()
        {
            if (Content != null)
            {
                Content.Dispose();
            }
        }
    }
}
