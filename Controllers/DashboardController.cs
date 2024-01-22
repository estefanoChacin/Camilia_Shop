using ANNIE_SHOP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ANNIE_SHOP.Controllers
{
    [Authorize(Policy = "RequireAdminOrStaff")]

    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context) : base(context) { }

        public IActionResult Index()
        {
            return View();
        }


    }
}