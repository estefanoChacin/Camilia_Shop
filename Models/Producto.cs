using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANNIE_SHOP.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }
        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = null!;
        [Required(ErrorMessage ="El campo Nombre es obligatorio!")]
        [StringLength(255)]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage ="El campo Modelo es obligatorio!")]
        [StringLength(255)]
        public string Modelo { get; set; } = null!;
        [Required(ErrorMessage ="El campo Descripcion es obligatorio!")]
        [StringLength(1000)]
        public string Descripcion { get; set; } = null!;
        [Required(ErrorMessage ="El campo Precio es obligatorio!")]
        public decimal Precio { get; set; }
        [Required(ErrorMessage ="El campo imagen es obligatorio!")]
        [StringLength(255)]
        public string Imagen { get; set; } = null!;
        [Required]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; } = null!;
        [Required(ErrorMessage ="El campo Stock es obligatorio!")]
        public int Stock { get; set; }
        [Required(ErrorMessage ="El campo Marca es obligatorio!")]
        [StringLength(100)]
        public string Marca { get; set; } = null!;
        [Required]
        public bool Activo  { get; set; }
        public ICollection<Detalle_Pedido> DetallePedido {get; set;} = null!;


    }
}