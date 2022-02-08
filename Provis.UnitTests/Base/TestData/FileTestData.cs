using Microsoft.AspNetCore.Http;
using Provis.Core.ApiModels;
using System.IO;
using System.Text;

namespace Provis.UnitTests.Base.TestData
{
    public class FileTestData
    {
        public static IFormFile GetTestFormFile(string fileName, string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            return new FormFile(
                baseStream: new MemoryStream(bytes),
                baseStreamOffset: 0,
                length: bytes.Length,
                name: "Data",
                fileName: fileName);
        }

        public static IFormFile GetTestFormFile(string fileName, string content, string contentType)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            var file = new FormFile(
                baseStream: new MemoryStream(bytes),
                baseStreamOffset: 0,
                length: bytes.Length,
                name: "Data",
                fileName: fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return file;
        }

        public static DownloadFile GetTestDownloadFile(string fileName,
            string contentType,
            string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            return new DownloadFile()
            {
                Content = new MemoryStream(bytes),
                Name = fileName,
                ContentType = contentType
            };
        }
    }
}
