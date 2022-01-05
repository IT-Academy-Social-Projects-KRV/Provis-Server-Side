using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    class FileIsEmptyException : FileException
    {
        public FileIsEmptyException()
            : base("This file is empty") { }

        public FileIsEmptyException(Exception innerException)
            : base("This file is empty", innerException) { }

        public FileIsEmptyException(string path)
            : base("This file is empty", path) { }

        protected FileIsEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
