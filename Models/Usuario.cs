using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ANNIE_SHOP.Models
{
    public class Usuario
    {
        public Usuario()
        {
            Pedidos = new List<Pedido>();
        }

        [Key]
        public int UsuarioId { get; set; }
        [Required(ErrorMessage ="El campo Nombre es obligatorio!")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage ="El campo telefono es obligatorio!")]
        [StringLength(15)]
        public string Telefono { get; set; } = null!;
        [Required(ErrorMessage ="El campo Nombre de Usuario es obligatorio!")]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = null!;
        [Required(ErrorMessage ="El campo Contraseña es obligatorio!")]
        [StringLength(255)]
        public string Contrasenia { get; set; } = null!;
        [Required(ErrorMessage ="El campo Correo es obligatorio!")]
        [StringLength(255)]
        public string Correo { get; set; } = null!;
        [Required(ErrorMessage ="El campo Direccion es obligatorio!")]
        [StringLength(100)]
        public string Direccion { get; set; } = null!;
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
        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol Rol {get; set;} = null!;
        public ICollection<Pedido> Pedidos {get; set;}
        [InverseProperty("Usuario")]
        public ICollection<Direccion> Direcciones {get; set;}=null!;

    }
}