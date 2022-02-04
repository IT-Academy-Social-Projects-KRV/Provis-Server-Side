using Provis.Core.Resources;
using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class CannotGetFileContentTypeException : FileException
    {
        public CannotGetFileContentTypeException()
            : base(ErrorMessages.CannotGetFileContentType) { }

        public CannotGetFileContentTypeException(Exception innerException)
            : base(ErrorMessages.CannotGetFileContentType, innerException) { }

        public CannotGetFileContentTypeException(string path)
            : base(ErrorMessages.CannotGetFileContentType, path) { }

        protected CannotGetFileContentTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
