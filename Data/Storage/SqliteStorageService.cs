using System.Data.SQLite;

namespace QuoteBot.Data.Storage;

public class SqliteStorageService : IStorageService {
    private const string ConnectionString = "Data Source=db.dat";
    private SQLiteConnection _connection = null!;
    
    public void Init() {
        _connection = new SQLiteConnection(ConnectionString);
        _connection.Open();
        CreateTables();
    }

    private void CreateTables() {
        SQLiteCommand cmd = new(@"
CREATE TABLE IF NOT EXISTS quotes (
    quoter VARCHAR(64), 
    quotee VARCHAR(64),
    guild VARCHAR(64),
    message_id VARCHAR(64),
    quote TEXT,
    quoted_message_channel VARCHAR(64),
    quoted_message_id VARCHAR(64)
);

CREATE TABLE IF NOT EXISTS guild_configs (
    guild VARCHAR(64) PRIMARY KEY,
    channel_id VARCHAR(64),
    form VARCHAR(16)
)
", _connection);
        cmd.ExecuteNonQuery();
    }
    
    public void Deinit() {
        _connection.Dispose();
    }

    public void CreateQuote(Quote quote) {
        using SQLiteCommand cmd = new("INSERT INTO quotes (quoter, quotee, guild, message_id, quote, quoted_message_channel, quoted_message_id) " +
                                      "VALUES (@quoter, @quotee, @guild, @message_id, @quote, @quoted_message_channel, @quoted_message_id);", _connection);
        cmd.Parameters.AddWithValue("quoter", quote.Quoter.ToString());
        cmd.Parameters.AddWithValue("quotee", quote.Quotee);
        cmd.Parameters.AddWithValue("guild", quote.Guild.ToString());
        cmd.Parameters.AddWithValue("message_id", quote.MessageId.ToString());
        cmd.Parameters.AddWithValue("quote", quote.Text);
        cmd.Parameters.AddWithValue("quoted_message_channel", quote.QuotedMessageChannel.ToString());
        cmd.Parameters.AddWithValue("quoted_message_id", quote.QuotedMessageId.ToString());
        cmd.ExecuteNonQuery();
    }
    
    public void SetQuoteSettings(ulong guild, ulong channel, string form) {
        using SQLiteCommand cmd = new("INSERT OR REPLACE INTO guild_configs (guild, channel_id, form) VALUES (@guild, @channel, @form);", _connection);
        cmd.Parameters.AddWithValue("guild", guild.ToString());
        cmd.Parameters.AddWithValue("channel", channel.ToString());
        cmd.Parameters.AddWithValue("form", form);
        cmd.ExecuteNonQuery();
    }
    
    public ulong? GetQuoteChannel(ulong guild) {
        using SQLiteCommand cmd = new("SELECT channel_id FROM guild_configs WHERE guild = @guild;", _connection);
        cmd.Parameters.AddWithValue("guild", guild.ToString());
        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) {
            return null;
        }
        return ulong.Parse(reader.GetString(0));
    }
    
    public string? GetQuoteForm(ulong guild) {
        using SQLiteCommand cmd = new("SELECT form FROM guild_configs WHERE guild = @guild;", _connection);
        cmd.Parameters.AddWithValue("guild", guild.ToString());
        using SQLiteDataReader reader = cmd.ExecuteReader();
        if (!reader.Read()) {
            return null;
        }
        return reader.GetString(0);
    }
}