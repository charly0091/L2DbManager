namespace GameDbManager.API.Models.Items
{
    public class Armor : Item
    {
        public string ArmorType { get; set; }
        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }
        public List<Stat> Stats { get; set; }
    }
}
