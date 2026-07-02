using Xunit;
using FileSystemCommands;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirSize");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "123"); 

        var command = new DirectorySizeCommand(testDir);
        command.Execute();

        Assert.Equal(3, command.TotalSize);
        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDirFind");
        Directory.CreateDirectory(testDir);
        File.Create(Path.Combine(testDir, "a.txt")).Dispose();
        File.Create(Path.Combine(testDir, "b.log")).Dispose();

        var command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Assert.Single(command.FoundFiles);
        Assert.Contains("a.txt", Path.GetFileName(command.FoundFiles[0]));
        Directory.Delete(testDir, true);
    }
}