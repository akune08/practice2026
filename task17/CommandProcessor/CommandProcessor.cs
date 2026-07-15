using System.Collections.Concurrent;
using System.Windows.Input;

public class CommandProcessor
{
    private readonly IScheduler _scheduler;
    private readonly BlockingCollection<ICommand> _incomingQueue; 

    public void Run()
    {
        while (true)
        {
            if (_incomingQueue.TryTake(out var newCmd, 0))
            {
                _scheduler.Add(newCmd);
            }

            if (_scheduler.HasCommand())
            {
                var cmd = _scheduler.Select();
                cmd.Execute(); 
            }
            else
            {
                var cmd = _incomingQueue.Take();
                _scheduler.Add(cmd);
            }
        }
    }
}