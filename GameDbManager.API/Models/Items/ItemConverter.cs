using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameDbManager.API.Models.Items;

public class ItemConverter : JsonConverter<Item>
{
    public override Item Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("discriminator", out JsonElement discriminatorElement))
            {
                string discriminator = discriminatorElement.GetString();

                // Crear nuevas opciones sin el ItemConverter para evitar recursión
                var newOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // Copiar todos los converters excepto ItemConverter
                foreach (var converter in options.Converters)
                {
                    if (!(converter is ItemConverter))
                    {
                        newOptions.Converters.Add(converter);
                    }
                }

                string json = root.GetRawText();
                Item item = null;

                switch (discriminator)
                {
                    case "Accessory":
                        item = JsonSerializer.Deserialize<Accessory>(json, newOptions);
                        break;
                    case "Armor":
                        item = JsonSerializer.Deserialize<Armor>(json, newOptions);
                        break;
                    case "Etc":
                        item = JsonSerializer.Deserialize<Etc>(json, newOptions);
                        break;
                    case "Jewelry":
                        item = JsonSerializer.Deserialize<Jewelry>(json, newOptions);
                        break;
                    case "Weapon":
                        item = JsonSerializer.Deserialize<Weapon>(json, newOptions);
                        break;
                    default:
                        throw new NotSupportedException($"Unsupported item type: {discriminator}");
                }

                return item;
            }
            else
            {
                throw new KeyNotFoundException("The 'Discriminator' property was not found in the JSON.");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, Item value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}