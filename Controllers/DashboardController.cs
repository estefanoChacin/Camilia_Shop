using ANNIE_SHOP.Data;
using Microsoft.AspNetCore.Mvc;


namespace ANNIE_SHOP.Controllers
{

    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context) : base(context) { }

        public IActionResult Index()
        {
            return View();
        }


    }
}