using Discord;
using Discord.WebSocket;
using SimpleDiscordNet.MessageReceived;

namespace QuoteBot;

public class ReplyQuoteListener {
    
    [MessageListener]
    public async Task MessageListen(SocketMessage msg, DiscordSocketClient client) {
        if (msg.Channel is not IGuildChannel channel) {
            return;
        }
        
        if (msg.MentionedUsers.All(u => u.Id != client.CurrentUser.Id)) {
            return;
        }

        if (msg.Reference == null || !msg.Reference.MessageId.IsSpecified) {
            await msg.Channel.SendMessageAsync("Reply to a message and ping me to quote it.",
                messageReference: msg.ToReference());
            return;
        }

        IMessage quotedMsg = await msg.Channel.GetMessageAsync(msg.Reference.MessageId.Value);
        await Quoting.QuoteMessage(client, 
            channel.GuildId, 
            quotedMsg.Author.Username, 
            msg.Author.Username,
            msg.Author.Id, 
            quotedMsg.Author.Id.ToString(), 
            quotedMsg.Content,
            quotedMsg.Author,
            quotedMsg.Channel.Id,
            quotedMsg.Id);

        await msg.AddReactionAsync(Emoji.Parse(":white_check_mark:"));
    }
}