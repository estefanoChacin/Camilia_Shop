
using ANNIE_SHOP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ANNIE_SHOP.Data;
using ANNIE_SHOP.Models;
using Microsoft.AspNetCore.Authorization;

namespace ANNIE_SHOP.Controllers
{
    [Authorize(Roles = "Administrador, Staff")]
    public class UsuariosController : BaseController
    {

        private IUsuarioServices _usuriosServices;
        public UsuariosController(ApplicationDbContext context, IUsuarioServices usuarioServices) : base(context)
        {
            _usuriosServices = usuarioServices;
        }




        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Usuarios.Include(u => u.Rol);
            return View(await applicationDbContext.ToListAsync());
        }





        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }





        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre");
            return View();
        }





        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Usuario usuario)
        {

            var rol = await _context.Roles
            .Where(d => d.RolId == usuario.RolId)
            .FirstOrDefaultAsync();

            if (rol != null)
            {
                usuario.Rol = rol;

                usuario.Direcciones = new List<Direccion>
                {
                    new Direccion
                    {
                        Address=usuario.Direccion,
                        Ciudad=usuario.Ciudad,
                        Departamento=usuario.Departamento,
                        CodigoPostal=usuario.CodigoPostal
                    }
                };

                usuario.Contrasenia = _usuriosServices.EncriptedPassword(usuario.Contrasenia);

                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }





        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }




        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            var rol = await _context.Roles
            .Where(d => d.RolId == usuario.RolId)
            .FirstOrDefaultAsync();

            if (rol != null)
            {
                usuario.Rol = rol;

                var ExistingUser = await _context.Usuarios
                .Include(d => d.Direcciones)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (ExistingUser != null)
                {
                    if (ExistingUser.Direcciones.Count > 0)
                    {
                        new Direccion
                        {
                            Address = usuario.Direccion,
                            Ciudad = usuario.Ciudad,
                            Departamento = usuario.Departamento,
                            CodigoPostal = usuario.CodigoPostal
                        };
                    }

                    if(usuario.Contrasenia != ExistingUser.Contrasenia)
                        usuario.Contrasenia = _usuriosServices.EncriptedPassword(usuario.Contrasenia);

                    try
                    {
                        _context.Entry(ExistingUser).State = EntityState.Detached;
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsuarioExists(usuario.UsuarioId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }




        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }




        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }




        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
