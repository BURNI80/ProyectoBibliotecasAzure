using Microsoft.AspNetCore.Mvc;
using NuguetProyectoBibliotecas.Models;
using ProyectoBibliotecas.Services;

namespace ProyectoBibliotecas.Controllers
{
    public class BibliotecasController : Controller
    {
        private ServiceApiBibliotecas service;
        private ServiceStorageBlobs serviceStorageBlobs;

        public BibliotecasController(ServiceApiBibliotecas service, ServiceStorageBlobs serviceStorageBlobs)
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
        }

        public async Task<IActionResult> IndexBibliotecas()
        {
            List<Biblioteca> biblios = await this.service.GetBibliotecas();
            foreach(Biblioteca biblioteca in biblios)
            {
                biblioteca.IMAGEN = await this.serviceStorageBlobs.GetUrl("bibliotecas", biblioteca.IMAGEN);
            }
            return View(biblios);
        }

        [HttpPost]
        public async Task<IActionResult> IndexBibliotecas(string search)
        {
            List<Biblioteca> biblios = await this.service.SearchBiblioteca(search);
            foreach (Biblioteca biblioteca in biblios)
            {
                biblioteca.IMAGEN = await this.serviceStorageBlobs.GetUrl("bibliotecas", biblioteca.IMAGEN);
            }
            return View(biblios);
        }

        [HttpGet]
        public async Task<IActionResult> DetailsBiblioteca(int id)
        {
            List<LibroDisponibilidad> libros = await this.service.GetLibrosBiblioteca(id);
            foreach (LibroDisponibilidad l in libros)
            {
                l.IMAGEN = await this.serviceStorageBlobs.GetUrl("libros", l.IMAGEN);
            }
            ViewData["LISTALIBROS"] = libros;

            Biblioteca biblio = await this.service.DetailsBiblioteca(id);
            biblio.IMAGEN = await this.serviceStorageBlobs.GetUrl("bibliotecas", biblio.IMAGEN);
            return View(biblio);
        }

        [HttpPost]
        public async Task<IActionResult> DetailsBiblioteca(int id, string input, char option)
        {
            List<LibroDisponibilidad> libros = await this.service.SearchLibroBiblioteca(id, input, option);
            if (input == null)
            {
                libros = await this.service.GetLibrosBiblioteca(id);
            }
            foreach (LibroDisponibilidad l in libros)
            {
                l.IMAGEN = await this.serviceStorageBlobs.GetUrl("libros", l.IMAGEN);
            }
            ViewData["LISTALIBROS"] = libros;

            Biblioteca biblio = await this.service.DetailsBiblioteca(id);
            biblio.IMAGEN = await this.serviceStorageBlobs.GetUrl("bibliotecas", biblio.IMAGEN);
            return View(biblio); ;
        }
    }
}
