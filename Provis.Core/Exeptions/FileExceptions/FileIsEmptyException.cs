namespace Provis.Core.Exeptions.FileExceptions
{
    class FileIsEmptyException: FileException
    {
        public FileIsEmptyException(string path) : base("This file is empty", path)
        {

        }
    }
}
