using System;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileNotFoundException: FileException
    {
        public FileNotFoundException(string path) : base("File not found", path)
        {

        }
    }
}
