using Spacebattle;
using ICommand = Spacebattle.Interfaces.ICommand;

namespace SpacebattleTests
{
    public class UnitTests
    {
        /// <summary>
        // Тест, который проверяет, что после команды старт поток запущен
        /// </summary>
        [Fact]
        public void Test_StartThread_ShouldRun()
        {
            CommandProcessor processor = new();
            for (int i = 1; i <= 11; i++)
            {
                processor.EnqueueCommand(new TestCommand($"Test command {i}"));
            }
            processor.StartProcessingCommands();
            Thread.Sleep(1000);
            
            Assert.True(processor?.comandThread?.ThreadState == ThreadState.Running || processor?.comandThread?.ThreadState == ThreadState.WaitSleepJoin);

            var softStopCommand = new SoftStopCommand(processor);
            processor.EnqueueCommand(softStopCommand);
        }

        /// <summary>
        // Тест для проверки завершения потока командой soft stop. Поток должен завершиться только после того, как все команды закончились
        /// </summary>
        [Fact]
        public void Test_SoftStopThread_ShouldPassed()
        {
            CommandProcessor processor = new();
            for (int i = 1; i <= 5; i++)
            {
                processor.EnqueueCommand(new TestCommand($"Test command {i}"));
            }
            var softStopCommand = new SoftStopCommand(processor);
            processor.EnqueueCommand(softStopCommand);
            for (int i = 1; i <= 5; i++)
            {
                processor.EnqueueCommand(new TestCommand($"Test command {i}"));
            }
            processor.StartProcessingCommands();
            Thread.Sleep(3000);

            Assert.True(processor?.comandThread?.ThreadState == ThreadState.Stopped);
            Assert.True(processor?.commandQueue.IsEmpty);
        }

        /// <summary>
        // Тест для проверки моментального завершения потока командой hard stop
        /// </summary>
        [Fact]
        public void Test_HardStopThread_ShouldPassed()
        {
            CommandProcessor processor = new();
            for (int i = 1; i <= 10; i++)
            {
                processor.EnqueueCommand(new TestCommand($"Test command {i}"));
            }
            var hardStopCommand = new HardStopCommand(processor);
            processor.EnqueueCommand(hardStopCommand);
            for (int i = 1; i <= 10; i++)
            {
                processor.EnqueueCommand(new TestCommand($"Test command {i}"));
            }
            processor.StartProcessingCommands();
            Thread.Sleep(3000);

            Assert.True(processor?.comandThread?.ThreadState == ThreadState.Stopped);
            Assert.False(processor?.commandQueue.IsEmpty);

        }
    }

    /// <summary>
    //  Класс от интерфейса ICommand для тестирования с выводом сообщения в консоль и задержкой
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
}

