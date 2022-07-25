using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using MemoryBackService.Models;
using MemoryBackService.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sprache;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MemoryBackService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                string assemblyPath = Path.GetDirectoryName(AppContext.BaseDirectory);
                string connectionString = Path.Combine(assemblyPath!, "theses.db");
                _logger.LogInformation("{DbPath}", connectionString);
                
                using LiteDatabase database = new(connectionString);
                ILiteCollection<Md> theses = database.GetCollection<Md>("theses");

                if (theses.Count() == 0)
                {
                    theses.Insert(await ReadAndParse(stoppingToken));
                }

                List<Md> list = theses.Query().ToList();
                list.Shuffle();
                Md md = list.First();

                _logger.LogInformation("{Title}", md.Headline.Title.Text);
                await SendMessageToTelegram(md, stoppingToken);
                
                bool delete = theses.Delete(md.Id);
                if (delete)
                {
                    _logger.LogInformation("Deletion was successful. Theses count: {Count}", theses.Count());
                }
                else
                {
                    _logger.LogInformation("Deletion was unsuccessful");
                    break;
                }

                await Task.Delay(TimeSpan.FromHours(8), stoppingToken);
            }
        }

        private async Task SendMessageToTelegram(Md md, CancellationToken stoppingToken)
        {
            try
            {
                TelegramBotClient bot = new(Program.TelegramBotApiKey);
                StringBuilder sb = new();
                sb.AppendLine($"<b>{md.Headline.Title.Text}</b>");
                sb.AppendLine();
                sb.AppendLine(md.Text);
                sb.AppendLine();
                sb.AppendLine($"<i>{md.Headline.Book.Name}</i>");
                await bot.SendTextMessageAsync("@mybookmarks", sb.ToString(),
                    ParseMode.Html, cancellationToken: stoppingToken);
                
                _logger.LogInformation("Telegram message send");
            }
            catch (Exception exception)
            {
                _logger.LogError("{Error}", exception);
            }
        }
        
        private static async Task<List<Md>> ReadAndParse(CancellationToken stoppingToken)
        {
            string[] files = Directory.GetFiles(Program.PathToTheses, "*.md",
                SearchOption.AllDirectories);

            List<Md> mds = new();
            foreach (string file in files)
            {
                IResult<Md> result = MdParser.WholeMdParser.TryParse(await File.ReadAllTextAsync(file, stoppingToken));
                if (result.WasSuccessful)
                {
                    mds.Add(result.Value);
                }
            }

            return mds;
        }
    }
}