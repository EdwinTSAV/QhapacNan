using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QhapaqÑan.Clases;
using QhapaqÑan.Models;
using QhapaqÑan.Models.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
        private readonly string email = "xavedraandy@gmail.com";
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
            if (context.Roles.ToList().Count == 0)
                CrearRoles();
            if (context.Servicios.ToList().Count == 0)
                CrearServicios();
            if (context.Horas.ToList().Count == 0)
                CrearHoras();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string dni, string contrasenia)
        {
            var user = context.Usuarios.Where(o => o.DNI == dni && o.Contrasenia == CreateHash(contrasenia)).FirstOrDefault();

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

        [Authorize]
        [HttpGet]
        public IActionResult Perfil()
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Editar(Usuario usuario)
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            var users = context.Usuarios.Where(o => (o.Celular == usuario.Celular || o.Correo == usuario.Correo) && o.DNI != usuario.DNI).ToList(); 
            foreach (var item in users)
            {
                if (item.Correo == usuario.Correo)
                    ModelState.AddModelError("Correo", "Este correo ya se encuentra registrado en otro usuario");
                if (item.Celular == usuario.Celular)
                    ModelState.AddModelError("Celular", "Este Celular ya se encuentra resgitrado en otro usuario");
            }

            if (ModelState.IsValid)
            {
                user.Correo = usuario.Correo;
                user.Celular = usuario.Celular;
                context.Usuarios.Update(user);
                context.SaveChanges();
                return View("Perfil", user);
            }
            else
                return View("Perfil", user);
            
        }


        [Authorize]
        [HttpGet]
        public IActionResult Configuracion()
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Configuracion(string ContraseniaAntigua, string ContraseniaNueva, string ContraseniaConfirma)
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            if (user.Contrasenia != ContraseniaAntigua)
                ModelState.AddModelError("contrasenia", "Contraseña invalida");
            if (ContraseniaConfirma != ContraseniaNueva)
                ModelState.AddModelError("contraseniaX", "Las contraseñas no coinciden");

            if (ModelState.IsValid)
            {
                user.Contrasenia = CreateHash(ContraseniaNueva);
                context.Usuarios.Update(user);
                context.SaveChanges();
                
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, user.DNI)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return View("Configuracion", user);
            }
            else
                return View("Configuracion", user);

        }

        [HttpGet]
        public IActionResult Recover()
        {
            ViewBag.Codigo = false;
            return View(new Recovery());
        }

        [HttpPost]
        public IActionResult Recover(Recovery recovery)
        {
            try
            {
                ViewBag.Codigo = false;
                var users = context.Usuarios.Where(o => o.Correo == recovery.Email).FirstOrDefault();
                if (users == null)
                {
                    ModelState.AddModelError("Email", "El usuario del correo no esta resgistrado");
                }
                if (ModelState.IsValid)
                {
                    Random random = new Random();
                    int num = random.Next(1000, 9999);
                    var numero = CreateHash(num.ToString());

                    users.Recovery = numero;
                    context.Usuarios.Update(users);
                    context.SaveChanges();

                    EmailSend(recovery.Email, numero);
                    ViewBag.Codigo = true;
                    return View(recovery);
                }
                else
                    return View(recovery);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult StartRecover(string token)
        {
            RecoverP recoverp = new RecoverP
            {
                Token = token
            };

            var users = context.Usuarios.Where(o => o.Recovery == token).FirstOrDefault();
            if (users == null || recoverp.Token == null || recoverp.Token.Trim().Equals(""))
            {
                return RedirectToAction("Login");
            }
            return View(recoverp);
        }

        [HttpPost]
        public IActionResult StartRecover(RecoverP recoverp)
        {
            try
            {
                string token = recoverp.Token;
                var users = context.Usuarios.Where(o => o.Recovery == token).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    users.Contrasenia = CreateHash(recoverp.Password);
                    users.Recovery = null;
                    context.Usuarios.Update(users);
                    context.SaveChanges();
                }
                else
                    return View(recoverp);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return RedirectToAction("Login");
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
        public void CrearHoras()
        {
            var horario = new List<Hora>();
            var horarioA = new Hora() { Id = 1, Hora_Inicio = 8, Hora_Fin = 9 };
            var horarioB = new Hora() { Id = 2, Hora_Inicio = 9, Hora_Fin = 10 };
            var horarioC = new Hora() { Id = 3, Hora_Inicio = 10, Hora_Fin = 11 };
            var horarioD = new Hora() { Id = 4, Hora_Inicio = 11, Hora_Fin = 12 };
            var horarioE = new Hora() { Id = 5, Hora_Inicio = 12, Hora_Fin = 13 };
            var horarioF = new Hora() { Id = 6, Hora_Inicio = 13, Hora_Fin = 14 };
            var horarioG = new Hora() { Id = 7, Hora_Inicio = 14, Hora_Fin = 15 };
            var horarioH = new Hora() { Id = 8, Hora_Inicio = 15, Hora_Fin = 16 };
            var horarioI = new Hora() { Id = 9, Hora_Inicio = 16, Hora_Fin = 17 };
            var horarioJ = new Hora() { Id = 10, Hora_Inicio = 17, Hora_Fin = 18 };
            horario.Add(horarioA);
            horario.Add(horarioB);
            horario.Add(horarioC);
            horario.Add(horarioD);
            horario.Add(horarioE);
            horario.Add(horarioF);
            horario.Add(horarioG);
            horario.Add(horarioH);
            horario.Add(horarioI);
            horario.Add(horarioJ);
            context.AddRange(horario);
            context.SaveChanges();
        }
        protected void EmailSend(string correoUser, string token)
        {
            string url = urlDomain + "user/startrecover/?token=" + token;
            MailMessage mailMessage = new MailMessage(email, correoUser,
                "Recuperación de la cuenta QhapacÑan",
                "<p>Correo de recuperacion de contraseña</p><br>"
                + "<a href='" + url + "'><strong>Click para recuperar</strong></a>")
            {
                IsBodyHtml = true
            };
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new System.Net.NetworkCredential(email, password)
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }
    }
}
