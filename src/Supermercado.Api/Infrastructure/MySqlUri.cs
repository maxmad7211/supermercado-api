using MySql.Data.MySqlClient;

namespace Supermercado.Api.Infrastructure;

/// <summary>
/// Acepta el "Service URI" que da Aiven (mysql://usuario:contraseña@host:puerto/bd?ssl-mode=REQUIRED)
/// y lo convierte a una cadena de conexión de MySQL.
/// </summary>
public static class MySqlUri
{
    public static string ToConnectionString(string value)
    {
        if (!value.StartsWith("mysql://", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        var uri = new Uri(value);
        var credentials = uri.UserInfo.Split(':', 2);
        var builder = new MySqlConnectionStringBuilder
        {
            Server = uri.Host,
            Port = (uint)(uri.Port > 0 ? uri.Port : 3306),
            Database = uri.AbsolutePath.Trim('/'),
            UserID = Uri.UnescapeDataString(credentials[0]),
            Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : "",
            SslMode = MySqlSslMode.Required,
        };
        return builder.ConnectionString;
    }
}
