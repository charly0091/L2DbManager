using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDbManager.API.Models.Items
{
    public class ItemStat
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string StatType { get; set; }  // "add", "set", "sub", "enchant"
        public string Stat { get; set; }      // "pDef", "mDef", etc.
        public string Order { get; set; }     // "0x10", "0x20", etc.
        public string Value { get; set; }     // Valor numérico como string para flexibilidad

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}