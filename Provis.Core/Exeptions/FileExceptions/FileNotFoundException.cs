namespace Provis.Core.Exeptions.FileExceptions
{
    public class FileNotFoundException: FileException
    {
        public FileNotFoundException(string path) : base("File not found", path)
        {

        }
    }
}
