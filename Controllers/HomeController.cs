using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ANNIE_SHOP.Services;

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
                var model = await _productos.GetProductoPaginados(categoriaId, busqueda, pagina, productosPorPagina);
                ViewBag.Categorias = await _categorias.GetCategorias();

                if(Request.Headers["X-Request-With"] == "XMLHttpRequets")
                    return PartialView("_ProductosPartial", model);

                return View(model);
            }
            catch (Exception e)
            {
                
                return HandleError(e);
            }
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





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
