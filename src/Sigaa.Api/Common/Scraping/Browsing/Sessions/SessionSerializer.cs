using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sigaa.Api.Common.Scraping.Browsing.Sessions;

internal sealed class SessionSerializer : JsonConverter<Session>
{
    private const string IdProperty = nameof(ISession.Id);
    private const string CreatedAtProperty = nameof(ISession.CreatedAt);
    private const string ExpiresAtProperty = nameof(ISession.ExpiresAt);
    private const string CookiesProperty = "Cookies";
    private const string AutoRefreshProperty = nameof(ISession.AutoRefreshLifetime);

    public override Session Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected '{JsonTokenType.StartObject}' token.");
        }

        string? id = null;
        DateTimeOffset? createdAt = null;
        DateTimeOffset? expiresAt = null;
        List<Cookie>? cookies = null;
        var autoRefresh = true;

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
            {
                return new Session(
                    id ?? throw new JsonException("Session ID is missing."),
                    createdAt ?? default,
                    expiresAt ?? default,
                    autoRefresh,
                    cookies ?? []);
            }

            if (reader.TokenType is not JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected {JsonTokenType.PropertyName} token.");
            }

            var property = reader.GetString();

            reader.Read(); // Move to the property value.

            switch (property)
            {
                case IdProperty:
                    id = reader.GetString();
                    break;
                case CreatedAtProperty:
                    createdAt = reader.GetDateTimeOffset();
                    break;
                case ExpiresAtProperty:
                    expiresAt = reader.GetDateTimeOffset();
                    break;
                case CookiesProperty:
                    cookies = JsonSerializer.Deserialize<List<Cookie>>(ref reader, options);
                    break;
                case AutoRefreshProperty:
                    autoRefresh = reader.GetBoolean();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }
        throw new JsonException("Unexpected end of JSON.");
    }

    public override void Write(Utf8JsonWriter writer, Session value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(IdProperty, value.Id);
        writer.WriteString(CreatedAtProperty, value.CreatedAt);
        writer.WriteString(ExpiresAtProperty, value.ExpiresAt);
        writer.WriteBoolean(AutoRefreshProperty, value.AutoRefreshLifetime);

        // We can directly serialize the list of cookies.
        writer.WritePropertyName(CookiesProperty);
        
        JsonSerializer.Serialize(writer, value.ListCookies(), options);

        writer.WriteEndObject();
    }
}