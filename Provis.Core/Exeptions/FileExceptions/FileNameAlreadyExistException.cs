using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileNameAlreadyExistException : FileException
    {
        public FileNameAlreadyExistException()
            : base("File with this name already exist") { }

        public FileNameAlreadyExistException(Exception innerException)
            : base("File with this name already exist", innerException) { }

        public FileNameAlreadyExistException(string path)
            : base("File with this name already exist", path) { }

        protected FileNameAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
