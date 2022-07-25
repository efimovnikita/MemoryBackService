using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemoryBackService
{
    public static class Program
    {
        internal static string PathToTheses;
        internal static string TelegramBotApiKey;
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("Service need arguments");
            }

            PathToTheses = args[0];
            TelegramBotApiKey = args[1];
            
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) => { services.AddHostedService<Worker>(); });
        }
    }
}