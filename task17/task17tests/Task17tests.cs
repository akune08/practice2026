[TestFixture]
public class ServerThreadTests
{
    [Test]
    public void SoftStop_ExecutesRemainingCommands()
    {
        var server = new ServerThread();
        var list = new List<int>();

        server.AddCommand(new ActionCommand(() => list.Add(1)));
        server.AddCommand(new SoftStopCommand(server));
        server.AddCommand(new ActionCommand(() => list.Add(2)));

        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void HardStop_InterruptsImmediately()
    {
        var server = new ServerThread();
        server.AddCommand(new HardStopCommand(server));
        server.AddCommand(new ActionCommand(() => {  }));

        
    }
}