using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ANNIE_SHOP.Services
{
    public interface IUsuarioServices
    {
        string EncriptedPassword(string password);
    }
}