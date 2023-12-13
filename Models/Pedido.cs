using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANNIE_SHOP.Models
{
    public class Pedido
    {
        [Key]
        public int PedidoId { get; set; }
        [Required]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario {get; set;} = null!;
        [Required(ErrorMessage ="El campo Fecha es obligatorio!")]
        public DateTime Fecha {get; set;}
        [Required(ErrorMessage ="El campo Estado es obligatorio!")]
        public string Estado {get; set;} = null!;
        public int DireccionIdSeleccionada { get; set; }
        [ForeignKey("DireccionIdSeleccionada")]
        public Direccion Direccion {get; set;} = null!;
        public decimal Total {get; set;}
        public ICollection<Detalle_Pedido> DetallesPedidos {get; set;} = null!;


    }
}