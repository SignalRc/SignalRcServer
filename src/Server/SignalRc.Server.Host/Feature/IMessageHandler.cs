namespace SignalRc.Server.Host.Feature;

public interface IMessageHandler
{
    Task Handle(string message);
}