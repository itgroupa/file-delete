namespace FileDeleter;

public class FileInformation
{
    public string Patch;
    public string Hash;
    public long Size;

    public FileInformation(string patch, string hash, long size)
    {
        Patch = patch;
        Hash = hash;
        Size = size;
    }
}