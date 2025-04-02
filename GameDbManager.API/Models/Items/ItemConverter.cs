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
                switch (discriminator)
                {
                    case "Accessory":
                        return JsonSerializer.Deserialize<Accessory>(root.GetRawText(), options);
                    case "Armor":
                        return JsonSerializer.Deserialize<Armor>(root.GetRawText(), options);
                    case "Etc":
                        return JsonSerializer.Deserialize<Etc>(root.GetRawText(), options);
                    case "Jewelry":
                        return JsonSerializer.Deserialize<Jewelry>(root.GetRawText(), options);
                    case "Weapon":
                        return JsonSerializer.Deserialize<Weapon>(root.GetRawText(), options);
                    default:
                        throw new NotSupportedException($"Unsupported item type: {discriminator}");
                }
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