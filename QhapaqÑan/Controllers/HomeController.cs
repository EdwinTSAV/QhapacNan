using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QhapaqÑan.Clases;
using QhapaqÑan.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace QhapaqÑan.Controllers
{
    public class HomeController : Controller
    {
        private readonly PContext context;
        private readonly IWebHostEnvironment hosting;

        public HomeController(PContext context, IWebHostEnvironment hosting)
        {
            this.context = context;
            this.hosting = hosting;
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

        [HttpGet]
        public IActionResult HorasDisOcu(string fecha)
        {
            var rHorarios = context.ReservaHoras.Where(o => o.Reserva.Fecha_Post == DateTime.Parse(fecha)).ToList();
            ViewBag.Horarios = context.Horas.ToList();
            return View(rHorarios);
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
            if (reserva.Fecha_Post == DateTime.Parse(fechaActual))
            {
                foreach (var item in horario)
                {
                    if (item + 7 < Int32.Parse(horaActual.ToString("HH")))
                    {
                        ModelState.AddModelError("Hora", "La hora seleccionada ah caducado");
                    }
                }
            }
            if (reserva.Fecha_Post < DateTime.Parse(fechaActual))
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
            reserva.Fecha_Pre = DateTime.Now;
            reserva.Hora_Fecha = DateTime.Now;
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
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha_Post).ToList();
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
            ViewBag.RServicios = context.ReservaServicios.Where(o => o.Id_Reserva == id).ToList();
            ViewBag.RHorarios = context.ReservaHoras.Where(o => o.Id_Reserva == id).ToList();
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
            res.Fecha_Post = reserva.Fecha_Post;
            res.Precio = 0;
            List<ReservaServicio> reservaServicios = new List<ReservaServicio>();
            List<ReservaHora> reservaHoras = new List<ReservaHora>();
            if (servicio.Count() == 0)
                ModelState.AddModelError("servicio", "Seleccione por lo menos uno");
            if (horario.Count() == 0)
                ModelState.AddModelError("horario", "Seleccione por lo menos uno");

            if (reserva.Fecha_Post == DateTime.Parse(FechaActual))
            {
                foreach (var item in horario)
                {
                    if (item + 7 < Int32.Parse(horaActual.ToString("HH")))
                    {
                        ModelState.AddModelError("Hora", "La hora seleccionada ah caducado");
                    }
                }
            }
            if (reserva.Fecha_Post < DateTime.Parse(FechaActual))
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

            reserva.Fecha_Pre = DateTime.Now;
            reserva.Hora_Fecha = DateTime.Now;

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
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            ViewBag.User = user;

            ViewBag.Servicios = context.ReservaServicios.Include(o => o.Servicios).ToList();
            ViewBag.Horarios = context.ReservaHoras.Include(o => o.Hora).ToList().OrderBy(o => o.Hora.Hora_Inicio);
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha_Post).ToList();
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
            ViewBag.Servicios = context.ReservaServicios.Include(o => o.Servicios).ToList();
            ViewBag.Horarios = context.ReservaHoras.Include(o => o.Hora).ToList().OrderBy(o => o.Hora.Hora_Inicio);
            var reservas = context.Reservas.OrderByDescending(o => o.Fecha_Post).ToList();
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            ViewBag.User = user;
            if (user.Id_Rol == 1)
            {
                var reserva = context.Reservas.Where(o => o.Id == id).FirstOrDefault();
                reserva.Estado = true;
                var userReserva = context.Usuarios.Where(o => o.DNI == reserva.DNI_User).FirstOrDefault();
                context.Reservas.Update(reserva);

                //TODO
                var horaActual = DateTime.Now.Date.ToString();
                var reservasCanceladas = context.Reservas.Where(o => o.Fecha_Post < DateTime.Parse(horaActual) && o.Estado == false).ToList();

                foreach (var item in reservasCanceladas)
                {
                    var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).ToList();
                    var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                    context.ReservaHoras.RemoveRange(resHor);
                    context.ReservaServicios.RemoveRange(resSer);
                    context.Reservas.RemoveRange(reservasCanceladas);
                }

                var reservasNoPagadas = context.Reservas.Where(o => o.Fecha_Pre.AddDays(2) < DateTime.Parse(horaActual) && o.Estado == false).ToList();
                foreach (var item in reservasNoPagadas)
                {
                    var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).ToList();
                    var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                    context.ReservaHoras.RemoveRange(resHor);
                    context.ReservaServicios.RemoveRange(resSer);
                    context.Reservas.RemoveRange(reservasNoPagadas);
                }

                var reservasNoPagadas2Dias = context.Reservas.Where(o => o.Fecha_Pre.AddDays(2) == o.Fecha_Post && o.Estado == false).ToList();

                foreach (var item in reservasNoPagadas2Dias)
                {
                    if (DateTime.Parse(item.Fecha_Pre.ToString()).AddDays(1).AddHours(18) == DateTime.Parse(horaActual).AddHours(18))
                    {
                        var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).ToList();
                        var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                        context.ReservaHoras.RemoveRange(resHor);
                        context.ReservaServicios.RemoveRange(resSer);
                        context.Reservas.RemoveRange(reservasNoPagadas2Dias);
                    }
                }

                var reservasNoPagadas1Dias = context.Reservas.Where(o => o.Fecha_Pre.AddDays(1) == o.Fecha_Post && o.Estado == false).ToList();

                foreach (var item in reservasNoPagadas1Dias)
                {
                    if (item.Fecha_Post.AddHours(10) == DateTime.Parse(horaActual).AddHours(10))
                    {
                        var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).ToList();
                        var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                        context.ReservaHoras.RemoveRange(resHor);
                        context.ReservaServicios.RemoveRange(resSer);
                        context.Reservas.RemoveRange(reservasNoPagadas1Dias);
                    }
                }

                var reservasNoPagadas0Dias = context.Reservas.Where(o => o.Fecha_Pre == o.Fecha_Post && o.Estado == false).ToList();
                
                foreach (var item in reservasNoPagadas0Dias)
                {
                    if (item.Hora_Fecha >= DateTime.Parse(horaActual).AddHours(8))
                    {
                        if (item.Hora_Fecha.AddHours(2) < DateTime.Now)
                        {
                            var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).OrderBy(o => o.Hora.Hora_Inicio).ToList();
                            var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                            context.ReservaHoras.RemoveRange(resHor);
                            context.ReservaServicios.RemoveRange(resSer);
                            context.Reservas.RemoveRange(reservasNoPagadas0Dias);
                        }
                    }
                    else if (DateTime.Now > DateTime.Parse(horaActual).AddHours(10))
                    {
                        var resHor = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).OrderBy(o => o.Hora.Hora_Inicio).ToList();
                        var resSer = context.ReservaServicios.Where(o => o.Id_Reserva == item.Id).ToList();
                        context.ReservaHoras.RemoveRange(resHor);
                        context.ReservaServicios.RemoveRange(resSer);
                        context.Reservas.RemoveRange(reservasNoPagadas0Dias);
                    }
                }

                var resHoraA = context.ReservaHoras.Where(o => o.Id_Reserva == id).ToList();
                foreach (var item in resHoraA)
                {
                    item.Estado = true;
                }
                context.UpdateRange(resHoraA);
                context.SaveChanges();


                var cantidad = reservas.Where(o => o.DNI_User == User.Claims.FirstOrDefault().Value).Count();
                
                var file = Path.Combine(hosting.WebRootPath, Path.Combine("Tickets", reserva.Id + user.DNI + reserva.Precio + ".pdf"));
                using (var archivo = new FileStream(file, FileMode.Create))
                {
                    Document pdf = new Document(PageSize.A4,25,25,25,25);
                    PdfWriter escribir = PdfWriter.GetInstance(pdf, archivo);
                    pdf.Open();
                    string html;
                    using (var sr = new StreamReader(Path.Combine(hosting.WebRootPath, Path.Combine("Tickets", "Ticket.cshtml"))))
                    {
                        html = sr.ReadToEnd();
                    }
                    html = html.Replace("NombreyApellidosdeCliente", userReserva.Nombres +" "+ userReserva.Ap_Paterno + " " + userReserva.Ap_Materno);
                    html = html.Replace("3434343434", reserva.DNI_User);
                    html = html.Replace("00000111", reserva.Id + user.DNI + reserva.Precio);
                    html = html.Replace("FECHAReserva", reserva.Fecha_Post.ToString("D"));
                    html = html.Replace("FECHAPagada", DateTime.Now.ToString("F"));
                    html = html.Replace("FECHAPagada", DateTime.Now.ToString("F"));
                    html = html.Replace("TOTALAPagar", reserva.Precio.ToString());

                    string filaser = string.Empty;
                    string filaHor = string.Empty;

                    var servicios = context.ReservaServicios.Where(o => o.Id_Reserva == id).Include(o => o.Servicios).ToList();
                    var horas = context.ReservaHoras.Where(o => o.Id_Reserva == id).Include(o => o.Hora).ToList();
                    foreach (var item in horas)
                    {
                        filaHor += "<li>" + item.Hora.Hora_Inicio+":00 - " + item.Hora.Hora_Fin + ":00" + "</li>";
                    }
                    foreach (var item in servicios)
                    {
                        filaser += "<tr>"
                            + "<td>" + item.Servicios.Nombre + "</td>"
                            + "<td>" + item.Servicios.Precio + "</td>"
                            + "<td>" + horas.Count() + "</td>"
                            + "</tr>";
                    }

                    html = html.Replace("FILASSer", filaser);
                    html = html.Replace("HORASReserva", filaHor);


                    using (var sr = new StringReader(html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(escribir, pdf, sr);
                    }
                    pdf.Close();
                    archivo.Close();
                }


                return View("Reservas", reservas);
            }
            else
            {
                return View("Reservas", reservas);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Reporte()
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            ViewBag.User = user;
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            var user = context.Usuarios.Where(o => o.DNI == User.Claims.FirstOrDefault().Value).FirstOrDefault();
            ViewBag.User = user;

            var reservas = context.Reservas.Where(o => o.Fecha_Post >= DateTime.Parse(FechaInicio.ToString("d")) && o.Fecha_Post < DateTime.Parse(FechaFin.ToString("d"))).ToList().OrderBy(o => o.Fecha_Post);

            if (FechaInicio > FechaFin)
                ModelState.AddModelError("FechaI", "Esta fecha debe ser menor a la fecha Final");
            if (FechaFin > DateTime.Parse(DateTime.Now.ToString("d")))
                ModelState.AddModelError("FechaF", "Esta fecha aun no llega");

            if (ModelState.IsValid)
            {
                var file = Path.Combine(hosting.WebRootPath, Path.Combine("Reportes", FechaInicio.ToString("ddMMyyy") + FechaFin.ToString("ddMMyyyy") + ".pdf"));
                using (var archivo = new FileStream(file, FileMode.Create))
                {
                    Document pdf = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter escribir = PdfWriter.GetInstance(pdf, archivo);
                    pdf.Open();
                    string html;
                    using (var sr = new StreamReader(Path.Combine(hosting.WebRootPath, Path.Combine("Reportes", "Reportes.cshtml"))))
                    {
                        html = sr.ReadToEnd();
                    }
                    html = html.Replace("FECHAINICIO", FechaInicio.ToString("d"));
                    html = html.Replace("FECHAFIN", FechaFin.ToString("d"));
                    html = html.Replace("FECHAACTUAL", DateTime.Now.ToString("f"));

                    string filaser = string.Empty;

                    foreach (var item in reservas)
                    {
                        var horas = context.ReservaHoras.Where(o => o.Id_Reserva == item.Id).Include(o => o.Hora).Count();
                        if (item.Estado == true)
                        {
                            filaser += "<tr>"
                            + "<td>" + item.DNI_User + "</td>"
                            + "<td>" + item.Fecha_Post + "</td>"
                            + "<td>" + item.Precio + "</td>"
                            + "<td> ACEPTADO </td>"
                            + "<td>" + horas + "</td>"
                            + "</tr>";
                        }
                        else
                        {
                            filaser += "<tr>"
                            + "<td>" + item.DNI_User + "</td>"
                            + "<td>" + item.Fecha_Post + "</td>"
                            + "<td>" + item.Precio + "</td>"
                            + "<td> CANCELADO </td>"
                            + "<td>" + horas + "</td>"
                            + "</tr>";
                        }
                    }

                    html = html.Replace("FILASRESERV", filaser);


                    using (var sr = new StringReader(html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(escribir, pdf, sr);
                    }
                    pdf.Close();
                    archivo.Close();
                }
            }
            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
