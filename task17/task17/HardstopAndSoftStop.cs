using System.Windows.Input;

public class HardStopCommand : ICommand
{
    private readonly ServerThread _server;
    public HardStopCommand(ServerThread server) => _server = server;
    public void Execute() => _server.RequestHardStop();
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _server;
    public SoftStopCommand(ServerThread server) => _server = server;
    public void Execute() => _server.RequestSoftStop();
}