using System.ComponentModel.DataAnnotations;

namespace RestarRapsodya.Models
{
    public class Estado
    {
        [Key]
        public int id_Estado { get; set;}
        [Required][StringLength(10)]
        public string estado { get; set;} 

    }
}
