namespace GameDbManager.Client.Models.Items
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int Weight { get; set; }
        public int Price { get; set; }
        public bool Sellable { get; set; }
        public bool Tradeable { get; set; }
        public bool Dropable { get; set; }
        public bool Destroyable { get; set; }
        public string ItemType { get; set; }

        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }

        public string ArmorType { get; set; }
        public string WeaponType { get; set; }
        public bool Stackable { get; set; }
        public string EtcItemType { get; set; }

        public List<ItemStatDto> Stats { get; set; } = new List<ItemStatDto>();
        public List<ItemSkillDto> Skills { get; set; } = new List<ItemSkillDto>();
    }

    public class ItemStatDto
    {
        public int Id { get; set; }
        public string StatType { get; set; }
        public string Stat { get; set; }
        public string Order { get; set; }
        public string Value { get; set; }
    }

    public class ItemSkillDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public int SkillLevel { get; set; }
    }
}