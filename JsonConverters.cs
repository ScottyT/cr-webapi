using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cr_app_webapi
{
    public class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type? typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            var dictionary = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JsonTokenType was not PropertyName");
                }

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                /* if (!ValidExtensions.IsValid<object>(value))
                {
                    throw new JsonException("Value is not valid");
                } */

                reader.Read();
                dictionary.Add(propertyName, ExtractValue(ref reader, options));
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }

        private object? ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (!ValidExtensions.IsValid<JsonTokenType>(reader.TokenType))
            {
                return false;
            }
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var date))
                    {
                        return date;
                    }
                    return reader.GetString();
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var result))
                    {
                        return result;
                    }
                    return reader.GetDecimal();
                case JsonTokenType.StartObject:
                    return Read(ref reader, null, options);
                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ExtractValue(ref reader, options));
                    }
                    return list;

                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }

    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
                JsonTokenType.Number => reader.GetDouble(),
                JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
                JsonTokenType.String => reader.GetString()!,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };
        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }
    public class DictionaryObjectJsonModelBinder : IModelBinder
    {
        private readonly ILogger<DictionaryObjectJsonModelBinder> _logger;

        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            Converters = { new DictionaryStringObjectJsonConverter() }
        };

        public DictionaryObjectJsonModelBinder(ILogger<DictionaryObjectJsonModelBinder> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.ModelType != typeof(Dictionary<string, object>))
            {
                throw new NotSupportedException($"The '{nameof(DictionaryObjectJsonModelBinder)}' model binder should only be used on Dictionary<string, object>, it will not work on '{bindingContext.ModelType.Name}'");
            }

            try
            {
                var data = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(bindingContext.HttpContext.Request.Body, DefaultJsonSerializerOptions);
                bindingContext.Result = ModelBindingResult.Success(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when trying to model bind Dictionary<string, object>");
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
    }
}