using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QhapaqÑan.Clases;
using QhapaqÑan.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QhapaqÑan.Controllers
{
    public class HomeController : Controller
    {
        private readonly PContext context;

        public HomeController(PContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Reserva()
        {
            ViewBag.Servicios = context.Servicios.ToList();
            ViewBag.Horarios = context.Horas.ToList();
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Reserva(Reserva reserva, List<int> servicio, List<int> horario)
        {
            reserva.DNI_User = User.Claims.FirstOrDefault().Value;
            List<ReservaServicio> reservaServicios = new List<ReservaServicio>();
            List<ReservaHora> reservaHoras = new List<ReservaHora>();
            if (servicio.Count() == 0)
                ModelState.AddModelError("servicio", "Seleccione por lo menos uno");
            if (horario.Count() == 0)
                ModelState.AddModelError("horario", "Seleccione por lo menos uno");

            if (ModelState.IsValid)
            {
                context.Reservas.Add(reserva);
                context.SaveChanges();
                foreach (var item in servicio)
                {
                    var servicios = new ReservaServicio();
                    servicios.Id_Reserva = reserva.Id;
                    servicios.Id_Servicio = item;
                    reservaServicios.Add(servicios);
                }
                context.ReservaServicios.AddRange(reservaServicios);
                context.SaveChanges();

                foreach (var item in horario)
                {
                    var horarios = new ReservaHora();
                    horarios.Id_Reserva = reserva.Id;
                    horarios.Id_Hora = item;
                    reservaHoras.Add(horarios);
                }
                context.ReservaHoras.AddRange(reservaHoras);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                Response.StatusCode = 400;
                ViewBag.Servicios = context.Servicios.ToList();
                ViewBag.Horarios = context.Horas.ToList();
                return View(reserva);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Reservas()
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha).ToList();
            ViewBag.Servicios = context.ReservaServicios.Where(o => o.Reserva.DNI_User == User.Claims.FirstOrDefault().Value).Include(o => o.Servicios).ToList();
            ViewBag.Horarios = context.ReservaHoras.Where(o => o.Reserva.DNI_User == User.Claims.FirstOrDefault().Value).Include(o => o.Hora).ToList().OrderBy(o => o.Hora.Hora_Inicio);
            if (user.Id_Rol != 1)
            {
                return View(reservas.Where(o => o.DNI_User == User.Claims.FirstOrDefault().Value).ToList());
            }
            else
            {
                ViewBag.User = user;
                return View(reservas);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            var reservas = context.Reservas.Where(o => o.Id == id).FirstOrDefault();
            ViewBag.Servicios = context.Servicios.ToList();
            ViewBag.Horarios = context.Horas.ToList();
            if (user.Id_Rol != 1)
            {
                return View(reservas);
            }
            else
            {
                ViewBag.User = user;
                return View(reservas);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Editar(Reserva reserva, List<int> servicio, List<int> horario)
        {
            var res = context.Reservas.Where(o => o.Id == reserva.Id).FirstOrDefault();
            res.Fecha = reserva.Fecha;
            List<ReservaServicio> reservaServicios = new List<ReservaServicio>();
            List<ReservaHora> reservaHoras = new List<ReservaHora>();
            if (servicio.Count() == 0)
                ModelState.AddModelError("servicio", "Seleccione por lo menos uno");
            if (horario.Count() == 0)
                ModelState.AddModelError("horario", "Seleccione por lo menos uno");

            if (ModelState.IsValid)
            {
                var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == res.Id).ToList();
                var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == res.Id).ToList();
                context.ReservaHoras.RemoveRange(resHor);
                context.ReservaServicios.RemoveRange(resSer);
                context.Reservas.Update(res);
                context.SaveChanges();
                foreach (var item in servicio)
                {
                    var servicios = new ReservaServicio();
                    servicios.Id_Reserva = res.Id;
                    servicios.Id_Servicio = item;
                    reservaServicios.Add(servicios);
                }
                context.ReservaServicios.AddRange(reservaServicios);
                context.SaveChanges();

                foreach (var item in horario)
                {
                    var horarios = new ReservaHora();
                    horarios.Id_Reserva = res.Id;
                    horarios.Id_Hora = item;
                    reservaHoras.Add(horarios);
                }
                context.ReservaHoras.AddRange(reservaHoras);
                context.SaveChanges();
                return RedirectToAction("Perfil","User", res);
            }
            else
            {
                Response.StatusCode = 400;
                ViewBag.Servicios = context.Servicios.ToList();
                ViewBag.Horarios = context.Horas.ToList();
                return View(res);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var reserva = context.Reservas.Where(o => o.Id == id).FirstOrDefault();
            var horas = context.ReservaHoras.Where(o => o.Id_Reserva == id).ToList();
            var servicios = context.ReservaServicios.Where(o => o.Id_Reserva == id).ToList();
            context.Reservas.Remove(reserva);
            context.ReservaHoras.RemoveRange(horas);
            context.ReservaServicios.RemoveRange(servicios);
            context.SaveChanges();
            return View("Reservas");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
