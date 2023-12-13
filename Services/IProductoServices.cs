using ANNIE_SHOP.Models;
using ANNIE_SHOP.Models.ViewModel;

namespace ANNIE_SHOP.Services
{
    public interface IProductoServices
    {
        Producto GetProducto(int id);

        Task<List<Producto>> GetProductosDestacados();

        Task<ProductosPaginadosViewModel> GetProductoPaginados(int? categoriaId, string? busqueda, int pagina, int productosPorPaginas);
    }
}