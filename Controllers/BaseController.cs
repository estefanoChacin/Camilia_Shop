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
            if (!string.IsNullOrEmpty(carritoJson))
            {
                var carrito = JsonConvert.DeserializeObject<List<ProductoandCantidad>>(carritoJson);
                if (carrito != null)
                {
                    count = carrito.Count;
                }
            }
            return count;
        }




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