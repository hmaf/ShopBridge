using Npgsql;
using ShopBridge.Application.Abstractions.Logging;

namespace ShopBridge.Infra.Logging.Metadata;

public class LogMetadataResolver(string connectionString)
    : ILogMetadataResolver
{
    public async Task<int> GetActionIdAsync(
        string controllerName,
        string actionName,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            WITH controller AS
            (
                INSERT INTO "Controllers" ("ControllerName")
                VALUES (@ControllerName)
                ON CONFLICT ("ControllerName")
                DO UPDATE SET "ControllerName" = EXCLUDED."ControllerName"
                RETURNING "Id"
            )
            INSERT INTO "Actions" ("ControllerId", "ActionName")
            SELECT "Id", @ActionName
            FROM controller
            ON CONFLICT ("ControllerId", "ActionName")
            DO UPDATE SET "ActionName" = EXCLUDED."ActionName"
            RETURNING "Id";
            """;

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "ControllerName", controllerName);

        command.Parameters.AddWithValue(
            "ActionName", actionName);

        var result = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(result);
    }
}