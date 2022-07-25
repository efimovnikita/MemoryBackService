using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel.Invocable;
using LiteDB;
using MemoryBackService.Models;
using MemoryBackService.Tools;
using Microsoft.Extensions.Logging;
using Sprache;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MemoryBackService
{
    public class InvocableWorker : IInvocable
    {
        private readonly ILogger<InvocableWorker> _logger;

        public InvocableWorker(ILogger<InvocableWorker> logger)
        {
            _logger = logger;
        }
        
        public async Task Invoke()
        {
            string assemblyPath = Path.GetDirectoryName(AppContext.BaseDirectory);
            string connectionString = Path.Combine(assemblyPath!, "theses_test.db");
            _logger.LogInformation("{DbPath}", connectionString);
                
            using LiteDatabase database = new(connectionString);
            ILiteCollection<Md> theses = database.GetCollection<Md>("theses");

            await InsertIfEmpty(theses);

            Md md = GetRandomTheses(theses);

            _logger.LogInformation("{Title}", md.Headline.Title.Text);
            await SendMessageToTelegram(md);
                
            theses.Delete(md.Id);
            _logger.LogInformation("Theses count: {Count}", theses.Count());
        }

        private static Md GetRandomTheses(ILiteCollection<Md> theses)
        {
            List<Md> list = theses.Query().ToList();
            list.Shuffle();
            Md md = list.First();
            return md;
        }

        private static async Task InsertIfEmpty(ILiteCollection<Md> theses)
        {
            if (theses.Count() == 0)
            {
                theses.Insert(await ReadAndParse());
            }
        }

        private async Task SendMessageToTelegram(Md md )
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
                    ParseMode.Html);
                
                _logger.LogInformation("Telegram message send");
            }
            catch (Exception exception)
            {
                _logger.LogError("{Error}", exception);
            }
        }
        
        private static async Task<List<Md>> ReadAndParse()
        {
            string[] files = Directory.GetFiles(Program.PathToTheses, "*.md",
                SearchOption.AllDirectories);

            List<Md> mds = new();
            foreach (string file in files)
            {
                IResult<Md> result = MdParser.WholeMdParser.TryParse(await File.ReadAllTextAsync(file));
                if (result.WasSuccessful)
                {
                    mds.Add(result.Value);
                }
            }

            return mds;
        }
    }
}