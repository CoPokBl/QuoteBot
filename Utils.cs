using Discord;
using Discord.WebSocket;

namespace QuoteBot;

public static class Utils {

    public static async Task<ITextChannel?> GetQuotesChannel(this SocketSlashCommand cmd, DiscordSocketClient client) {
        ulong? channelId = Program.Storage.GetQuoteChannel(cmd.GuildId!.Value);
        if (channelId == null) {
            return null;
        }

        IChannel channel = await client.GetChannelAsync(channelId.Value);
        return channel as ITextChannel;
    }
    
    public static async Task<ITextChannel?> GetQuotesChannel(this DiscordSocketClient client, ulong guild) {
        ulong? channelId = Program.Storage.GetQuoteChannel(guild);
        if (channelId == null) {
            return null;
        }

        IChannel channel = await client.GetChannelAsync(channelId.Value);
        return channel as ITextChannel;
    }

    public static MessageReference ToReference(this SocketMessage msg) {
        return new MessageReference(msg.Id);
    }
}