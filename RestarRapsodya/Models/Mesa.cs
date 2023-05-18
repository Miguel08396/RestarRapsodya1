using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestarRapsodya.Models
{
    public class Mesa
    {
        [Key]
        public int id_Mesa { get; set; }
        [Required]
        public int Capacidad { get; set; }
        [ForeignKey("Estado")]
        public int id_Estado { get; set; }
        public virtual Estado? Estado { get; set; }
    }
}
