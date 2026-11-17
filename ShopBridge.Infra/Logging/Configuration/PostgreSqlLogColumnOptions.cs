
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;
using ShopBridge.Infra.Logging.ColumnWriters;

namespace ShopBridge.Infra.Logging.Configuration;

public static class PostgreSqlLogColumnOptions
{
    public static IDictionary<string, ColumnWriterBase>
        ApplicationColumns()
    {
        var columns = CommonColumns();

        columns["Response"] = new SinglePropertyColumnWriter(
            "Response",
            PropertyWriteMethod.Raw,
            NpgsqlDbType.Jsonb);

        columns["ExecutionTime"] = new TimeSpanColumnWriter();

        return columns;
    }

    public static IDictionary<string, ColumnWriterBase>
        BusinessErrorColumns()
    {
        var columns = CommonColumns();

        columns["ErrorCode"] = new SinglePropertyColumnWriter(
            "ErrorCode",
            PropertyWriteMethod.Raw,
            NpgsqlDbType.Integer);

        columns["ErrorMessage"] = new StringColumnWriter(
            "ErrorMessage",
            NpgsqlDbType.Varchar);

        return columns;
    }

    public static IDictionary<string, ColumnWriterBase>
        SystemErrorColumns()
    {
        var columns = CommonColumns();

        columns["ErrorMessage"] = new StringColumnWriter(
            "ErrorMessage",
            NpgsqlDbType.Text);

        columns["StackTrace"] = new StringColumnWriter(
            "StackTrace",
            NpgsqlDbType.Text);

        return columns;
    }

    private static Dictionary<string, ColumnWriterBase>
        CommonColumns()
    {
        return new Dictionary<string, ColumnWriterBase>
        {
            ["ActionId"] = new SinglePropertyColumnWriter(
                "ActionId",
                PropertyWriteMethod.Raw,
                NpgsqlDbType.Integer),

            ["UserId"] = new SinglePropertyColumnWriter(
                "UserId",
                PropertyWriteMethod.Raw,
                NpgsqlDbType.Uuid),

            ["Request"] = new SinglePropertyColumnWriter(
                "Request",
                PropertyWriteMethod.Raw,
                NpgsqlDbType.Jsonb),

            ["CreatedAt"] = new LocalTimestampColumnWriter()
        };
    }
}
