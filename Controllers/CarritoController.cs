using System.Security.Claims;
using ANNIE_SHOP.Data;
using Microsoft.AspNetCore.Mvc;
using ANNIE_SHOP.Models;
using ANNIE_SHOP.Models.ViewModel;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Core;
using PayPalHttp;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
namespace ANNIE_SHOP.Controllers
{
    public class CarritoController : BaseController
    {
        private readonly string clientId = "Ae3KbekakPz-FnS_OQRBjqAoRt9PznJLIgBBTiLLL6KlT5kLHBe8OY9nJWf7tSTbdwCNexZroYCkWXa3";
        private readonly string clientSecret = "ED-3QHzy2im3eSslnpgs8ZqdrIM7k0kh-B3E3NIYd2AwETvisCpKcB7wGufB6uEMLCUR2zsAgOXpzjel";


        public CarritoController(ApplicationDbContext context) : base(context)
        {
        }




        public async Task<IActionResult> Index()
        {
            var carritoViewModel = await GetCarritoViewModelAsync();

            var itemsEliminar = new List<CarritoItemViewModel>();
            foreach (var item in carritoViewModel.Items)
            {
                var producto = await _context.Productos.FindAsync(item.ProdcutoId);
                if (producto != null)
                {
                    item.Producto = producto;

                    if (!producto.Activo)
                        itemsEliminar.Add(item);
                    else
                        item.Cantidad = Math.Min(item.Cantidad, producto.Stock);

                    if (item.Cantidad == 0)
                        carritoViewModel.Items.Remove(item);
                }
                else
                    itemsEliminar.Add(item);
            }

            foreach (var item in itemsEliminar)
            {
                carritoViewModel.Items.Remove(item);
            }
            await ActualizarCarritoViewModelAsync(carritoViewModel);

            carritoViewModel.Total = carritoViewModel.Items.Sum(item => item.Subtotal);


            var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;
            var direcciones = User.Identity?.IsAuthenticated == true ? _context.Direccion.Where(d => d.UsuarioId == usuarioId).ToList() : new List<Direccion>();

            var procederConComparaViewModel = new ProcederConCompraViewModel
            {
                Carrito = carritoViewModel,
                Direcciones = direcciones
            };
            return View(procederConComparaViewModel);
        }




        [HttpPost]
        public async Task<IActionResult> ActualizarCantidad(int id, int cantidad)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(i => i.ProdcutoId == id);

            if (carritoItem != null)
            {
                carritoItem.Cantidad = cantidad;
                var producto = await _context.Productos.FindAsync(id);
                if (producto != null && producto.Activo && producto.Stock > 0)
                    carritoItem.Cantidad = Math.Min(cantidad, producto.Stock);

                await ActualizarCarritoViewModelAsync(carritoViewModel);
            }
            return RedirectToAction("Index", "Carrito");
        }




        [HttpPost]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var carritoViewModel = await GetCarritoViewModelAsync();
            var carritoItem = carritoViewModel.Items.FirstOrDefault(i => i.ProdcutoId == id);

            if (carritoItem != null)
            {
                carritoViewModel.Items.Remove(carritoItem);

                await ActualizarCarritoViewModelAsync(carritoViewModel);
            }
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> VaciarCarrito()
        {
            await RemoveCarritoViewModelAsync();
            return RedirectToAction("Index");
        }


        private async Task RemoveCarritoViewModelAsync()
        {
            await Task.Run(() => Response.Cookies.Delete("carrito"));
        }




        public IActionResult ProcederConCompra(decimal montoTotal, int direccionIdSeleccionada)
        {
            if (direccionIdSeleccionada > 0)
            {
                Response.Cookies.Append(
                    "direccionSeleccionada",
                    direccionIdSeleccionada.ToString(),
                    new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) }
                );
            }
            else
                return View("Index");

            var requets = new OrdersCreateRequest();
            requets.Prefer("return=representation");
            requets.RequestBody(BuilderRequetsBody(montoTotal));

            var environment = new SandboxEnvironment(clientId, clientSecret);
            var client = new PayPalHttpClient(environment);

            try
            {
                var response = client.Execute(requets).Result;
                var statusCode = response.StatusCode;
                var responseBody = response.Result<Order>();

                var approveLink = responseBody.Links.FirstOrDefault(x => x.Rel == "approve");
                if (approveLink != null)
                    return Redirect(approveLink.Href);
                else
                    return RedirectToAction("Error");
            }
            catch (HttpException e)
            {
                return (IActionResult)e;
            }
        }




        private OrderRequest BuilderRequetsBody(decimal montoTotal)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var requets = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode="USD",
                            Value=montoTotal.ToString("F2")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = $"{baseUrl}/carrito/PagoCompletado",
                    CancelUrl = $"{baseUrl}/Carrito/Index"
                }
            };
            return requets;
        }




        public IActionResult PagoCompletado()
        {
            try
            {
                var carritoJson = Request.Cookies["carrito"];
                int direccionId = 0;

                if (Request.Cookies.TryGetValue("direccionSeleccionada", out string? cookieValue) && int.TryParse(cookieValue, out int parseValue))
                {
                    direccionId = parseValue;
                }
                List<ProductoIdandCantidad>? productoAndCantidades = !string.IsNullOrEmpty(carritoJson) ? JsonConvert.DeserializeObject<List<ProductoIdandCantidad>>(carritoJson) : null;
                CarritoViewModel carritoViewModel = new();

                if (productoAndCantidades != null)
                {
                    foreach (var item in productoAndCantidades)
                    {
                        var producto = _context.Productos.Find(item.ProductoId);
                        if (producto != null)
                        {
                            carritoViewModel.Items.Add(
                                new CarritoItemViewModel
                                {
                                    ProdcutoId = producto.ProductoId,
                                    Nombre = producto.Nombre,
                                    Precio = producto.Precio,
                                    Cantidad = item.Cantidad
                                }
                            );
                        }
                    }
                }
                var usuarioId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : 0;

                carritoViewModel.Total = carritoViewModel.Items.Sum(i => i.Subtotal);
                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    Fecha = DateTime.UtcNow,
                    Estado = "Vendido",
                    DireccionIdSeleccionada = direccionId,
                    Total = carritoViewModel.Total
                };

                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                foreach (var item in carritoViewModel.Items)
                {
                    var pedidoDetalle = new Detalle_Pedido
                    {
                        PedidoId = pedido.PedidoId,
                        ProductoId = item.ProdcutoId,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio
                    };

                    _context.DetallesPedidos.Add(pedidoDetalle);

                    var producto = _context.Productos.FirstOrDefault(p => p.ProductoId == item.ProdcutoId);

                    if (producto != null)
                        producto.Stock -= item.Cantidad;
                }
                _context.SaveChanges();

                Response.Cookies.Delete("carrito");
                Response.Cookies.Delete("direccionSeleccionada");

                ViewBag.DetallePedidos = _context.DetallesPedidos
                .Where(dp => dp.PedidoId == pedido.PedidoId)
                .Include(dp => dp.Producto)
                .ToList();

                return View("PagoCompletado", pedido);

            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }
    }
}