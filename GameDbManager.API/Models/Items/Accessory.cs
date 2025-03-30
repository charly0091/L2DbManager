namespace GameDbManager.API.Models.Items
{
    public class Accessory : Item
    {
        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }
    }
}
