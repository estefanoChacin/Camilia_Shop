using ANNIE_SHOP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ANNIE_SHOP.Controllers
{
    [Authorize(Roles = "Administrador, Staff")]

    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context) : base(context) { }

        public IActionResult Index()
        {
            return View();
        }


    }
}