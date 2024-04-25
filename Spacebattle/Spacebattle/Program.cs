class Program
{
    static async Task Main(string[] args)
    {
        var finishCommand = new finishCommand();
        var command3 = new Command3(finishCommand);
        var command2 = new Command2(command3);
        var command1 = new Command1(command2);

        await command1.ExecuteAsync();
    }
}