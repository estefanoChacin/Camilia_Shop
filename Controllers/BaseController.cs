using Microsoft.AspNetCore.Mvc;
using ANNIE_SHOP.Data;
using Newtonsoft.Json;
using ANNIE_SHOP.Models;

namespace ANNIE_SHOP.Controllers
{
    public class BaseController : Controller
    {
        public readonly ApplicationDbContext _context;

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
            if(!string.IsNullOrEmpty(carritoJson))
            {
                var carrito = JsonConvert.DeserializeObject<List<ProductoandCantidad>>(carritoJson);
                if (carrito != null)
                {
                    count = carrito.Count;
                }
            }
            return count;
        }
    }
}