namespace ANNIE_SHOP.Models.ViewModel
{
    public class CarritoItemViewModel
    {
        public int ProdcutoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }
}