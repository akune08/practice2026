using System.Windows.Input;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _newCommands = new Queue<ICommand>();
    private readonly List<ICommand> _activeCommands = new List<ICommand>();
    private int _currentIndex = 0;

    public void Add(ICommand cmd)
    {
        lock (_newCommands)
        {
            _newCommands.Enqueue(cmd);
        }
    }

    public bool HasCommand() => _newCommands.Count > 0 || _activeCommands.Count > 0;

    public ICommand Select()
    {

        lock (_newCommands)
        {
            while (_newCommands.Count > 0)
                _activeCommands.Add(_newCommands.Dequeue());
        }

        if (_activeCommands.Count == 0) return null;


        _currentIndex %= _activeCommands.Count;
        var command = _activeCommands[_currentIndex];


        if (command.IsCompleted)
        {
            _activeCommands.RemoveAt(_currentIndex);
            return Select(); 
        }

        _currentIndex++;
        return command;
    }
}