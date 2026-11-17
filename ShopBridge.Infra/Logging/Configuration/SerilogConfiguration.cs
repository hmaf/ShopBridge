
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace ShopBridge.Infra.Logging.Configuration;

public static class SerilogConfiguration
{
    public static void Configure(
        LoggerConfiguration configuration,
        IConfiguration appConfiguration)
    {
        var connectionString =
            appConfiguration.GetConnectionString("LogsConnection")
            ?? throw new InvalidOperationException(
                "LogsConnection is not configured.");

        configuration
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "ShopBridge");

        ConfigureDatabaseSink(
            configuration,
            connectionString,
            LogType.Application,
            "ApplicationLogs",
            PostgreSqlLogColumnOptions.ApplicationColumns(),
            LogEventLevel.Information);

        ConfigureDatabaseSink(
            configuration,
            connectionString,
            LogType.BusinessError,
            "BusinessErrorLogs",
            PostgreSqlLogColumnOptions.BusinessErrorColumns(),
            LogEventLevel.Warning);

        ConfigureDatabaseSink(
            configuration,
            connectionString,
            LogType.SystemError,
            "SystemErrorLogs",
            PostgreSqlLogColumnOptions.SystemErrorColumns(),
            LogEventLevel.Error);

        configuration.WriteTo.Console();
    }

    private static void ConfigureDatabaseSink(
        LoggerConfiguration configuration,
        string connectionString,
        string logType,
        string tableName,
        IDictionary<string, Serilog.Sinks.PostgreSQL.ColumnWriterBase> columns,
        LogEventLevel minimumLevel)
    {
        configuration.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(logEvent =>
                HasLogType(logEvent, logType))
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: tableName,
                columnOptions: columns,
                needAutoCreateTable: false,
                respectCase: true,
                restrictedToMinimumLevel: minimumLevel));
    }

    private static bool HasLogType(
        LogEvent logEvent,
        string expectedType)
    {
        return logEvent.Properties.TryGetValue(
                   "LogType", out var value)
               && value is ScalarValue scalar
               && string.Equals(
                   scalar.Value?.ToString(),
                   expectedType,
                   StringComparison.OrdinalIgnoreCase);
    }
}
