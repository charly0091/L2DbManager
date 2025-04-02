using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDbManager.API.Models.Items
{
    public class ItemSkill
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int SkillId { get; set; }
        public int SkillLevel { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}