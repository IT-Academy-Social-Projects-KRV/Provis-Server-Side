using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class CannotGetFileContentTypeException : FileException
    {
        public CannotGetFileContentTypeException()
            : base("Can not get content type of file") { }

        public CannotGetFileContentTypeException(Exception innerException)
            : base("Can not get content type of file", innerException) { }

        public CannotGetFileContentTypeException(string path)
            : base("Can not get content type of file", path) { }

        protected CannotGetFileContentTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
