namespace Provis.Core.Exeptions.FileExceptions
{
    public class FileFolderNotExistException: FileException
    {
        public FileFolderNotExistException(string path) : base("Cannot save the file", path)
        {

        }
    }
}
