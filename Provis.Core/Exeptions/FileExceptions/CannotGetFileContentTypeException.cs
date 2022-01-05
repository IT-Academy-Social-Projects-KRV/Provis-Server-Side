using System;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class CannotGetFileContentTypeException: FileException
    {
        public CannotGetFileContentTypeException(string path) : base("Can not get content type of file", path)
        {

        }
    }
}
