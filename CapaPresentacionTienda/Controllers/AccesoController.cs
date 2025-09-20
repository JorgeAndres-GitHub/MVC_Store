using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapaPresentacionTienda.Models;

namespace CapaPresentacionTienda.Controllers
{
    public class AccesoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Reestablecer()
        {
            return View();
        }

        public IActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;

            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "": objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "": objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "": objeto.Correo;

            if (objeto.Clave != objeto.ConfirmarClave)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            resultado = new CN_Cliente().Registrar(objeto, out mensaje);

            if(resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo, string clave)
        {
            Cliente oCliente = null;
            oCliente = new CN_Cliente().Listar().Where(c => c.Correo == correo && c.Clave == CN_Recursos.ConvertirSHA256(clave)).FirstOrDefault();
            if (oCliente == null)
            {
                ViewBag.Error = "Correo o contraseña no son correctas";
                return View();
            }
            else
            {
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarClave");
                }
                else
                {
                    // 🟢 Crear los claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, oCliente.Correo),
                    };


                    HttpContext.Session.SetObjectAsJson("Cliente", oCliente);

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    //HttpContext.Session.SetObjectAsJson("Cliente", oCliente);

                    // 🟢 Autenticar al usuario
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Tienda");
                }
            }
        }

        [HttpPost]
        public IActionResult Reestablecer(string correo)
        {
            Cliente oCliente = new Cliente();
            oCliente = new CN_Cliente().Listar().Where(item => item.Correo == correo).FirstOrDefault();
            if (oCliente == null)
            {
                ViewBag.Error = "No se encontro un cliente relacionado a ese correo";
                return View();
            }

            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().ReestablecerClave(oCliente.IdCliente, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        [HttpPost]
        public IActionResult CambiarClave(string idCliente, string claveActual, string nuevaClave, string confirmarClave)
        {
            Cliente oCliente = new Cliente();
            oCliente = new CN_Cliente().Listar().Where(u => u.IdCliente == int.Parse(idCliente)).FirstOrDefault();

            if (oCliente.Clave != CN_Recursos.ConvertirSHA256(claveActual))
            {
                TempData["IdCliente"] = idCliente;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual no es correcta";
                return View();
            }
            else if (nuevaClave != confirmarClave)
            {
                TempData["IdCliente"] = idCliente;
                ViewData["vclave"] = claveActual;
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaClave = CN_Recursos.ConvertirSHA256(nuevaClave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idCliente), nuevaClave, out mensaje);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = idCliente;
                ViewBag.Error = mensaje;
                return View();
            }
        }

        public async Task<ActionResult> CerrarSesion()
        {
            HttpContext.Session.Remove("Cliente");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Acceso");
        }
    }
}
