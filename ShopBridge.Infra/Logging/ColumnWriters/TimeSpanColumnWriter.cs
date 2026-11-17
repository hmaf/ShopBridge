using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace ShopBridge.Infra.Logging.ColumnWriters;

public class TimeSpanColumnWriter : ColumnWriterBase
{
    public TimeSpanColumnWriter()
        : base(NpgsqlDbType.Interval)
    {
    }

    public override object GetValue(
        LogEvent logEvent,
        IFormatProvider? formatProvider = null)
    {
        if (logEvent.Properties.TryGetValue(
                "ExecutionTime",
                out var value)
            && value is ScalarValue scalar
            && scalar.Value is TimeSpan executionTime)
        {
            return executionTime;
        }

        return DBNull.Value;
    }
}