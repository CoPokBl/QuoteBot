using GeneralPurposeLib;
using QuoteBot.Data.Storage;
using SimpleDiscordNet;

namespace QuoteBot;

public static class Program {
    public static IStorageService Storage = null!;
    
    public static async Task Main(string[] args) {
        Logger.Init(LogLevel.Debug);
        Config config = new(DefaultConfig.Values);

        Storage = new SqliteStorageService();
        Storage.Init();

        SimpleDiscordBot bot = new(config["token"]);

        bot.Log += message => {
            Logger.Info(message.Message);
            if (message.Exception != null) {
                Logger.Info(message.Exception);
            }
            return Task.CompletedTask;
        };
        
        bot.Client.Ready += () => {
            bot.UpdateCommands();
            bot.Client.SetCustomStatusAsync("Watching for funny quotes");
            Logger.Info("Bot ready");
            return Task.CompletedTask;
        };
        await bot.StartBot();
        Logger.Info("Bot started");
        await bot.WaitAsync();
    }
}