using System;

namespace Provis.Core.Exeptions.FileExceptions
{
    public class FileException: Exception
    {
        public override string Message { get; }
        public string Path { get; }
        public FileException(string message, string path)
        {
            Message = message;
            Path = path;
        }
    }
}
