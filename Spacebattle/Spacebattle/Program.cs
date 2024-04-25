using Spacebattle;
using Spacebattle.Interfaces;

internal class Program
{
    /// <summary>
    //  Класс от интерфейса ICommand для тестирования с выводом сообщени¤ в консоль и задержкой
    /// </summary>
    public class TestCommand : ICommand
    {
        private string _message;

        public TestCommand(string message)
        {
            _message = message;
        }

        public void Execute()
        {
            Console.WriteLine(_message);
            Thread.Sleep(200);
        }
    }

    /// <summary>
    /// Тестовая консольная программа
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        CommandProcessor processor = new();
        for (int i = 1; i <= 11; i++)
        {
            processor.EnqueueCommand(new TestCommand($"Test command {i}"));
        }
        processor.StartProcessingCommands();

        var softStopCommand = new SoftStopCommand(processor);
        processor.EnqueueCommand(softStopCommand);

        Thread.Sleep(3000);

        Console.WriteLine(processor.commandQueue.Count.ToString());
    }
}