using CommandLib;
using System.Windows.Input;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _path;
    public long TotalSize { get; private set; } 

    public DirectorySizeCommand(string path) => _path = path;

    public void Execute()
    {
        var dirInfo = new DirectoryInfo(_path);
        TotalSize = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        Console.WriteLine($"Размер каталога '{_path}': {TotalSize} байт");
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _path;
    private readonly string _mask;
    public List<string> FoundFiles { get; private set; } = new();

    public FindFilesCommand(string path, string mask)
    {
        _path = path;
        _mask = mask;
    }

    public void Execute()
    {
        FoundFiles = Directory.GetFiles(_path, _mask).ToList();
        Console.WriteLine($"Найдено файлов по маске '{_mask}': {FoundFiles.Count}");
        foreach (var file in FoundFiles) Console.WriteLine(file);
    }
}
