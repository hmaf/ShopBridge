
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ShopBridge.Tests.Integration.Infrastructure;

public sealed class CustomWebApplicationFactory
    : WebApplicationFactory<global::Program>
{
    private const string DefaultConnectionKey =
        "ConnectionStrings__DefaultConnection";

    private const string LogsConnectionKey =
        "ConnectionStrings__LogsConnection";

    private readonly string? _originalDefaultConnection;
    private readonly string? _originalLogsConnection;

    public CustomWebApplicationFactory()
    {
        _originalDefaultConnection =
            Environment.GetEnvironmentVariable(DefaultConnectionKey);

        _originalLogsConnection =
            Environment.GetEnvironmentVariable(LogsConnectionKey);

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var defaultConnection =
            configuration.GetConnectionString("DefaultConnection");

        var logsConnection =
            configuration.GetConnectionString("LogsConnection");

        ValidateDatabase(
            defaultConnection,
            "DefaultConnection",
            "shopbridge_test_db");

        ValidateDatabase(
            logsConnection,
            "LogsConnection",
            "shopbridge_test_logs_db");

        Environment.SetEnvironmentVariable(
            DefaultConnectionKey,
            defaultConnection);

        Environment.SetEnvironmentVariable(
            LogsConnectionKey,
            logsConnection);
    }

    private static void ValidateDatabase(
        string? connectionString,
        string connectionName,
        string expectedDatabase)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"{connectionName} is not configured for integration tests.");
        }

        var connection =
            new NpgsqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
                connection.Database,
                expectedDatabase,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Integration tests must use '{expectedDatabase}' " +
                $"for {connectionName}.");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Environment.SetEnvironmentVariable(
                DefaultConnectionKey,
                _originalDefaultConnection);

            Environment.SetEnvironmentVariable(
                LogsConnectionKey,
                _originalLogsConnection);
        }

        base.Dispose(disposing);
    }
}
