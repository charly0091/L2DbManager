using System.ComponentModel.DataAnnotations;

namespace GameDbManager.API.Models.Items
{
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
    }
}
