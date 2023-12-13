using System.ComponentModel.DataAnnotations;

namespace ANNIE_SHOP.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        [Required(ErrorMessage ="El campo Nombre es obligatorio!")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
    }
}