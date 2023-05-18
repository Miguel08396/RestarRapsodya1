using System.ComponentModel.DataAnnotations;

namespace RestarRapsodya.Models
{
    public class Rol
    {
        [Key]
        public int id_Rol { get; set; }
        [Required][StringLength(20)]
        public string nombre_Rol { get; set; }
    }
}
