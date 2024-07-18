using Discord;
using Discord.Webhook;
using Discord.WebSocket;
using QuoteBot.Data;

namespace QuoteBot;

public class Quoting {

    public static async Task QuoteMessage(DiscordSocketClient client, 
        ulong guild, 
        string quoteeName,
        string quoterName, 
        ulong quoterId,
        string quoteeData, 
        string msg,
        IUser? quotee = null,
        ulong? quotedMsgChannel = null,
        ulong? quotedMsgId = null) {
        ITextChannel? quotesChannel = await client.GetQuotesChannel(guild);
        if (quotesChannel == null) {
            throw new Exception("Quotes channel isn't defined");
        }
        
        string form = Program.Storage.GetQuoteForm(guild)!;  // Should be defined if channel is

        IMessage createdMsg;
        switch (form) {
            case "embed": {
                EmbedBuilder embedBuilder = new();
                embedBuilder.WithTitle(quoteeName);
                embedBuilder.WithDescription($"\"{msg}\"");
                embedBuilder.WithFooter("Quoted by " + quoterName);
                embedBuilder.WithColor(Color.Green);
        
                createdMsg = await quotesChannel.SendMessageAsync(embed: embedBuilder.Build());
                break;
            }

            case "fake-msg": {
                IWebhook? webhook = (await quotesChannel.GetWebhooksAsync()).FirstOrDefault() ?? await quotesChannel.CreateWebhookAsync("Quotes");

                DiscordWebhookClient webhookClient = new(webhook);
                ulong newMsgId = await webhookClient.SendMessageAsync(text: msg, 
                    username: quoteeName,
                    avatarUrl: quotee?.GetAvatarUrl());

                createdMsg = await quotesChannel.GetMessageAsync(newMsgId);
                break;
            }
            
            case "bot-msg": {
                createdMsg = await quotesChannel.SendMessageAsync($"{quoteeName}: \"{msg}\"");
                break;
            }
            
            default:
                throw new Exception("Invalid quote form in db.");
        }
        
        
        Quote quote = new(quoterId, quoteeData, guild, createdMsg.Id, msg, quotedMsgChannel, quotedMsgId);
        Program.Storage.CreateQuote(quote);
    }
}