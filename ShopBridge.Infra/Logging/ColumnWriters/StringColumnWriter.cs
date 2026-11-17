
using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace ShopBridge.Infra.Logging.ColumnWriters;

internal class StringColumnWriter : ColumnWriterBase
{
    private readonly string _propertyName;

    public StringColumnWriter(
       string propertyName,
       NpgsqlDbType dbType)
       : base(dbType)
    {
        _propertyName = propertyName;
    }

    public override object GetValue(
        LogEvent logEvent,
        IFormatProvider? formatProvider = null)
    {
        if (!logEvent.Properties.TryGetValue(
                _propertyName,
                out var propertyValue))
        {
            return DBNull.Value;
        }

        if (propertyValue is ScalarValue scalar)
        {
            return scalar.Value?.ToString() ?? (object)DBNull.Value;
        }

        return propertyValue.ToString();
    }
}
