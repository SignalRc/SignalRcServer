using SignalRc.Server.Host.Feature;

namespace SignalRc.Server.Host;

public static class Extensions
{
    public static IServiceCollection AddServer(this IServiceCollection services, IConfigurationRoot configuration)
    {
        // Add services to the container.
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<IMessageHandler, MessageHandler>();

// Add a singleton WebSocket manager
        services.AddSingleton<WebSocketConnectionManager>();
        return services;
    }
}