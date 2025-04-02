using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public string ItemType { get; set; }

        [JsonConstructor]
        public Item()
        {
            // Establecer el ItemType en el constructor de la clase base
            ItemType = this.GetType().Name;
        }
    }
}
