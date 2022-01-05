using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileFolderNotExistException : FileException
    {
        public FileFolderNotExistException()
            : base("Cannot save the file") { }

        public FileFolderNotExistException(Exception innerException)
            : base("Cannot save the file", innerException) { }

        public FileFolderNotExistException(string path)
            : base("Cannot save the file", path) { }

        protected FileFolderNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
