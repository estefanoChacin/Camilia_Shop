

namespace ANNIE_SHOP.Services
{
    public interface IBannerService
    {
        Task<string> SubirImgenStorage(Stream archivo, string nombre);
        
    }
}