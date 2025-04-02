using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Etc : Item
    {
        public string ItemType { get; set; }
        public bool Stackable { get; set; }
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Etc() : base()
        {
            Discriminator = "Etc";
        }
    }
}
