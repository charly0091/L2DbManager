using System.Text.Json.Serialization;

namespace GameDbManager.API.Models.Items
{
    public class Jewelry : Item
    {
        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }
        public List<Stat> Stats { get; set; }
        public string Discriminator { get; private set; }

        [JsonConstructor]
        public Jewelry() : base()
        {
            Discriminator = "Jewelry";
            Stats = new List<Stat>();
        }
    }
}
