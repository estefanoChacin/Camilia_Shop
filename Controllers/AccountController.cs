using System.Data.Common;
using System.Security.Claims;
using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ANNIE_SHOP.Controllers
{

    public class AccountController : BaseController
    {

        public AccountController(ApplicationDbContext context) : base(context)
        { }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Usuario user)
        {
            try
            {
                if (user != null)
                {
                    if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == user.NombreUsuario))
                    {
                        ModelState.AddModelError(nameof(user.NombreUsuario), "El nombre de usuario ya esta en uso");
                        return View(user);
                    }
                    if (await _context.Usuarios.AnyAsync(u => u.Correo == user.Correo))
                    {
                        ModelState.AddModelError(nameof(user.Correo), "El correo ya esta en uso");
                        return View(user);
                    }

                    var clienteRol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente");
                    if (clienteRol != null)
                    {
                        user.RolId = clienteRol.RolId;
                    }

                    user.Direcciones = new List<Direccion>
                {
                    new Direccion
                    {
                        Address=user.Direccion,
                        Ciudad=user.Ciudad,
                        Departamento=user.Departamento,
                        CodigoPostal=user.CodigoPostal
                    }
                };

                    await _context.AddAsync(user);
                    await _context.SaveChangesAsync();

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.NombreUsuario));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "Cliente"));

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity)
                    );
                    return RedirectToAction("Index", "Home");
                }
                return View(user);


            }
            catch (DbException e)
            {
                return HandleError(e);
            }
        }




        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Administrador") || User.IsInRole("Staff"))
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }




        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email && u.Contrasenia == password);

                if (user != null)
                {
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.NombreUsuario));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()));

                    var rol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == user.RolId);
                    if (rol != null)
                        identity.AddClaim(new Claim(ClaimTypes.Role, rol.Nombre));

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity)
                    );

                    if (User.IsInRole("Administrador") || User.IsInRole("Staff"))
                        return RedirectToAction("Index", "Dashboard");
                    else
                        return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Credenciales invalidas. Por favor intentelo nuevamente.");
                return View();
            }
            catch (Exception e)
            {
                return HandleError(e);
            }
        }




        public async Task<IActionResult> Logout(){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }




        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}