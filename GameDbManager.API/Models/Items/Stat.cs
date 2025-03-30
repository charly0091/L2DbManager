using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GameDbManager.API.Models.Items
{
    public class Stat
    {
        public int Id { get; set; }  // Clave primaria
        public string Name { get; set; }
        public string Order { get; set; }
        public int Value { get; set; }
    }
}
