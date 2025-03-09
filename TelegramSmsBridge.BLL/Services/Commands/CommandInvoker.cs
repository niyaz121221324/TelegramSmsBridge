namespace TelegramSmsBridge.BLL.Services.Commands;

public class CommandInvoker
{
    public async Task ExecuteCommand(ICommand command)
    {
        await command.ExecuteAsync();
    }        
}