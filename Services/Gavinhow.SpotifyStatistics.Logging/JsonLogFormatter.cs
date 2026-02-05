using System.Buffers;
using System.Text;
using System.Text.Json;
using Serilog.Events;
using Serilog.Formatting;

namespace Gavinhow.SpotifyStatistics.Logging;

public sealed class JsonLogFormatter : ITextFormatter
{
    private static readonly HashSet<string> ReservedFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "timestamp",
        "level",
        "message",
        "service",
        "environment",
        "http",
        "trace_id",
        "span_id",
        "error"
    };

    private static readonly HashSet<string> HttpPropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Method",
        "Path",
        "StatusCode",
        "Protocol",
        "Scheme",
        "Host",
        "ElapsedMilliseconds",
        "Elapsed",
        "Duration"
    };

    private readonly string? _serviceName;
    private readonly string? _environment;

    public JsonLogFormatter(string? serviceName, string? environment)
    {
        _serviceName = serviceName;
        _environment = environment;
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        writer.WriteStartObject();
        writer.WriteString("timestamp", logEvent.Timestamp.ToUniversalTime().ToString("O"));
        writer.WriteString("level", MapLevel(logEvent.Level));
        writer.WriteString("message", logEvent.RenderMessage());

        WriteOptionalString(writer, "service", GetScalarString(logEvent, "service") ?? _serviceName);
        WriteOptionalString(writer, "environment", GetScalarString(logEvent, "environment") ?? _environment);

        if (TryGetScalar(logEvent, "TraceId", out var traceId) && !IsNullOrEmpty(traceId))
        {
            writer.WriteString("trace_id", Convert.ToString(traceId, System.Globalization.CultureInfo.InvariantCulture));
        }

        if (TryGetScalar(logEvent, "SpanId", out var spanId) && !IsNullOrEmpty(spanId))
        {
            writer.WriteString("span_id", Convert.ToString(spanId, System.Globalization.CultureInfo.InvariantCulture));
        }

        var http = BuildHttpObject(logEvent);
        if (http is { Count: > 0 })
        {
            writer.WritePropertyName("http");
            WriteObject(writer, http);
        }

        if (logEvent.Exception is not null)
        {
            writer.WritePropertyName("error");
            writer.WriteStartObject();
            WriteOptionalString(writer, "type", logEvent.Exception.GetType().Name);
            WriteOptionalString(writer, "message", logEvent.Exception.Message);
            WriteOptionalString(writer, "stack_trace", logEvent.Exception.StackTrace);
            writer.WriteEndObject();
        }

        foreach (var property in logEvent.Properties)
        {
            if (IsInternalProperty(property.Key))
            {
                continue;
            }

            if (HttpPropertyNames.Contains(property.Key))
            {
                continue;
            }

            if (property.Key.Equals("TraceId", StringComparison.OrdinalIgnoreCase)
                || property.Key.Equals("SpanId", StringComparison.OrdinalIgnoreCase)
                || property.Key.Equals("service", StringComparison.OrdinalIgnoreCase)
                || property.Key.Equals("environment", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var key = ToSnakeCase(property.Key);
            if (ReservedFields.Contains(key))
            {
                continue;
            }

            var value = ToSimpleValue(property.Value);
            if (IsNullOrEmpty(value))
            {
                continue;
            }

            WriteProperty(writer, key, value);
        }

        writer.WriteEndObject();
        writer.Flush();

        output.Write(Encoding.UTF8.GetString(buffer.WrittenSpan));
        output.WriteLine();
    }

    private static Dictionary<string, object?> BuildHttpObject(LogEvent logEvent)
    {
        var http = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        TryAddHttpProperty(http, logEvent, "Method", "method");
        TryAddHttpProperty(http, logEvent, "Path", "path");
        TryAddHttpProperty(http, logEvent, "StatusCode", "status_code");
        TryAddHttpProperty(http, logEvent, "Protocol", "protocol");
        TryAddHttpProperty(http, logEvent, "Scheme", "scheme");
        TryAddHttpProperty(http, logEvent, "Host", "host");

        if (TryGetScalar(logEvent, "ElapsedMilliseconds", out var elapsedMs) && !IsNullOrEmpty(elapsedMs))
        {
            http["duration_ms"] = ToDurationMilliseconds(elapsedMs);
        }
        else if (TryGetScalar(logEvent, "Elapsed", out var elapsed) && !IsNullOrEmpty(elapsed))
        {
            http["duration_ms"] = ToDurationMilliseconds(elapsed);
        }
        else if (TryGetScalar(logEvent, "Duration", out var duration) && !IsNullOrEmpty(duration))
        {
            http["duration_ms"] = ToDurationMilliseconds(duration);
        }

        return http;
    }

    private static void TryAddHttpProperty(IDictionary<string, object?> http, LogEvent logEvent, string sourceName, string targetName)
    {
        if (!TryGetScalar(logEvent, sourceName, out var value))
        {
            return;
        }

        if (IsNullOrEmpty(value))
        {
            return;
        }

        http[targetName] = value;
    }

    private static object? ToDurationMilliseconds(object value)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds;
        }

        return value;
    }

    private static string MapLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "debug",
            LogEventLevel.Debug => "debug",
            LogEventLevel.Information => "info",
            LogEventLevel.Warning => "warning",
            LogEventLevel.Error => "error",
            LogEventLevel.Fatal => "fatal",
            _ => "info"
        };
    }

    private static bool IsInternalProperty(string name)
    {
        if (name.StartsWith("@", StringComparison.Ordinal))
        {
            return true;
        }

        return name.Equals("SourceContext", StringComparison.OrdinalIgnoreCase)
            || name.Equals("MessageTemplate", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetScalarString(LogEvent logEvent, string name)
    {
        if (!TryGetScalar(logEvent, name, out var value))
        {
            return null;
        }

        return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    private static bool TryGetScalar(LogEvent logEvent, string name, out object? value)
    {
        if (logEvent.Properties.TryGetValue(name, out var property))
        {
            value = ToSimpleValue(property);
            return true;
        }

        value = null;
        return false;
    }

    private static object? ToSimpleValue(LogEventPropertyValue propertyValue)
    {
        return propertyValue switch
        {
            ScalarValue scalar => scalar.Value,
            SequenceValue sequence => sequence.Elements.Select(ToSimpleValue).ToList(),
            StructureValue structure => structure.Properties.ToDictionary(
                prop => ToSnakeCase(prop.Name),
                prop => ToSimpleValue(prop.Value),
                StringComparer.OrdinalIgnoreCase),
            DictionaryValue dictionary => dictionary.Elements.ToDictionary(
                kvp => ToSnakeCase(Convert.ToString(ToSimpleValue(kvp.Key), System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty),
                kvp => ToSimpleValue(kvp.Value),
                StringComparer.OrdinalIgnoreCase),
            _ => propertyValue.ToString()
        };
    }

    private static bool IsNullOrEmpty(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        if (value is IReadOnlyCollection<object?> list)
        {
            return list.Count == 0;
        }

        if (value is IDictionary<string, object?> dict)
        {
            return dict.Count == 0;
        }

        return false;
    }

    private static void WriteOptionalString(Utf8JsonWriter writer, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            writer.WriteString(name, value);
        }
    }

    private static void WriteProperty(Utf8JsonWriter writer, string name, object? value)
    {
        if (IsNullOrEmpty(value))
        {
            return;
        }

        writer.WritePropertyName(name);
        WriteValue(writer, value);
    }

    private static void WriteObject(Utf8JsonWriter writer, IDictionary<string, object?> values)
    {
        writer.WriteStartObject();
        foreach (var (key, value) in values)
        {
            if (IsNullOrEmpty(value))
            {
                continue;
            }

            writer.WritePropertyName(key);
            WriteValue(writer, value);
        }
        writer.WriteEndObject();
    }

    private static void WriteValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case string text:
                writer.WriteStringValue(text);
                break;
            case bool boolean:
                writer.WriteBooleanValue(boolean);
                break;
            case byte number:
                writer.WriteNumberValue(number);
                break;
            case sbyte number:
                writer.WriteNumberValue(number);
                break;
            case short number:
                writer.WriteNumberValue(number);
                break;
            case ushort number:
                writer.WriteNumberValue(number);
                break;
            case int number:
                writer.WriteNumberValue(number);
                break;
            case uint number:
                writer.WriteNumberValue(number);
                break;
            case long number:
                writer.WriteNumberValue(number);
                break;
            case ulong number:
                writer.WriteNumberValue(number);
                break;
            case float number:
                writer.WriteNumberValue(number);
                break;
            case double number:
                writer.WriteNumberValue(number);
                break;
            case decimal number:
                writer.WriteNumberValue(number);
                break;
            case DateTime dateTime:
                writer.WriteStringValue(dateTime.ToUniversalTime().ToString("O"));
                break;
            case DateTimeOffset dateTimeOffset:
                writer.WriteStringValue(dateTimeOffset.ToUniversalTime().ToString("O"));
                break;
            case Guid guid:
                writer.WriteStringValue(guid.ToString("D"));
                break;
            case IDictionary<string, object?> dict:
                WriteObject(writer, dict);
                break;
            case IEnumerable<object?> list:
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    WriteValue(writer, item);
                }
                writer.WriteEndArray();
                break;
            default:
                writer.WriteStringValue(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                break;
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                var hasPrev = i > 0;
                var prev = hasPrev ? value[i - 1] : '\0';
                var hasNext = i + 1 < value.Length;
                var next = hasNext ? value[i + 1] : '\0';

                if (hasPrev && (char.IsLower(prev) || char.IsDigit(prev) || (char.IsUpper(prev) && hasNext && char.IsLower(next))))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(c));
                continue;
            }

            if (c == '-' || c == ' ')
            {
                builder.Append('_');
                continue;
            }

            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }
}
