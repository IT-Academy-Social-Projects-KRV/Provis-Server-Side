namespace Provis.Core.Exeptions.FileExceptions{
    public class FileNameAlreadyExistException: FileException
    {
        public FileNameAlreadyExistException(string path) : base("File with this name already exist", path)
        {

        }
    }
}
