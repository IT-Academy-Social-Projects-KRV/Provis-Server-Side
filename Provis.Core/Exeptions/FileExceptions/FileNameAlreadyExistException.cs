using Provis.Core.Resources;
using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileNameAlreadyExistException : FileException
    {
        public FileNameAlreadyExistException()
            : base(ErrorMessages.FileNameAlreadyExist) { }

        public FileNameAlreadyExistException(Exception innerException)
            : base(ErrorMessages.FileNameAlreadyExist, innerException) { }

        public FileNameAlreadyExistException(string path)
            : base(ErrorMessages.FileNameAlreadyExist, path) { }

        protected FileNameAlreadyExistException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
