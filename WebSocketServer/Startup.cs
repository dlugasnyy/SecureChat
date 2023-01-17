using WebSocketServer.Middleware;

namespace WebSocketServer;
//2:30 wideo signalr
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddWebSocketManager();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        app.UseWebSockets();
        app.UseWebSocketServer();
        
        app.Run(async context =>
        {
            Debug.WriteLine("Hello from 3rd delegate;");
            await context.Response.WriteAsync("odpowiedz");
        });
    }


    
    private void WriteRequestParam(HttpContext context)
    {
        Debug.WriteLine($"Request method: {context.Request.Method}");
        Debug.WriteLine($"Request protocol: {context.Request.Protocol}");

        foreach (var h in context.Request.Headers)
        {
                Debug.WriteLine($"--> {h.Key} : {h.Value}");
        }
    }
}