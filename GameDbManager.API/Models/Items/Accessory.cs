using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Accessory : Item
    {
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Accessory() : base()
        {
            Discriminator = "Accessory";
        }
    }
}