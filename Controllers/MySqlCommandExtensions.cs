using MySql.Data.MySqlClient;
using System.Data;

namespace Web_Api.Controllers;

public static class MySqlCommandExtensions
{
    public static void AddMissingStoredProcedureParameters(this MySqlCommand cmd)
    {
        if (cmd.CommandType != CommandType.StoredProcedure || cmd.Connection == null)
        {
            return;
        }

        SetConnectionCollation(cmd.Connection);

        var procedureName = cmd.CommandText.Split('.').Last().Trim('`');
        var parameterNames = GetStoredProcedureParameterNames(cmd.Connection, procedureName);
        if (parameterNames.Count == 0)
        {
            return;
        }

        var suppliedValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (MySqlParameter parameter in cmd.Parameters)
        {
            suppliedValues[Normalize(parameter.ParameterName)] = parameter.Value;
        }

        cmd.Parameters.Clear();

        foreach (var parameterName in parameterNames)
        {
            var normalizedName = Normalize(parameterName);
            cmd.Parameters.AddWithValue(
                $"@{parameterName}",
                suppliedValues.TryGetValue(normalizedName, out var value) ? value ?? DBNull.Value : DBNull.Value);
        }
    }

    private static List<string> GetStoredProcedureParameterNames(MySqlConnection connection, string procedureName)
    {
        using var schemaCommand = connection.CreateCommand();
        schemaCommand.CommandText = @"
            SELECT PARAMETER_NAME
            FROM INFORMATION_SCHEMA.PARAMETERS
            WHERE SPECIFIC_SCHEMA = DATABASE()
              AND SPECIFIC_NAME = @procedureName
            ORDER BY ORDINAL_POSITION;";
        schemaCommand.Parameters.AddWithValue("@procedureName", procedureName);

        var parameterNames = new List<string>();
        using var reader = schemaCommand.ExecuteReader();
        while (reader.Read())
        {
            parameterNames.Add(reader.GetString(0));
        }

        return parameterNames;
    }

    private static void SetConnectionCollation(MySqlConnection connection)
    {
        using var collationCommand = connection.CreateCommand();
        collationCommand.CommandText = "SET NAMES utf8mb4 COLLATE utf8mb4_general_ci;";
        collationCommand.ExecuteNonQuery();
    }

    private static string Normalize(string parameterName)
    {
        return parameterName.TrimStart('@').Replace("_", string.Empty);
    }
}
