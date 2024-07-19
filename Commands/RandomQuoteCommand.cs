using Discord;
using Discord.WebSocket;
using QuoteBot.Data;
using SimpleDiscordNet.Commands;

namespace QuoteBot.Commands;

public class RandomQuoteCommand {

    [SlashCommand("random-quote", "Display a random quote from this server.")]
    public async Task Execute(SocketSlashCommand cmd, DiscordSocketClient client) {
        if (cmd.Channel.GetChannelType() == ChannelType.DM) {
            await cmd.RespondWithEmbedAsync("Quote", "You can't do this in your DMs.", ResponseType.Error, ephemeral: false);
            return;
        }
        
        Quote? quote = Program.Storage.GetRandomQuote(cmd.GuildId!.Value);

        if (quote == null) {
            await cmd.RespondWithEmbedAsync("Quote",
                "This server has no quotes yet, quote someone by running /quote, /quote-user to by replying to a message and pinging me.",
                ResponseType.Error);
            return;
        }

        bool hasId = ulong.TryParse(quote.Quotee, out ulong quoteeId);
        IUser? quotee = hasId ? await client.GetUserAsync(quoteeId) : null;
        string quoteeDisplay = quotee?.Username ?? quote.Quotee;

        IUser? quoter = await client.GetUserAsync(quote.Quoter);
        string quoterDisplay = quoter?.Username ?? quote.Quoter.ToString();
        
        EmbedBuilder embedBuilder = new();
        embedBuilder.WithTitle(quoteeDisplay);
        embedBuilder.WithDescription($"\"{quote.Text}\"");
        embedBuilder.WithFooter("Quoted by " + quoterDisplay);
        embedBuilder.WithColor(Color.Green);

        await cmd.RespondAsync(embed: embedBuilder.Build());
    }
}