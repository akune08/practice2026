using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using NUnit.Framework;

namespace TaskScheduling.Tests
{

    public interface ICommand
    {
        void Execute();
        bool IsCompleted { get; }
    }

    public class MockCommand : ICommand
    {
        public bool IsCompleted { get; set; }
        public void Execute() { }
    }

    public class CountCommand : ICommand
    {
        private int _remainingSteps;
        public CountCommand(int steps) => _remainingSteps = steps;
        public bool IsCompleted => _remainingSteps <= 0;
        public void Execute() { if (_remainingSteps > 0) _remainingSteps--; }
    }

    public interface IScheduler
    {
        bool HasCommand();
        ICommand Select();
        void Add(ICommand cmd);
    }

    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _newCommands = new Queue<ICommand>();
        private readonly List<ICommand> _activeCommands = new List<ICommand>();
        private int _currentIndex = 0;

        public void Add(ICommand cmd) { lock (_newCommands) { _newCommands.Enqueue(cmd); } }

        public bool HasCommand()
        {
            lock (_newCommands) { return _newCommands.Count > 0 || _activeCommands.Count > 0; }
        }

        public ICommand Select()
        {
            lock (_newCommands) { while (_newCommands.Count > 0) _activeCommands.Add(_newCommands.Dequeue()); }
            if (_activeCommands.Count == 0) return null;

            _currentIndex %= _activeCommands.Count;
            var command = _activeCommands[_currentIndex];

            if (command.IsCompleted) { _activeCommands.RemoveAt(_currentIndex); return Select(); }

            _currentIndex++;
            return command;
        }
    }


    [TestFixture]
    public class SchedulerTests
    {
        [Test]
        public void Scheduler_ShouldExecuteCommandsInRoundRobinOrder()
        {
            var scheduler = new RoundRobinScheduler();
            var cmd1 = new MockCommand();
            var cmd2 = new MockCommand();

            scheduler.Add(cmd1);
            scheduler.Add(cmd2);

            Assert.That(scheduler.Select(), Is.EqualTo(cmd1));
            Assert.That(scheduler.Select(), Is.EqualTo(cmd2));
            Assert.That(scheduler.Select(), Is.EqualTo(cmd1));
        }

        [Test]
        public void Scheduler_ShouldRemoveCompletedCommands()
        {
            var scheduler = new RoundRobinScheduler();
            var cmd = new MockCommand { IsCompleted = false };
            scheduler.Add(cmd);

            scheduler.Select(); 
            cmd.IsCompleted = true; 

            Assert.That(scheduler.HasCommand(), Is.True);
            scheduler.Select(); 
            Assert.That(scheduler.HasCommand(), Is.False);
        }

        [Test]
        public void Processor_ShouldSwitchBetweenCommands()
        {
            var scheduler = new RoundRobinScheduler();
            var cmd1 = new CountCommand(2);
            var cmd2 = new CountCommand(2);

            scheduler.Add(cmd1);
            scheduler.Add(cmd2);

            scheduler.Select().Execute(); 
            scheduler.Select().Execute(); 
            scheduler.Select().Execute(); 
            Assert.That(cmd1.IsCompleted, Is.True);
            Assert.That(cmd2.IsCompleted, Is.False);
        }

        [Test]
        public void Processor_ShouldWaitWhenNoCommands()
        {
            var queue = new BlockingCollection<ICommand>();
            var cts = new CancellationTokenSource();


            var thread = new Thread(() => {
                while (!cts.Token.IsCancellationRequested)
                {

                    if (queue.TryTake(out var cmd, 100)) {  }
                }
            }