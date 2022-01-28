using Provis.Core.Resources;
using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileNotFoundException : FileException
    {
        public FileNotFoundException()
            : base(ErrorMessages.FileNotFound) { }

        public FileNotFoundException(Exception innerException)
            : base(ErrorMessages.FileNotFound, innerException) { }

        public FileNotFoundException(string path)
            : base(ErrorMessages.FileNotFound, path) { }

        protected FileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
