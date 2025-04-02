using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Etc : Item
    {
        // Propiedad específica para Etc que define el tipo (Quest, Other, etc.)
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