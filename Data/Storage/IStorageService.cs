namespace QuoteBot.Data.Storage; 

public interface IStorageService {
    void Init();
    void Deinit();

    void CreateQuote(Quote quote);
    void SetQuoteSettings(ulong guild, ulong channel, string form);
    ulong? GetQuoteChannel(ulong guild);
    string? GetQuoteForm(ulong guild);
}
