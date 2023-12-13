using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANNIE_SHOP.Models
{
    public class Direccion
    {
        [Key]
        public int DireccionId { get; set; }
        [Required(ErrorMessage ="El campo Direccion es obligatorio!")]
        [StringLength(50)]
        public string Address { get; set; } = null!;
        [Required(ErrorMessage ="El campo Ciudad es obligatorio!")]
        [StringLength(50)]
        public string Ciudad { get; set; } = null!;
        [Required(ErrorMessage ="El campo Departamento es obligatorio!")]
        [StringLength(50)]
        public string Departamento { get; set; } = null!;
        [Required(ErrorMessage ="El campo Codigo Postal es obligatorio!")]
        [StringLength(15)]
        public string CodigoPostal { get; set; } = null!;
        [Required]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }=null!;
    }
}