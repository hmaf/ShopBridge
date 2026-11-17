using Npgsql;

namespace ShopBridge.Infra.Logging.Metadata;

public class LogMetadataInitializer(string connectionString)
{
    public async Task Initialize(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS "Controllers"
            (
                "Id" SERIAL PRIMARY KEY,
                "ControllerName" VARCHAR(200) NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS "Actions"
            (
                "Id" SERIAL PRIMARY KEY,
                "ControllerId" INTEGER NOT NULL
                    REFERENCES "Controllers"("Id"),
                "ActionName" VARCHAR(200) NOT NULL,
                CONSTRAINT "UQ_Actions_ControllerId_ActionName"
                    UNIQUE ("ControllerId", "ActionName")
            );

            CREATE TABLE IF NOT EXISTS "ApplicationLogs"
            (
                "Id" BIGSERIAL PRIMARY KEY,
                "ActionId" INTEGER NOT NULL REFERENCES "Actions"("Id"),
                "UserId" UUID NULL,
                "Request" JSONB NULL,
                "Response" JSONB NULL,
                "ExecutionTime" INTERVAL NOT NULL,
                "CreatedAt" TIMESTAMP WITHOUT TIME ZONE
                    NOT NULL DEFAULT LOCALTIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS "BusinessErrorLogs"
            (
                "Id" BIGSERIAL PRIMARY KEY,
                "ActionId" INTEGER NOT NULL REFERENCES "Actions"("Id"),
                "UserId" UUID NULL,
                "Request" JSONB NULL,
                "ErrorCode" INTEGER NOT NULL,
                "ErrorMessage" VARCHAR(1000) NOT NULL,
                "CreatedAt" TIMESTAMP WITHOUT TIME ZONE
                    NOT NULL DEFAULT LOCALTIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS "SystemErrorLogs"
            (
                "Id" BIGSERIAL PRIMARY KEY,
                "ActionId" INTEGER NOT NULL REFERENCES "Actions"("Id"),
                "UserId" UUID NULL,
                "Request" JSONB NULL,
                "ErrorMessage" TEXT NOT NULL,
                "StackTrace" TEXT NULL,
                "CreatedAt" TIMESTAMP WITHOUT TIME ZONE
                    NOT NULL DEFAULT LOCALTIMESTAMP
            );
            """;

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new NpgsqlCommand(sql, connection);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}