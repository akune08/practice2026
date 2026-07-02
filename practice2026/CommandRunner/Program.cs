using CommandLib;
using System.Reflection;
using System.Windows.Input;

string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");
Assembly assembly = Assembly.LoadFrom(dllPath);

var commandTypes = assembly.GetTypes()
    .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface);

foreach (var type in commandTypes)
{
    ICommand? command = null;
    if (type.Name == "DirectorySizeCommand")
        command = (ICommand?)Activator.CreateInstance(type, AppDomain.CurrentDomain.BaseDirectory);
    else if (type.Name == "FindFilesCommand")
        command = (ICommand?)Activator.CreateInstance(type, AppDomain.CurrentDomain.BaseDirectory, "*.dll");

    command?.Execute();
}