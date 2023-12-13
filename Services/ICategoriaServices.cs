using ANNIE_SHOP.Models;

namespace ANNIE_SHOP.Services
{
    public interface ICategoriaServices
    {
        Task<List<Categoria>>GetCategorias();
    }
}