
namespace ANNIE_SHOP.Models.ViewModel
{
    public class CarritoViewModel
    {
        public List<CarritoItemViewModel> Items { get; set; } = new List<CarritoItemViewModel>();
        public decimal Total { get; set; }
    }
}