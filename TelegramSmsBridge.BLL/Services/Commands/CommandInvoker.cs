namespace TelegramSmsBridge.BLL.Services.Commands;

public class CommandInvoker
{
    public async Task ExecuteCommandAsync(ICommand command)
    {
        await command.ExecuteAsync();
    }        
}