using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace GameDbManager.API.Models.Items
{
    [JsonConverter(typeof(ItemConverter))]
    public abstract class Item
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

        // Campos unificados (anteriormente duplicados en subclases)
        public string? BodyPart { get; set; }
        public string? Grade { get; set; }
        public bool? Crystallizable { get; set; }

        // Colecciones de navegación para stats y skills
        public virtual ICollection<ItemStat> Stats { get; set; }
        public virtual ICollection<ItemSkill> Skills { get; set; }

        [JsonConstructor]
        public Item()
        {
            Stats = new List<ItemStat>();
            Skills = new List<ItemSkill>();
        }
    }
}