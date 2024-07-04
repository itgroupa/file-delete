using System.Security.Cryptography;
using FileDeleter;

Console.WriteLine("Choose folder with files");
var path = Console.ReadLine();

if (string.IsNullOrEmpty(path))
{
    Console.WriteLine("You have not chosen any folder");
    return;
}

void GetFilesList(string p, List<FileInformation> filesList, HashSet<string> filesHash)
{
    foreach (var file in Directory.GetFiles(p))
    {
        var fileInfo = new FileInfo(file);

        using var md5 = MD5.Create();

        using var stream = File.OpenRead(file);

        var hashBytes = md5.ComputeHash(stream);

        var hash = BitConverter.ToString(hashBytes);

        if (filesHash.Contains(hash))
            filesList.Add(new FileInformation(file, hash, fileInfo.Length));
        else
            filesHash.Add(hash);
    }

    foreach (var directory in Directory.GetDirectories(p))
    {
        GetFilesList(directory, filesList, filesHash);
    }
}

void DeleteEmptyFolders(string p)
{
    var files = Directory.GetFiles(p);
    var directories = Directory.GetDirectories(p);

    var needResearch = false;

    foreach (var directory in directories)
    {
        DeleteEmptyFolders(directory);
        needResearch = true;
    }

    if (needResearch)
    {
        files = Directory.GetFiles(p);
        directories = Directory.GetDirectories(p);
    }

    if (files.Length != 0 || directories.Length != 0) return;
    
    Console.WriteLine($"folder will deleted - {p}");
    Console.WriteLine();
    Directory.Delete(p);
}

var filesForDelete = new List<FileInformation>();
GetFilesList(path, filesForDelete, new HashSet<string>());

Console.WriteLine($"will free space - {filesForDelete.Sum(r => r.Size)}");
Console.WriteLine("are you agree to delete files? [y/n]");

var confirmation = Console.ReadLine();

if (string.IsNullOrEmpty(confirmation) || confirmation != "y") return;

foreach (var file in filesForDelete)
{
    Console.WriteLine(file.Patch);
    Console.WriteLine(file.Hash);
    Console.WriteLine();
    File.Delete(file.Patch);
}

DeleteEmptyFolders(path);