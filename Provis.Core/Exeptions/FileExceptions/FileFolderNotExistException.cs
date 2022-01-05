using System;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileFolderNotExistException: FileException
    {
        public FileFolderNotExistException(string path) : base("Cannot save the file", path)
        {

        }
    }
}
