using Microsoft.AspNetCore.Mvc;
using ANNIE_SHOP.Data;
using Newtonsoft.Json;
using ANNIE_SHOP.Models;
using System.Diagnostics;
using System.Data.Common;
using ANNIE_SHOP.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ANNIE_SHOP.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }




        public override ViewResult View(string? ViewName, Object? model)
        {
            @ViewBag.NumeroProductos = GetCarritoCount();
            return base.View(ViewName, model);
        }




        protected int GetCarritoCount()
        {
            int count = 0;
            string? carritoJson = Request.Cookies["carrito"];
            if (!string.IsNullOrEmpty(carritoJson))
            {
                var carrito = JsonConvert.DeserializeObject<List<ProductoIdandCantidad>>(carritoJson);
                if (carrito != null)
                {
                    count = carrito.Count;
                }
            }
            return count;
        }




        public async Task<CarritoViewModel> AgregarProductoAlCarrito(int productoId, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(productoId);

            if(producto != null)
            {
                var carritoViewModel = await GetCarritoViewModelAsync();
                var carritoItem = carritoViewModel.Items.FirstOrDefault(item => item.ProdcutoId == productoId);

                if(carritoItem != null)
                    carritoItem.Cantidad += cantidad;
                else
                {
                    carritoViewModel.Items.Add(
                        new CarritoItemViewModel
                        {
                            ProdcutoId = producto.ProductoId,
                            Nombre = producto.Nombre,
                            Precio = producto.Precio,
                            Cantidad = cantidad
                        }
                    );
                }
                 carritoViewModel.Total = carritoViewModel.Items.Sum(item => item.Cantidad * item.Precio);

                await ActualizarCarritoViewModelAsync(carritoViewModel);
                return carritoViewModel;
            }
            return new CarritoViewModel();
        }





        public async Task ActualizarCarritoViewModelAsync(CarritoViewModel carritoViewModel)
        {
            var productosIds = carritoViewModel.Items.Select(
                item => 
                new ProductoIdandCantidad 
                {
                    ProductoId = item.ProdcutoId,
                    Cantidad = item.Cantidad
                }
            ).ToList();

            var carritoJson = await Task.Run(()=> JsonConvert.SerializeObject(productosIds));
            Response.Cookies.Append
            (
                "carrito",
                carritoJson,
                new CookieOptions{Expires = DateTimeOffset.Now.AddDays(7)}
            );
        }



                                                                

        public async Task<CarritoViewModel> GetCarritoViewModelAsync()
        {
            var carritoJson = Request.Cookies["carrito"];

            if(string.IsNullOrEmpty(carritoJson))
                return new CarritoViewModel();
            
            var productosIdsAndCantidades = JsonConvert.DeserializeObject<List<ProductoIdandCantidad>>(carritoJson);
            var carritoViewModel = new CarritoViewModel();

            if(productosIdsAndCantidades != null)
            {
                foreach (var item in productosIdsAndCantidades)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);
                    if(producto != null)
                    {
                        carritoViewModel.Items.Add(
                            new CarritoItemViewModel
                            {
                                ProdcutoId = producto.ProductoId,
                                Nombre = producto.Nombre,
                                Precio = producto.Precio,
                                Cantidad= item.Cantidad
                            }
                        );
                    }
                }
            }
            carritoViewModel.Total = carritoViewModel.Items.Sum(e=> e.Subtotal);
            return carritoViewModel;
        }




        //METODOS PARA MANEJAR LAS EXCEPCIONES
        protected IActionResult HandleError(Exception e)
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        protected IActionResult HandleDbError(DbException dbException)
        {
            var viewModel = new DbErrorViewModel
            {
                ErrorMessage = "Error de base de datos.",
                Details = dbException.Message
            };
            return View("DbError", viewModel);
        }




        protected IActionResult HandleDbUpdateError(DbUpdateException dbUpdateError)
        {
            var viewModel = new DbErrorViewModel
            {
                ErrorMessage = "Error de actualizacion de base de datos.",
                Details = dbUpdateError.Message
            };
            return View("DbError", viewModel);
        }
    }
}