namespace Provis.Core.Exeptions.FileExceptions
{
    public class CannotGetFileContentTypeException: FileException
    {
        public CannotGetFileContentTypeException(string path) : base("Can not get content type of file", path)
        {

        }
    }
}
