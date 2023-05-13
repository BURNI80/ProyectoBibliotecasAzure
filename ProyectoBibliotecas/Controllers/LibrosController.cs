using Microsoft.AspNetCore.Mvc;
using NuguetProyectoBibliotecas.Models;
using ProyectoBibliotecas.Services;
using System.Collections.Generic;
using System.Net;

namespace ProyectoBibliotecas.Controllers
{
    public class LibrosController : Controller
    {
        private ServiceApiBibliotecas service;
        private ServiceStorageBlobs storageBlob;

        public LibrosController(ServiceApiBibliotecas service, ServiceStorageBlobs storageBlob)
        {
            this.service = service;
            this.storageBlob = storageBlob;
        }

        public async Task<IActionResult> IndexLibros()
        {

            List<LibroDisponibilidad> libros = await this.service.SearchLibroBiblioteca(-1, null, 'T');
            foreach (LibroDisponibilidad l in libros)
            {
                l.IMAGEN = await this.storageBlob.GetUrl("libros", l.IMAGEN);
            }
            return View(libros);
        }


        [HttpPost]
        public async Task<IActionResult> IndexLibros(string input, char option)
        {
            List<LibroDisponibilidad> libros = await this.service.SearchLibroBiblioteca(-1, input, option);
            foreach (LibroDisponibilidad l in libros)
            {
                l.IMAGEN = await this.storageBlob.GetUrl("libros", l.IMAGEN);
            }
            return View(libros);
        }


        public async Task<IActionResult> DetailsLibro(int id)
        {
            ViewData["VALORACIONES"] = await this.service.GetValoraciones(id);
            ViewData["BIBLIOTECAS"] = await this.service.GetLibroDisponible(id);
            ViewData["FECHASNO"] = GetDiasReservado(id, 1);
            string dni;


            if (HttpContext.User.Identity.IsAuthenticated == false)
            {
                dni = null;
                ViewData["LISTADESEOS"] = -2;
            }
            else
            {
                dni = HttpContext.User.Identity.Name;
                ViewData["LISTADESEOS"] = await this.service.LibroDeseo(id, dni);
            }
            this.ViewData["COMENTARIOS"] = await this.service.GetComentarios(dni, id);

            ViewData["DNI"] = dni;

            Libro libro = await this.service.GetDatosLibro(id);
            libro.IMAGEN = await this.storageBlob.GetUrl("libros", libro.IMAGEN);
            return View(libro);
        }

        [HttpPost]
        public async Task<ActionResult> DetailsLibro(int orden, int id, string textoComentario, int rating, int idLibro)
        {
            string dni = HttpContext.User.Identity.Name;
            ViewData["LISTADESEOS"] = await this.service.LibroDeseo(id, dni);
            ViewData["FECHASNO"] = GetDiasReservado(id, 0);
            ViewData["BIBLIOTECAS"] = await this.service.GetLibroDisponible(id);
            if (orden != 0)
            {
                await this.service.LikeComentario(orden, id, dni, HttpContext.Session.GetString("token"));

            }
            else
            {
                DateTime fecha = DateTime.Now;
                await this.service.PostComentario(idLibro, dni, fecha, textoComentario, rating, HttpContext.Session.GetString("token"));

               ViewData["VALORACIONES"] = await this.service.GetValoraciones(id);
                ViewData["COMENTARIOS"] = await this.service.GetComentarios(dni, id);
                ViewData["DNI"] = dni;
                Libro libro = await this.service.GetDatosLibro(id);
                libro.IMAGEN = await this.storageBlob.GetUrl("libros", libro.IMAGEN);
                return View(libro);
            }
            return new EmptyResult();

        }


        [HttpPost]
        public void AddListaLibro(string dni, int idLibro, int orden)
        {
            this.service.AddListaLibro(dni, idLibro, orden, HttpContext.Session.GetString("token"));
        }


        [HttpPost]
        public async Task<List<string>> GetDiasReservado(int id, int idBiblio)
        {
            List<Reserva> reservas = await this.service.GetResrevasLibro(id, idBiblio, HttpContext.Session.GetString("token"));
            List<string> resultado = new List<string>();
            if (reservas != null)
            {
                foreach (Reserva reserva in reservas)
                {
                    List<string> arr = this.service.GetDaysBetween(reserva.FECHA_INICIO, reserva.FECHA_FIN);
                    resultado.AddRange(arr);
                }
            }
            return resultado;
        }

        [HttpPost]
        public async Task ReservarLibro(int idLibro, int idBiblio, DateTime fechaInicio, DateTime fechaFin)
        {
            string dni = HttpContext.User.Identity.Name;
            await this.service.CreateReserva(dni, idLibro, idBiblio, fechaInicio, fechaFin, HttpContext.Session.GetString("token"));
        }
    }
}
