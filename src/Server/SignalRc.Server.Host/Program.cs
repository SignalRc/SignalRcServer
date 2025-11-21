using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SignalRc.Server.Host;

var builder = WebApplication.CreateBuilder(args);
var computername = Environment.MachineName;
var b = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.{computername}.json", true)
    .AddEnvironmentVariables();
var configuration = b.Build();
builder.Services.AddServer(configuration);

var app = builder.Build();
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.Map("/signalrc/stream", async (HttpContext context, WebSocketConnectionManager wsManager) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var socket = await context.WebSockets.AcceptWebSocketAsync();

        await wsManager.HandleConnection(socket);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();