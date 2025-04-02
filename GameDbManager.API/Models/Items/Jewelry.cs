using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Jewelry : Item
    {
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Jewelry() : base()
        {
            Discriminator = "Jewelry";
        }
    }
}