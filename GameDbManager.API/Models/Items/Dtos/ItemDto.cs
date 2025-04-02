using System.Collections.Generic;

namespace GameDbManager.API.Models.Items.Dtos
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
        public string ItemType { get; set; } // "Armor", "Weapon", etc.

        // Propiedades comunes (ahora en clase base)
        public string BodyPart { get; set; }
        public string Grade { get; set; }
        public bool Crystallizable { get; set; }

        // Propiedades específicas por tipo
        public string ArmorType { get; set; } // Para Armor
        public string WeaponType { get; set; } // Para Weapon
        public bool Stackable { get; set; } // Para Etc
        public string EtcItemType { get; set; } // Para Etc

        // Colecciones relacionadas
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