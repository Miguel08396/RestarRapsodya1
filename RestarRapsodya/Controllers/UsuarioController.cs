using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RestarRapsodya.Data;
using RestarRapsodya.Models;
using System.Security.Claims;
using RestarRapsodya.Services;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Http.Extensions;

namespace RestarRapsodya.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RestarRapsodyaContext _context;
        private readonly ServiciosRapsodya _servicios;
        private readonly IConverter _converter;

        public UsuarioController(RestarRapsodyaContext context, IEmailService emailService,IConverter converter)
        {
            _context = context;
            _servicios = new ServiciosRapsodya(context, emailService);
            _converter = converter;
        }

        
        public IActionResult DescargarPDF(int id_pedido,string pedidoUrl)
        {

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Landscape
                },
                Objects = {
                    new ObjectSettings()
                    {
                        Page = pedidoUrl
                    }
                }
            };

            var archivoPDF = _converter.Convert(pdf);
            string nombrePDF = "Pedido" + DateTime.Now.ToString("ddMMyyyyHHmm")+".pdf";
            return File(archivoPDF, "application/pdf",nombrePDF);
        }

        public string ObtenerURLActual()
        {
            var urlActual = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            return urlActual;
        }


        // GET: Admin
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Consultar()
        {
            return _context.Usuario != null ?
                        View(await _context.Usuario.Include(u => u.Rol).ToListAsync()) :
                        Problem("Entity set 'RestarRapsodyaContext.Usuario'  is null.");
        }
        public IActionResult Registro(bool agregado = false)
        {
            return View();
        }
        public async Task<IActionResult> Registrarse([Bind("Correo,Nombre,password,id_Rol,Telefono")] Usuario usuario)
        {
            Usuario usuarioExiste = _context.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo);
            if (ModelState.IsValid && usuarioExiste == null)
            {
                string contasenaCif = _servicios.CifrarContraseña(usuario.password);
                usuario.password = contasenaCif;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                _servicios.MandarCorreo(usuario.Correo);
                return RedirectToAction("Iniciar_Sesion_Cliente", new { agregado = true });
            }
            if (usuarioExiste != null)
            {
                return RedirectToAction("Registrarse", new { agregado = true });
            }
            return View(usuario);
        }

        public IActionResult Iniciar_Sesion()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        public IActionResult Iniciar_Sesion_Cliente(bool agregado = false)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Agregado = agregado;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string contrasena)
        {
            Usuario usuarioLog;
            if (correo != null && contrasena != null)
            {
                usuarioLog = _servicios.comprobarContrasenas(correo, contrasena);
                if (usuarioLog != null)
                {
                    ClaimsIdentity identity = new ClaimsIdentity("CookieAuth", ClaimTypes.Name, ClaimTypes.Role);
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usuarioLog.Nombre),
                        new Claim(ClaimTypes.Role, usuarioLog.Rol.nombre_Rol),
                        new Claim("IdUsuario", usuarioLog.Correo),
                    };

                    identity.AddClaim(claims.FirstOrDefault(c => c.Type == ClaimTypes.Name));
                    identity.AddClaim(claims.FirstOrDefault(c => c.Type == ClaimTypes.Role));
                    identity.AddClaim(claims.FirstOrDefault(c => c.Type == "IdUsuario"));

                    ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("CookieAuth", userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(30)
                    });
                    return RedirectToAction("PrincipalAdmin", "usuario");
                }
            }
            usuarioLog = _context.Usuario.Include(u=> u.Rol).FirstOrDefault(u => u.Correo == correo);
            if (correo != null && usuarioLog != null && usuarioLog.id_Rol == 2)
            {
                ClaimsIdentity identity = new ClaimsIdentity("CookieAuth", ClaimTypes.Name, ClaimTypes.Role);
                var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, usuarioLog.Nombre),
                        new Claim(ClaimTypes.Role, usuarioLog.Rol.nombre_Rol),
                        new Claim("IdUsuario", usuarioLog.Correo),
                    };

                identity.AddClaim(claims.FirstOrDefault(c => c.Type == ClaimTypes.Name));
                identity.AddClaim(claims.FirstOrDefault(c => c.Type == ClaimTypes.Role));
                identity.AddClaim(claims.FirstOrDefault(c => c.Type == "IdUsuario"));

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("CookieAuth", userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddMinutes(30)
                });
                return RedirectToAction("PrincipalCliente", "usuario", new { pedido = false });
            }
            return RedirectToAction("Iniciar_Sesion", "usuario");
        }
        public IActionResult CerrarSesion()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("RestarRapsodya.Cookie");
            // Agregar respuesta de no almacenamiento en caché
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate"; // HTTP 1.1
            Response.Headers["Pragma"] = "no-cache"; // HTTP 1.0
            Response.Headers["Expires"] = "0"; // Proxies
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult PrincipalAdmin(bool agregado = false)
        {
            ViewBag.Agregado = agregado;
            return View();
        }
        [Authorize(Roles = "Cliente")]
        public IActionResult PrincipalCliente(bool pedido = false)
        {
            ViewData["Mesas"] = new SelectList(_context.Mesa.Where(m => m.id_Estado == 1), "id_Mesa", "id_Mesa");
            ViewBag.Pedido = pedido;
            return View();
        }

        // GET: Admin/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Detalles(string correo)
        {
            if (correo == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Correo == correo);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Admin/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear(bool agregado = true)
        {
            ViewData["Roles"] = new SelectList(_context.Rol, "id_Rol", "nombre_Rol");
            ViewBag.Agregado = agregado;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([Bind("Correo,Nombre,password,id_Rol,Telefono")] Usuario usuario)
        {
            Usuario usuarioExiste = _context.Usuario.FirstOrDefault(u => u.Correo == usuario.Correo);
            if (ModelState.IsValid && usuarioExiste == null)
            {
                string contasenaCif = _servicios.CifrarContraseña(usuario.password);
                usuario.password = contasenaCif;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                _servicios.MandarCorreo(usuario.Correo);
                return RedirectToAction("PrincipalAdmin", new { agregado = true });
            }
            if (usuarioExiste != null)
            {
                return RedirectToAction("Crear", new { agregado = false });
            }
            ViewData["Roles"] = new SelectList(_context.Rol, "id_Rol", "nombre_Rol");
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(string? correo)
        {
            if (correo == null || _context.Usuario == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuario.FindAsync(correo);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["Roles"] = new SelectList(_context.Rol, "id_Rol", "nombre_Rol");
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(string? correo, [Bind("Correo,Nombre,password,id_Rol,Telefono,")] Usuario usuario)
        {
            if (correo != usuario.Correo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string contasenaCif = _servicios.CifrarContraseña(usuario.password);
                    usuario.password = contasenaCif;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Correo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Consultar");
            }
            ViewData["Roles"] = new SelectList(_context.Rol, "id_Rol", "nombre_Rol");
            return View(usuario);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(string? correo)
        {
            if (correo == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.Include(u => u.Rol).FirstOrDefaultAsync(m => m.Correo == correo);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarConfirmed(string? correo)
        {
            if (_context.Usuario == null)
            {
                return Problem("Entity set 'RestarRapsodyaContext.Usuario'  is null.");
            }
            var usuario = await _context.Usuario.FindAsync(correo);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Consultar));
        }

        private bool UsuarioExists(string correo)
        {
            return (_context.Usuario?.Any(e => e.Correo == correo)).GetValueOrDefault();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HacerPedido([Bind("id_Pedido,fecha_Pedido,Nombre_Plato,Cantidad,Correo_Usuario,id_Mesa")] Pedido pedido)
        {
            var claimsPrincipal = HttpContext.User;
            var correoUsuario = claimsPrincipal.FindFirst("IdUsuario").Value;
            pedido.Correo_Usuario = correoUsuario.ToString();
            pedido.fecha_Pedido = DateTime.Now;
            _context.Add(pedido);
            Mesa mesa = _context.Mesa.FirstOrDefault(m => m.id_Mesa == pedido.id_Mesa);
            mesa.id_Estado = 2;
            await _context.SaveChangesAsync();
            Pedido ultimoPedido = _context.Pedido.OrderByDescending(p => p.fecha_Pedido).FirstOrDefault();
            return RedirectToAction("InfoPedido", new { id_pedido = ultimoPedido.id_Pedido });
        }

        public async Task<IActionResult> InfoPedido(int id_pedido)
        {
            if (id_pedido == null)
            {
                return NotFound();
            }
            var pedido = await _context.Pedido.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.id_Pedido == id_pedido);
            if (pedido == null)
            {
                return NotFound();
            }

            ViewBag.pedidoUrl = ObtenerURLActual();

            return View(pedido);
        }

    }
}
