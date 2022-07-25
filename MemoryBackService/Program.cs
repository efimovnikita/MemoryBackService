using System;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
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

            IHost host = CreateHostBuilder(args).Build();

            host.Services.UseScheduler(scheduler =>
            {
                IScheduleInterval schedule = scheduler.Schedule<InvocableWorker>();
                schedule.DailyAtHour(17).Zoned(TimeZoneInfo.Local);
            });
            
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((_, services) =>
                {
                    services.AddScheduler();
                    services.AddTransient<InvocableWorker>();
                });
        }
    }
}