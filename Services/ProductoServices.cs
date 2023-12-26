
using ANNIE_SHOP.Models;
using ANNIE_SHOP.Models.ViewModel;
using ANNIE_SHOP.Data;
using Microsoft.EntityFrameworkCore;
namespace ANNIE_SHOP.Services
{
    public class ProductoServices : IProductoServices
    {
        private readonly ApplicationDbContext _context;
        public ProductoServices(ApplicationDbContext context)
        {
            _context = context;
        }




        public Producto GetProducto(int id)
        {
            var productoFind = _context.Productos
            .Include(e => e.Categoria)
            .FirstOrDefault(p => p.ProductoId == id);

            if (productoFind != null)
                return productoFind;

            return new Producto();
        }





        public async Task<ProductosPaginadosViewModel> GetProductoPaginados(int? categoriaId, string? busqueda, int pagina, int productosPorPaginas)
        {
            IQueryable<Producto> query = _context.Productos;
            query = query.Where(p=> p.Activo);

            if(categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId);

            if(!string.IsNullOrEmpty(busqueda))
                query = query.Where(p=>p.Nombre.Contains(busqueda) || p.Descripcion.Contains(busqueda));
            
            int totalProductos = await query.CountAsync();
            int TotalPaginas = (int)Math.Ceiling((double)totalProductos/productosPorPaginas);

            if(pagina<1)
                pagina = 1;
            else if(pagina > TotalPaginas)
                pagina = TotalPaginas;

            List<Producto> Productos = new List<Producto>();
            if(totalProductos > 0)
            {
                Productos = await query
                    .OrderBy(p=>p.Nombre)
                    .Skip((pagina-1)  * productosPorPaginas)
                    .Take(productosPorPaginas)
                    .ToListAsync();
            }

            bool MostrarMensajeSinResultados = totalProductos == 0;

            var model = new ProductosPaginadosViewModel{
                Productos = Productos,
                PaginaActual = pagina,
                TotalPaginas=TotalPaginas,
                CategoriaIdSeleccionada = categoriaId,
                Busqueda=busqueda,
                MostrarMensajeSinResultados = MostrarMensajeSinResultados
            };

            return model;
        }





        public async Task<List<Producto>> GetProductosDestacados()
        {
            IQueryable<Producto> productosQuery = _context.Productos;
            productosQuery = productosQuery.Where(p=> p.Activo);

            List<Producto> ProductosDestacados = await productosQuery.Take(9).ToListAsync();
            return ProductosDestacados;
        }
    }
}