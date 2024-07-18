namespace QuoteBot.Data;

public record Quote(ulong Quoter, string Quotee, ulong Guild, ulong MessageId, string Text, ulong? QuotedMessageChannel = null, ulong? QuotedMessageId = null);