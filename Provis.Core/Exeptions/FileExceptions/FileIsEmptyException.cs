using System;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    class FileIsEmptyException: FileException
    {
        public FileIsEmptyException(string path) : base("This file is empty", path)
        {

        }
    }
}
