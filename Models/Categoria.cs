using System.ComponentModel.DataAnnotations;

namespace ANNIE_SHOP.Models
{
    public class Categoria
    {
        public Categoria()
        {
            Productos = new List<Producto>();
        }

        [Key]
        public int CategoriaId { get; set; }
        [Required(ErrorMessage ="El campo Nombre es obligatorio!")]
        [StringLength(255)]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage ="El campo Descripcion es obligatorio!")]
        [StringLength(1000)]
        public string Descripcion { get; set; } = null!;
        public ICollection<Producto> Productos { get; set; }
    }
}