using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.BLL.Services;
using TelegramSmsBridge.DAL.Repository;
using TelegramSmsBridge.BLL.Services.Queries;
using TelegramSmsBridge.BLL.Services.Commands;

namespace TelegramSmsBridge.API.Controllers;

public class TelegramController : BaseApiController
{
    private readonly TelegramSettings _telegramSettings;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IMongoDbRepository<Update> _updateRepository;
    private readonly IMongoDbRepository<SmsMessage> _smsMessageRepository;
    private readonly ICacheService<SmsMessage> _smsMessageCacheService;
    private readonly ICacheService<Update> _updateCacheService;
    private readonly CommandInvoker _commandInvoker;
    private readonly QueryHandler _queryHandler;

    public TelegramController(
        IOptions<TelegramSettings> telegramSettings,
        ILogger<UpdateHandler> logger,
        ITelegramBotClient botClient,
        ICacheService<SmsMessage> smsMessageCacheService,
        ICacheService<Update> updateCacheService,
        IMongoDbRepository<SmsMessage> smsMessageRepository,
        IMongoDbRepository<Update> updateRepository)
    {
        _telegramSettings = telegramSettings.Value;
        _logger = logger;
        _botClient = botClient;
        _smsMessageCacheService = smsMessageCacheService;
        _updateCacheService = updateCacheService;
        _smsMessageRepository = smsMessageRepository;
        _updateRepository = updateRepository;
        _commandInvoker = new CommandInvoker();
        _queryHandler = new QueryHandler();
    }


    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Telegram bot was started");
    }

    [HttpGet("getChatId")]
    public async Task<ActionResult<long>> GetChatId(string userName)
    {
        var chatId = await GetChatIdForUserAsync(userName);

        if (!chatId.HasValue)
        {
            return NotFound($"No chat found for user: {userName}");
        }

        return Ok(chatId);
    }

    private async Task<long?> GetChatIdForUserAsync(string userName)
    {
        var update = await _queryHandler.HandleQueryAsync(new GetUpdateByUserNameQuery(_updateCacheService, _updateRepository, userName));
        return update?.Message?.Chat.Id;
    }

    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage([FromQuery] long chatId, [FromBody] SmsMessage message)
    {
        try
        {
            await _commandInvoker.ExecuteCommand(new AddOrUpdateSmsMessageCommand(
                _smsMessageRepository, 
                _smsMessageCacheService, 
                message
            ));

            await _botClient.SendMessage(chatId, message.ToString());
            return Ok("Message was sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error 500 Internal server exception");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] Update update,
        [FromServices] ITelegramBotClient bot,
        [FromServices] UpdateHandler handleUpdateService,
        CancellationToken ct)
    {
        if (!IsValidSecretToken(Request.Headers["X-Telegram-Bot-Api-Secret-Token"]))
        {
            return Forbid();
        }
        
        await _commandInvoker.ExecuteCommand(new AddUpdateCommand(_updateRepository, update));
        return await ProcessUpdateAsync(bot, update, handleUpdateService, ct);
    }

    private bool IsValidSecretToken(string? providedToken)
    {
        return providedToken == _telegramSettings.WebhookSecretToken;
    }

    private async Task<IActionResult> ProcessUpdateAsync(
        ITelegramBotClient bot,
        Update update,
        UpdateHandler handleUpdateService,
        CancellationToken ct)
    {
        try
        {
            await handleUpdateService.HandleUpdateAsync(bot, update, ct);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error 500 Internal server exception");

            await handleUpdateService.HandleErrorAsync(bot, ex, Telegram.Bot.Polling.HandleErrorSource.HandleUpdateError, ct);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}