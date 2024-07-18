using Discord;
using Discord.WebSocket;
using SimpleDiscordNet.Commands;

namespace QuoteBot.Commands;

public class ConfigureQuotes {
    private static readonly string[] ValidForms = ["embed", "fake-msg", "bot-msg"];
    
    [SlashCommand("configure-quotes", "Change quote settings", GuildPermission.ManageGuild)]
    [SlashCommandArgument("channel", "The quotes channel to send quotes to", true, ApplicationCommandOptionType.Channel)]
    [SlashCommandArgument("quote-form", "How to display quotes: 'embed', 'fake-msg' or 'bot-msg'", true, ApplicationCommandOptionType.String)]
    public async Task Execute(SocketSlashCommand cmd, DiscordSocketClient client) {
        IGuildChannel channel = cmd.GetArgument<IGuildChannel>("channel")!;
        string form = cmd.GetArgument<string>("quote-form")!;

        if (!ValidForms.Contains(form)) {
            await cmd.RespondWithEmbedAsync("Configure Quotes", "Invalid quote form, must be 'embed', 'fake-msg' or 'bot-msg'.", ResponseType.Error, ephemeral: false);
            return;
        }

        if (cmd.Channel.GetChannelType() == ChannelType.DM) {
            await cmd.RespondWithEmbedAsync("Configure Quotes", "You can't do this in your DMs.", ResponseType.Error, ephemeral: false);
            return;
        }

        if (channel is not ITextChannel textChannel) {
            await cmd.RespondWithEmbedAsync("Configure Quotes", "Channel must be a text channel.", ResponseType.Error, ephemeral: true);
            return;
        }
        
        Program.Storage.SetQuoteSettings(cmd.GuildId!.Value, textChannel.Id, form);
        await cmd.RespondWithEmbedAsync("Configure Quotes", "Quotes have been configured in this server.", ResponseType.Success, ephemeral: true);
    }
}