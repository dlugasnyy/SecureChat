namespace WebSocketServer.Middleware;

public static class ServiceConfigurator
{
    public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Middleware2>();
    }

    public static IServiceCollection AddWebSocketManager(this IServiceCollection service)
    {
        service.AddSingleton<ConnectionHandler>();
        return service;
    }
}