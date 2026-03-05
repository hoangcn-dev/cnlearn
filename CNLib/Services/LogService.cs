using Microsoft.Extensions.DependencyInjection;

namespace CNLib.Services
{
    public interface ILogService<T>
    {
        void LogInfo(string message);
        void LogSuccess(string message);
        void LogError(string message);
    }

    public class ConsoleLogService<T> : ILogService<T>
    {
        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        public void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SUCCESS] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public static class LogServiceExtension
    {
        public static IServiceCollection AddConsoleLog(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ILogService<>), typeof(ConsoleLogService<>));
            return services;
        }
    }
}
