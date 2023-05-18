using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestarRapsodya.Models
{
    public class Pedido
    {
        [Key]
        public int id_Pedido { get; set; }
        [Required]
        public DateTime fecha_Pedido { get; set; }

        [Required,MaxLength(100)] 
        public string Nombre_Plato { get;set; }

        [Required]
        public int Cantidad { get; set; }

        [ForeignKey("Usuario")]
        public string Correo_Usuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Mesa")]
        public int id_Mesa { get; set; }
        public virtual Mesa Mesa { get; set; }
    }
}
