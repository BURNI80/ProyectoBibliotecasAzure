using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NuguetProyectoBibliotecas.Models;
using ProyectoBibliotecas.Extensions;
using ProyectoBibliotecas.Services;
using System.Security.Claims;

namespace ProyectoBibliotecas.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceApiBibliotecas service;

        public ManagedController(ServiceApiBibliotecas service)
        {
            this.service = service;
        }


        public IActionResult Login()
        {
            return View();
        }

        //REGISTRO
        [HttpPost]
        public async Task<IActionResult> Login(string nombre, string apellidos, string dni, string usuario, string password, string email, int telefono)
        {
            if (usuario == null)
            {
                //LOGIN
                string token = await this.service.GetTokenAsync(dni, password);
                HttpContext.Session.SetString("token", token);
                Usuario user = await this.service.GetUsu(dni, password);
                if (user == null)
                {
                    ViewData["MSG"] = "Incorrecto";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetObject("user", user);
                    ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim("ROL", user.ROL));
                    Claim claimUserName = new Claim(ClaimTypes.Name, user.DNI_USUARIO);
                    Claim claimRole = new Claim(ClaimTypes.Role, user.ROL);
                    identity.AddClaim(claimUserName);
                    identity.AddClaim(claimRole);
                    ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(15)
                    });
                    return RedirectToAction("IndexBibliotecas", "Bibliotecas");
                }
            }
            else
            {
                await this.service.Register(nombre, apellidos, dni, usuario, password, email, telefono);
            }
            return View();

        }

        public async Task<IActionResult> CerrarSesion()
        {
            HttpContext.Session.Remove("user");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //string urlAnterior = HttpContext.Request.Headers["Referer"];
            //return Redirect(urlAnterior);
            return RedirectToAction("Login", "Managed");
        }

        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult NoAccess()
        {
            return View();
        }


    }
}
