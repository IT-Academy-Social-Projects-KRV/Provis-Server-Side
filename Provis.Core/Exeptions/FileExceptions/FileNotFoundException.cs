using System;
using System.Runtime.Serialization;

namespace Provis.Core.Exeptions.FileExceptions
{
    [Serializable]
    public class FileNotFoundException : FileException
    {
        public FileNotFoundException()
            : base("File not found") { }

        public FileNotFoundException(Exception innerException)
            : base("File not found", innerException) { }

        public FileNotFoundException(string path)
            : base("File not found", path) { }

        protected FileNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
