using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Armor : Item
    {
        public string ArmorType { get; set; }
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Armor() : base()
        {
            Discriminator = "Armor";
        }
    }
}