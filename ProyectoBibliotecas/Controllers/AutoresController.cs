using Microsoft.AspNetCore.Mvc;
using NuguetProyectoBibliotecas.Models;
using ProyectoBibliotecas.Services;
using System.Collections.Generic;

namespace ProyectoBibliotecas.Controllers
{
    public class AutoresController : Controller
    {
        private ServiceApiBibliotecas service;
        private ServiceStorageBlobs storageBlob;

        public AutoresController(ServiceApiBibliotecas service, ServiceStorageBlobs storageBlob)
        {
            this.service = service;
            this.storageBlob = storageBlob;
        }

        public async Task<IActionResult> IndexAutores()
        {
            List<Autor> autores = await service.GetAutores();
            foreach(Autor a in autores)
            {
                a.IMAGEN = await this.storageBlob.GetUrl("autores", a.IMAGEN);
            }
            return View(autores);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAutores(string search)
        {
            List<Autor> autores = await this.service.SearchAutor(search);
            foreach (Autor a in autores)
            {
                a.IMAGEN = await this.storageBlob.GetUrl("autores", a.IMAGEN);
            }
            return View(autores);
        }

        public async Task<IActionResult> DetailsAutor(int id)
        {
            List<LibroDefault> libros = await this.service.GetLibrosAutor(id);
            foreach(LibroDefault l in libros)
            {
                l.IMAGEN = await this.storageBlob.GetUrl("libros", l.IMAGEN);
            }
            ViewData["LIBROS"] = libros;

            Autor autor = await this.service.GetDatosAutor(id);
            autor.IMAGEN = await this.storageBlob.GetUrl("autores", autor.IMAGEN);
            return View(autor);
        }

        [HttpPost]
        public async Task<IActionResult> DetailsAutor(int id, string input)
        {
            List<LibroDefault> libros = await this.service.SearchLibroAutor(id, input);
            foreach (LibroDefault l in libros)
            {
                l.IMAGEN = await this.storageBlob.GetUrl("libros", l.IMAGEN);
            }
            ViewData["LIBROS"] = libros;

            Autor autor = await this.service.GetDatosAutor(id);
            autor.IMAGEN = await this.storageBlob.GetUrl("autores", autor.IMAGEN);
            return View(autor);
        }
    }
}
