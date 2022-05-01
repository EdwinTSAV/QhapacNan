using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QhapaqÑan.Clases;
using QhapaqÑan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QhapaqÑan.Controllers
{
    public class UserController : Controller
    {
        private readonly PContext context;
        private readonly IWebHostEnvironment hosting;
        private readonly IConfiguration configuration;
        private readonly string email = "juliaRAF2020@gmail.com";
        private readonly string password = "juliaR4F2O2O";
        private readonly string urlDomain = "https://localhost:44313/";

        public UserController(PContext context, IWebHostEnvironment hosting, IConfiguration configuration)
        {
            this.context = context;
            this.hosting = hosting;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string dni, string contrasenia)
        {
            var user = context.Usuarios.Where(o => o.DNI == dni && o.Contrasenia == contrasenia).FirstOrDefault();

            if (user != null)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, dni)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                ModelState.AddModelError("Login", "Usuario o contraseña incorrectos");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return View("Login");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult Register(Usuario usuario, string passwordC)
        {
            if (usuario.Contrasenia != passwordC)
                ModelState.AddModelError("PasswordConf", "Las contraseñas no coinciden");

            var usuarios = context.Usuarios.Where(o => o.DNI == usuario.DNI || o.Correo == usuario.Correo).ToList(); ;
            foreach (var item in usuarios)
            {
                if (item.Correo == usuario.Correo)
                    ModelState.AddModelError("Correo", "Este correo ya se encuentra registrado");
                if (item.DNI == usuario.DNI)
                    ModelState.AddModelError("Username", "Este DNI ya se encuentra resgitrado");
            }

            if (ModelState.IsValid)
            {
                usuario.Contrasenia = CreateHash(usuario.Contrasenia);
                if (usuario.Correo == email)
                {
                    if (context.Roles.ToList().Count == 0) { }
                        CrearRoles();
                    if (context.Roles.ToList().Count == 0)
                        CrearServicios();
                    usuario.Id_Rol = 1;
                }
                else
                    usuario.Id_Rol = 3;
                context.Usuarios.Add(usuario);
                context.SaveChanges();
                return RedirectToAction("Login");
            }
            else
                return View(usuario);
        }

        protected string CreateHash(string input)
        {
            input += configuration.GetValue<string>("Token");
            var sha = SHA512.Create();
            var bytes = Encoding.Default.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public void CrearRoles()
        {
            var roles = new List<Roles>();
            var rolA = new Roles() { Id = 1, Nombre = "Admin" };
            var rolU = new Roles() { Id = 2, Nombre = "UserC" };
            roles.Add(rolA);
            roles.Add(rolU);
            context.AddRange(roles);
            context.SaveChanges();
        }

        public void CrearServicios()
        {
            var servicios = new List<Servicios>();
            var serviciosA = new Servicios() { Id = 1, Nombre = "Futbol 7", Precio = 10.00m };
            var serviciosB = new Servicios() { Id = 2, Nombre = "Voley", Precio = 10.00m };
            var serviciosC = new Servicios() { Id = 3, Nombre = "Fulbito", Precio = 10.00m };
            var serviciosD = new Servicios() { Id = 4, Nombre = "Basquet", Precio = 10.00m };
            var serviciosE = new Servicios() { Id = 5, Nombre = "Tenis", Precio = 10.00m };
            var serviciosF = new Servicios() { Id = 6, Nombre = "Fronton", Precio = 10.00m };
            servicios.Add(serviciosA);
            servicios.Add(serviciosB);
            servicios.Add(serviciosC);
            servicios.Add(serviciosD);
            servicios.Add(serviciosE);
            servicios.Add(serviciosF);
            context.AddRange(servicios);
            context.SaveChanges();
        }
    }
}
