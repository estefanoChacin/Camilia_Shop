
using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using Microsoft.EntityFrameworkCore;

namespace ANNIE_SHOP.Services
{
    public class CategoriaServices : ICategoriaServices
    {
        private readonly ApplicationDbContext _context;

        public CategoriaServices(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<List<Categoria>> GetCategorias()
        {
            return await _context.Categorias.ToListAsync();
        }
    }
}