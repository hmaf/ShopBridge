
using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace ShopBridge.Infra.Logging.ColumnWriters;

public class LocalTimestampColumnWriter : ColumnWriterBase
{
    public LocalTimestampColumnWriter()
        : base(NpgsqlDbType.Timestamp)
    {

    }

    public override object GetValue(
        LogEvent logEvent,
        IFormatProvider? formatProvider = null)
    {
        return logEvent.Timestamp.LocalDateTime;
    }
}
