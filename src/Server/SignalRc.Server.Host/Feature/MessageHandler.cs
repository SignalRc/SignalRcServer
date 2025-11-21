using System.Text.Json;
using Microsoft.Extensions.Options;
using SignalRc.Helpers;
using SignalRc.Models;

namespace SignalRc.Server.Host.Feature;

public class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private SignalRcModel _signalRcModel = null!;
    private SignalRcModel _signalRcDeltaModel = null!;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(string message)
    {
        // check if its full or delta
        var doc = JsonDocument.Parse(message);
        var type = doc.RootElement.GetProperty("type").GetString();

        if (type == "full")
        {
            await HandleFullMessage(message);
        }
        else if (type == "delta")
        {
            await HandleDeltaMessage(message);
        }
    }

    private async Task HandleDeltaMessage(string message)
    {
        // validate
        var result = Validate("delta", message);
        _logger.LogInformation("HandleDeltaMessage for " + _signalRcDeltaModel.Self);
        await Task.Delay(1);
    }

    private List<string> Validate(string type, string message)
    {
        var list = new List<string>();
        if (type == "full")
        {
            _signalRcModel = JsonSerializer.Deserialize<SignalRcModel>(message)!;
            list = ValidateModel.Validate(_signalRcModel);
        }
        else
        {
            _signalRcDeltaModel = JsonSerializer.Deserialize<SignalRcModel>(message)!;
            list = ValidateModel.Validate(_signalRcDeltaModel);
        }

        return list;
    }

    private async Task HandleFullMessage(string message)
    {
        // validate
        var result = Validate("full", message);
        _logger.LogInformation("HandleDeltaMessage for " + _signalRcModel.Self);
        await Task.Delay(1);
    }
}