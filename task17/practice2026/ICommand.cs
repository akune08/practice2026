public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
    private readonly Thread _thread;
    private bool _softStopRequested = false;

    public ServerThread()
    {
        _thread = new Thread(Process);
        _thread.Start();
    }

    public void AddCommand(ICommand command) => _queue.Add(command);

    private void Process()
    {
        try
        {
            foreach (var command in _queue.GetConsumingEnumerable())
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in {command.GetType().Name}: {ex.Message}");
                }

                if (_softStopRequested && _queue.Count == 0) break;
            }
        }
        catch (OperationCanceledException) { }
    }

    public void RequestHardStop()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("HardStop должен выполняться в целевом потоке.");

        _queue.CompleteAdding();
        _thread.Interrupt(); 
        throw new ThreadAbortException(); 
    }

    public void RequestSoftStop()
    {
        if (Thread.CurrentThread != _thread)
            throw new InvalidOperationException("SoftStop должен выполняться в целевом потоке.");

        _softStopRequested = true;
    }
}
}
