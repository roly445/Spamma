namespace Spamma.Api.Web.Infrastructure.Contracts.SutWrappers
{
    public interface IFileWrapper
    {
        void Delete(string path);
    }

    public class FileWrapper : IFileWrapper
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}