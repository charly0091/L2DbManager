using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Weapon : Item
    {
        public string WeaponType { get; set; }
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Weapon() : base()
        {
            Discriminator = "Weapon";
        }
    }
}