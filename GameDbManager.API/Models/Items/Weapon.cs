using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Weapon : Item
    {
        public string WeaponType { get; set; }
        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }
        public List<Stat> Stats { get; set; }
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Weapon() : base()
        {
            Discriminator = "Weapon";
            Stats = new List<Stat>();
        }
    }
}
