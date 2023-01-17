namespace WebSocketServer.Middleware;

public static class WebSocketServerMiddlewareExtension
{
    public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<WebSocketServerMiddleware>();
    }

    public static IServiceCollection AddWebSocketManager(this IServiceCollection service)
    {
        service.AddSingleton<WebSocketServerConnectionManager>();
        return service;
    }
}