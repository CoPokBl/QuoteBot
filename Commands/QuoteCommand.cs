using Discord;
using Discord.WebSocket;
using GeneralPurposeLib;
using QuoteBot.Data;
using SimpleDiscordNet.Commands;

namespace QuoteBot.Commands;

public class QuoteCommand {

    [SlashCommand("quote-user", "Quote something someone said and attribute quote to user")]
    [SlashCommandArgument("person", "The person you are quoting", true, ApplicationCommandOptionType.User)]
    [SlashCommandArgument("message", "What they said", true, ApplicationCommandOptionType.String)]
    public Task WithUser(SocketSlashCommand cmd, DiscordSocketClient client) {
        IUser quotee = cmd.GetArgument<IUser>("person")!;
        string message = cmd.GetArgument<string>("message")!;
        return Execute(cmd, client, message, quotee: quotee);
    }
    
    [SlashCommand("quote", "Quote something someone said")]
    [SlashCommandArgument("person", "The person you are quoting", true, ApplicationCommandOptionType.String)]
    [SlashCommandArgument("message", "What they said", true, ApplicationCommandOptionType.String)]
    public Task WithString(SocketSlashCommand cmd, DiscordSocketClient client) {
        string quotee = cmd.GetArgument<string>("person")!;
        string message = cmd.GetArgument<string>("message")!;
        return Execute(cmd, client, message, quoteeName: quotee);
    }

    public async Task Execute(SocketSlashCommand cmd, DiscordSocketClient client, string msg, string? quoteeName = null, IUser? quotee = null) {
        if (cmd.Channel.GetChannelType() == ChannelType.DM) {
            await cmd.RespondWithEmbedAsync("Quote", "You can't do this in your DMs.", ResponseType.Error, ephemeral: false);
            return;
        }
        
        ITextChannel? quotesChannel = await cmd.GetQuotesChannel(client);

        if (quotesChannel == null) {
            await cmd.RespondWithEmbedAsync("Error",
                "There is no quotes channel configured, ask a staff member to configure it.", ResponseType.Error,
                ephemeral: true);
            return;
        }

        await cmd.DeferAsync();

        await Quoting.QuoteMessage(client, 
            cmd.GuildId!.Value, 
            quoteeName ?? quotee!.Username, 
            cmd.User.Username,
            cmd.User.Id, 
            quoteeName ?? quotee!.Id.ToString(), 
            msg,
            quotee);

        await cmd.ModifyWithEmbedAsync("Quotes", $"Quote has been created in {quotesChannel.Mention}.",
            ResponseType.Success);
    }
}