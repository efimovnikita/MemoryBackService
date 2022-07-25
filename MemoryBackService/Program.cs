using System;
using System.CommandLine;
using System.Threading.Tasks;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MemoryBackService
{
    public static class Program
    {
        public static async Task<int>  Main(string[] args)
        {
            Option<string> pathOption = new("--path", description: "Path to theses folder") {IsRequired = true};
            Option<string> apiOption = new("--api", description: "Telegram bot API key") {IsRequired = true};

            RootCommand rootCommand = new("MemoryBack service");
            rootCommand.AddOption(pathOption);
            rootCommand.AddOption(apiOption);
            
            rootCommand.SetHandler(async (path, api) =>
            {
                IHost host = CreateHostBuilder(args, path, api).Build();
                host.Services.UseScheduler(scheduler =>
                {
                    IScheduleInterval schedule = scheduler.Schedule<InvocableWorker>();
                    schedule.DailyAtHour(17).Zoned(TimeZoneInfo.Local);
                });
                await host.RunAsync();
            }, pathOption, apiOption);
            
            return await rootCommand.InvokeAsync(args);
        }

        private static IHostBuilder CreateHostBuilder(string[] args, string path, string api)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((_, services) =>
                {
                    services.AddScheduler();
                    services.AddTransient(provider =>
                    {
                        ILogger<InvocableWorker> logger = provider.GetService<ILogger<InvocableWorker>>();
                        return new InvocableWorker(logger, path, api);
                    });
                });
        }
    }
}