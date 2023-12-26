using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using Microsoft.AspNetCore.Mvc;
using ANNIE_SHOP.Services;
using ANNIE_SHOP.Models.ViewModel;

namespace ANNIE_SHOP.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoriaServices _categorias;
        private readonly IProductoServices _productos;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ICategoriaServices categorias, IProductoServices productos) : base(context)
        {
            _logger = logger;
            _categorias = categorias;
            _productos = productos;
        }




        public async Task<IActionResult> Index()
        {
            ViewBag.Categorias = await _categorias.GetCategorias();
            try
            {
                List<Producto> productosDestacados = await _productos.GetProductosDestacados();
                return View(productosDestacados);
            }
            catch (Exception e)
            {

                return HandleError(e);
            }
        }




        public async Task<IActionResult> Productos(int? categoriaId, string? busqueda, int pagina = 1)
        {
            try
            {
                int productosPorPagina = 9;
                ProductosPaginadosViewModel model = await _productos.GetProductoPaginados(
                    categoriaId,
                    busqueda,
                    pagina,
                    productosPorPagina
                );
                ViewBag.Categorias = await _categorias.GetCategorias();
                if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_ProductosPartial", model);
                }
                return  View(model);
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }




        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int id, int cantidad, int? categoriaId, string? busqueda, int pagina = 1)
        {
            var carritoViewModel = await AgregarProductoAlCarrito(id, cantidad);
            if (carritoViewModel != null)
            {
                return RedirectToAction(
                    "Productos",
                    new
                    {
                        id,
                        categoriaId,
                        busqueda,
                        pagina
                    }
                );
            }
            else
                return NotFound();
        }



        public async Task<IActionResult> AgregarProductoIndex(int id, int cantidad)
        {
            var carritoViewModel = await AgregarProductoAlCarrito(id, cantidad);
            if (carritoViewModel != null)
            {
                return RedirectToAction("Index");
            }
            else
                return NotFound();
        }





        public async Task<IActionResult> AgregarProductoDetalle(int id, int cantidad)
        {
            var carritoViewModel = await AgregarProductoAlCarrito(id, cantidad);
            if (carritoViewModel != null)
            {
                return RedirectToAction("DetalleProducto", new { id });
            }
            else
                return NotFound();
        }



        public IActionResult DetalleProducto(int id)
        {
            var producto = _productos.GetProducto(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }





        public IActionResult Privacy()
        {
            return View();
        }

    }
}