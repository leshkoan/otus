using Spacebattle.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Spacebattle;

public class CommandProcessor
{
    public ConcurrentQueue<ICommand> commandQueue;
    public Thread? comandThread { get; private set; }
    public CancellationTokenSource softStopToken { get; }
    public CancellationTokenSource hardStopToken { get; }

    public CommandProcessor()
    {
        commandQueue = new ConcurrentQueue<ICommand>();
        comandThread = new Thread(StartProcessingCommands);
        softStopToken = new CancellationTokenSource();
        hardStopToken = new CancellationTokenSource();
    }

    public void StartProcessingCommands()
    {
        comandThread = new Thread(() =>
        {
            while (!((softStopToken != null && softStopToken.IsCancellationRequested && commandQueue.IsEmpty)
                || (hardStopToken != null && hardStopToken.IsCancellationRequested)))
            {
                if (commandQueue.TryDequeue(out ICommand? command))
                {
                    try
                    {
                        command?.Execute();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка запуска комманды: {ex.Message}");
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        });

        comandThread.Start();
    }

    public void EnqueueCommand(ICommand command)
    {
        commandQueue.Enqueue(command);
    }
}

public class StartCommand : ICommand
{
    private readonly CommandProcessor _processor;

    public StartCommand(CommandProcessor processor)
    {
        _processor = processor;
    }

    public void Execute()
    {
        _processor.StartProcessingCommands();
    }
}

public class HardStopCommand : ICommand
{
    private readonly CommandProcessor _processor;

    public HardStopCommand(CommandProcessor processor)
    {
        _processor = processor;
    }

    public void Execute()
    {
        _processor?.hardStopToken?.Cancel();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly CommandProcessor _processor;

    public SoftStopCommand(CommandProcessor processor)
    {
        _processor = processor;
    }

    public void Execute()
    {
        _processor?.softStopToken?.Cancel();
    }
}
