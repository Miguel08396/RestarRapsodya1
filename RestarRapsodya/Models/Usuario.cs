using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestarRapsodya.Models
{
    public class Usuario
    {
        [Key]
        [StringLength(80)]
        public string Correo { get; set; }
        [Required, StringLength(80)]
        public string Telefono { get; set; }

        [Required,MinLength(8),Column(TypeName = "nvarchar(MAX)")]
        public string password { get; set; }

        [Required, MinLength(3),StringLength(80)]
        public string Nombre { get; set; }

        [ForeignKey("Rol")]
        public int id_Rol { get; set; }
        public virtual Rol? Rol { get; set; }    
    }
}
