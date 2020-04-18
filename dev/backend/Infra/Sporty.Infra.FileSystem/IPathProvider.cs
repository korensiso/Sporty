namespace Sporty.Infra.FileSystem
{
    public interface IPathProvider
    {
        string GetOsPath(string path);
    }
}