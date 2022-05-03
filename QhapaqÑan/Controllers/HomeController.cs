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

            var horaActual = DateTime.Now.Date.ToString();
            var reservas = context.Reservas.Where(o => o.Fecha < DateTime.Parse(horaActual) && o.Estado == false).ToList();

            foreach (var item in reservas)
            {
                
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Reserva(Reserva reserva, List<int> servicio, List<int> horario)
        {
            reserva.Precio = 0;
            var fechaActual = DateTime.Now.Date.ToString();
            var horaActual = DateTime.Now;
            reserva.DNI_User = User.Claims.FirstOrDefault().Value;
            List<ReservaServicio> reservaServicios = new List<ReservaServicio>();
            List<ReservaHora> reservaHoras = new List<ReservaHora>();
            if (servicio.Count() == 0)
                ModelState.AddModelError("servicio", "Seleccione por lo menos uno");
            if (horario.Count() == 0)
                ModelState.AddModelError("horario", "Seleccione por lo menos uno");
            if (reserva.Fecha == DateTime.Parse(fechaActual))
            {
                foreach (var item in horario)
                {
                    if (item + 7 < Int32.Parse(horaActual.ToString("HH")))
                    {
                        ModelState.AddModelError("Hora", "La hora seleccionada ah caducado");
                    }
                }
            }
            if (reserva.Fecha < DateTime.Parse(fechaActual))
            {
                ModelState.AddModelError("Fecha", "La fecha ha caducado");
            }

            var serviciosList = context.Servicios.ToList();

            foreach (var item in servicio)
            {
                foreach (var item2 in serviciosList)
                {
                    if (item == item2.Id)
                    {
                        reserva.Precio += (item2.Precio * horario.Count);
                    }
                }
            }

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
            ViewBag.Servicios = context.ReservaServicios.Include(o => o.Servicios).ToList();
            ViewBag.Horarios = context.ReservaHoras.Include(o => o.Hora).ToList().OrderBy(o => o.Hora.Hora_Inicio);
            ViewBag.User = user;
            if (user.Id_Rol != 1)
                return View(reservas.Where(o => o.DNI_User == User.Claims.FirstOrDefault().Value).ToList());
            else
                return View(reservas);
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
            var FechaActual = DateTime.Now.Date.ToString();
            var horaActual = DateTime.Now;
            var res = context.Reservas.Where(o => o.Id == reserva.Id).FirstOrDefault();
            res.Fecha = reserva.Fecha;
            res.Precio = 0;
            List<ReservaServicio> reservaServicios = new List<ReservaServicio>();
            List<ReservaHora> reservaHoras = new List<ReservaHora>();
            if (servicio.Count() == 0)
                ModelState.AddModelError("servicio", "Seleccione por lo menos uno");
            if (horario.Count() == 0)
                ModelState.AddModelError("horario", "Seleccione por lo menos uno");

            if (reserva.Fecha == DateTime.Parse(FechaActual))
            {
                foreach (var item in horario)
                {
                    if (item + 7 < Int32.Parse(horaActual.ToString("HH")))
                    {
                        ModelState.AddModelError("Hora", "La hora seleccionada ah caducado");
                    }
                }
            }
            if (reserva.Fecha < DateTime.Parse(FechaActual))
            {
                ModelState.AddModelError("Fecha", "La fecha ha caducado");
            }

            var serviciosList = context.Servicios.ToList();

            foreach (var item in servicio)
            {
                foreach (var item2 in serviciosList)
                {
                    if (item == item2.Id)
                    {
                        res.Precio += (item2.Precio * horario.Count);
                    }
                }
            }

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
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha).ToList();
            var reserva = context.Reservas.Where(o => o.Id == id).FirstOrDefault();
            if (reserva.Estado == false)
            {
                var horas = context.ReservaHoras.Where(o => o.Id_Reserva == id).ToList();
                var servicios = context.ReservaServicios.Where(o => o.Id_Reserva == id).ToList();
                context.Reservas.Remove(reserva);
                context.ReservaHoras.RemoveRange(horas);
                context.ReservaServicios.RemoveRange(servicios);
                context.SaveChanges();
            }
            return View("Reservas", reservas);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Aceptar(int id)
        {
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha).ToList();
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            if (user.Id_Rol == 1)
            {
                var reserva = context.Reservas.Where(o => o.Id == id).FirstOrDefault();
                reserva.Estado = true;
                context.Reservas.Update(reserva);
                context.SaveChanges();
                return View("Reservas", reservas);
            }
            else
            {
                return View("Reservas", reservas);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
