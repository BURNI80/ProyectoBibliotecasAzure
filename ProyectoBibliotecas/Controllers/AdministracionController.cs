using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuguetProyectoBibliotecas.Models;
using ProyectoBibliotecas.Filters;
using ProyectoBibliotecas.Services;
using System.Collections.Generic;

namespace ProyectoBibliotecas.Controllers
{

    public class AdministracionController : Controller
    {
        private ServiceApiBibliotecas service;
        private ServiceStorageBlobs storageBlobs;

        public AdministracionController(ServiceApiBibliotecas service, ServiceStorageBlobs storageBlobs)
        {
            this.service = service;
            this.storageBlobs = storageBlobs;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Perfil_Info(string id)
        {
            string token = HttpContext.Session.GetString("token");
            id = HttpContext.User.Identity.Name;
            ViewData["NCOMENTARIOS"] = await this.service.NComentarios(id,token);
            ViewData["NLEIDOS"] = await this.service.NLibrosLeidos(id, token);
            ViewData["NRESEÑAS"] = await this.service.NReseñas(id, token);
            return View(await this.service.GetUsuario(id, token));
        }


        [AuthorizeUsers]
        public async Task<IActionResult> EditPerfil()
        {
            string id = HttpContext.User.Identity.Name;
            return View(await this.service.GetUsuario(id, HttpContext.Session.GetString("token")));
        }


        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> EditPerfil(string nombre, string apellido, string email, int telefono, string usuario)
        {
            string id = HttpContext.User.Identity.Name;
            await this.service.UpdateUsuario(id, nombre, apellido, email, telefono, usuario, HttpContext.Session.GetString("token"));
            return RedirectToAction("Perfil_Info", "Administracion");
        }


        [AuthorizeUsers]
        public async Task<IActionResult> Perfil_Comentarios(string id)
        {
            id = HttpContext.User.Identity.Name;
            return View(await this.service.GetComentariosUsuario(id, HttpContext.Session.GetString("token")));
        }
        
        
        [AuthorizeUsers]
        [HttpPost]
        public async Task DeleteComentario(int idComentario)
        {
            await this.service.DeleteComentario(idComentario, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers]
        public async Task<IActionResult> Perfil_Reservas(string id)
        {
            id = HttpContext.User.Identity.Name;
            return View(await this.service.GetReservasUsuario(id, HttpContext.Session.GetString("token")));
        }


        public async Task<IActionResult> Lista_LibrosFavoritos(string id, string token)
        {
            string dniUser = HttpContext.User.Identity.Name;
            if (id == null)
            {
                return RedirectToAction("Login", "Managed");
            }
            if (id.Equals(dniUser))
            {
                List<LibroDeseo> libros = await this.service.GetFavoritos(id, HttpContext.Session.GetString("token"));
                foreach(LibroDeseo l in libros)
                {
                    l.IMAGEN = await this.storageBlobs.GetUrl("libros", l.IMAGEN);
                }
                return View(libros);
            }
            else
            {
                Compartido share = await this.service.GetShare(id, HttpContext.Session.GetString("token"));
                if (share == null)
                {
                    return RedirectToAction("NoAccess", "Managed");
                }
                else
                {
                    if (share.TOKEN.Equals(token))
                    {
                        List<LibroDeseo> libros = await this.service.GetFavoritos(id, HttpContext.Session.GetString("token"));
                        foreach (LibroDeseo l in libros)
                        {
                            l.IMAGEN = await this.storageBlobs.GetUrl("libros", l.IMAGEN);
                        }
                        return View(libros);
                    }
                    else
                    {
                        return RedirectToAction("NoAccess", "Managed");
                    }
                }
            }
        }


        public async Task<IActionResult> Lista_LibrosLeidos(string id, string? token)
        {
            string dniUser = HttpContext.User.Identity.Name;
            if (id == null)
            {
                return RedirectToAction("Login", "Managed");
            }
            if (id.Equals(dniUser))
            {
                List<LibroDeseo> libros = await this.service.GetFavoritos(id, HttpContext.Session.GetString("token"));
                foreach (LibroDeseo l in libros)
                {
                    l.IMAGEN = await this.storageBlobs.GetUrl("libros", l.IMAGEN);
                }
                return View(libros);
            }
            else
            {
                Compartido share = await this.service.GetShare(id, HttpContext.Session.GetString("token"));
                if (share == null)
                {
                    return RedirectToAction("NoAccess", "Managed");
                }
                else
                {
                    if (share.TOKEN.Equals(token))
                    {
                        List<LibroDeseo> libros = await this.service.GetFavoritos(id, HttpContext.Session.GetString("token"));
                        foreach (LibroDeseo l in libros)
                        {
                            l.IMAGEN = await this.storageBlobs.GetUrl("libros", l.IMAGEN);
                        }
                        return View(libros);
                    }
                    else
                    {
                        return RedirectToAction("NoAccess", "Managed");
                    }
                }
            }
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> Admin_Panel()
        {
            if (HttpContext.User.IsInRole("EDITOR"))
            {
                ViewData["BIBLIOTECAS"] = await this.service.GetBibliotecasEditables(HttpContext.User.Identity.Name, HttpContext.Session.GetString("token"));
            }
            if (HttpContext.User.IsInRole("ADMIN"))
            {
                ViewData["BIBLIOTECAS"] = await this.service.GetBibliotecasSimples();
            }
            return View();
        }


        [AuthorizeUsers]
        [HttpPost]
        public async Task<string> Share(string dni, string path)
        {
            string token = this.service.GenerateToken();
            Compartido realToken = await this.service.GetToken(dni, token);
            return path + "?token=" + realToken.TOKEN;
        }


        [Authorize]
        [HttpPost]
        public async Task EliminarReserva(int id)
        {
            await this.service.DeleteReserva(id, HttpContext.Session.GetString("token"));
        }







        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> Bibliotecas()
        {
            return View(await this.service.GetBibliotecas());
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task EliminarBiblioteca(int id)
        {
            await this.service.DeleteBiblioteca(id, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public IActionResult NuevaBiblioteca()
        {
            return View();
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> NuevaBiblioteca(string nombre, string direccion, int telefono, string web, TimeSpan hora_apertura, TimeSpan hora_cierre, IFormFile imagen)
        {
            if (imagen == null)
            {
                await this.service.AddBiblio(nombre, direccion, telefono, web, hora_apertura, hora_cierre, null, HttpContext.Session.GetString("token"));

            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("bibliotecas", fileName, stream);
                }
                await this.service.AddBiblio(nombre, direccion, telefono, web, hora_apertura, hora_cierre, fileName.ToString(), HttpContext.Session.GetString("token"));

            }
            return RedirectToAction("Bibliotecas", "Administracion");
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> EditarBiblioteca(int id)
        {
            return View(await this.service.DetailsBiblioteca(id));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> EditarBiblioteca(int id, string nombre, string direccion, int telefono, string web, TimeSpan hora_apertura, TimeSpan hora_cierre, IFormFile imagen, string nImagen)
        {
            if (imagen == null)
            {
                await this.service.UpdateBiblio(id, nombre, direccion, telefono, web, hora_apertura, hora_cierre, nImagen, HttpContext.Session.GetString("token"));
            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("bibliotecas", fileName, stream);
                }
                await this.service.UpdateBiblio(id, nombre, direccion, telefono, web, hora_apertura, hora_cierre, fileName, HttpContext.Session.GetString("token"));
            }
            return RedirectToAction("Bibliotecas", "Administracion");

        }





        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> Libros()
        {
            return View(await this.service.GetLibrosTodos(HttpContext.Session.GetString("token")));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task EliminarLibro(int id)
        {
            await this.service.DeleteLibro(id, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> NuevoLibro()
        {
            ViewData["AUTORES"] = await this.service.GetAutores();
            return View();
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> NuevoLibro(string nombre, int numpag, IFormFile imagen, string urlcompra, string descripcion, string idioma, DateTime fecha_publicacion, int idautor)
        {
            if (imagen == null)
            {
                await this.service.AddLibro(nombre, numpag, null, urlcompra, descripcion, idioma, fecha_publicacion, idautor, HttpContext.Session.GetString("token"));
            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("libros", fileName, stream);
                }
                await this.service.AddLibro(nombre, numpag, fileName, urlcompra, descripcion, idioma, fecha_publicacion, idautor, HttpContext.Session.GetString("token"));
            }
            return RedirectToAction("Libros", "Administracion");
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> EditarLibro(int id)
        {
            ViewData["AUTORES"] = await this.service.GetAutores();
            return View(await this.service.GetDatosLibroDef(id));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> EditarLibro(int id, string nombre, int numpag, IFormFile imagen, string urlcompra, string descripcion, string idioma, DateTime fecha_publicacion, int idautor, string nImagen)
        {
            if (imagen == null)
            {
                await this.service.UpdateLibro(id, nombre, numpag, nImagen, urlcompra, descripcion, idioma, fecha_publicacion, idautor, HttpContext.Session.GetString("token"));
            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("libros", fileName, stream);
                }
                await this.service.UpdateLibro(id, nombre, numpag, fileName, urlcompra, descripcion, idioma, fecha_publicacion, idautor, HttpContext.Session.GetString("token"));
            }
            return RedirectToAction("Libros", "Administracion");
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> Autores()
        {
            return View(await this.service.GetAutores());
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task EliminarAutor(int id)
        {
            await this.service.DeleteAutor(id, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public IActionResult NuevoAutor()
        {
            return View();
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> NuevoAutor(string nombre, string nacionalidad, DateTime fechaNac, IFormFile imagen, string descripcion, int numLibros, string wiki)
        {
            if (imagen == null)
            {
                await this.service.AddAutor(nombre, nacionalidad, fechaNac, null, descripcion, numLibros, wiki, HttpContext.Session.GetString("token"));
            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("autores", fileName, stream);
                }
                await this.service.AddAutor(nombre, nacionalidad, fechaNac, fileName, descripcion, numLibros, wiki, HttpContext.Session.GetString("token"));
            }
            return RedirectToAction("Autores", "Administracion");
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> EditarAutor(int id)
        {
            return View(await this.service.GetDatosAutor(id));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> EditarAutor(int id, string nombre, string nacionalidad, DateTime fechaNac, IFormFile imagen, string descripcion, int numLibros, string wiki, string nImagen)
        {
            if (imagen == null)
            {
                await this.service.UpdateAutor(id, nombre, nacionalidad, fechaNac, nImagen, descripcion, numLibros, wiki, HttpContext.Session.GetString("token"));
            }
            else
            {
                string fileName = imagen.FileName;
                using (Stream stream = imagen.OpenReadStream())
                {
                    await this.storageBlobs.UploadBlobAsync("autores", fileName, stream);
                }
                await this.service.UpdateAutor(id, nombre, nacionalidad, fechaNac, fileName, descripcion, numLibros, wiki, HttpContext.Session.GetString("token"));

            }
            return RedirectToAction("Autores", "Administracion");
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> LibrosBiblioteca(int id)
        {
            ViewData["LIBROSADD"] = await this.service.GetLibrosNotInBiblioteca(id, HttpContext.Session.GetString("token"));
            @ViewData["IDBIBLIO"] = id;
            return View(await this.service.GetLibrosBiblioteca(id));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task AddLibroBiblio(int idBiblio, int idLibro)
        {
            await this.service.AddLibroBiblio(idBiblio, idLibro , HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task EliminarLibroBiblioteca(int idBiblio, int idLibro)
        {
            await this.service.DeleteLibroBiblio(idBiblio, idLibro, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        public async Task<IActionResult> PrestamosBiblioteca(int id)
        {
            @ViewData["IDBIBLIO"] = id;
            return View(await this.service.GetReservasBiblio(id, HttpContext.Session.GetString("token")));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task EliminarPrestamoBiblioteca(int id)
        {
            await this.service.DeleteReserva(id, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task RecogerLibro(int idPrestamo, int idBiblio)
        {
            await this.service.RecogerLibro(idPrestamo, idBiblio, HttpContext.Session.GetString("token"));
        }


        [AuthorizeUsers(Policy = "ADMIN")]
        [AuthorizeUsers]
        [HttpPost]
        public async Task DevolverLibro(int idPrestamo, int idBiblio)
        {
            await this.service.DevolverLibro(idPrestamo, idBiblio, HttpContext.Session.GetString("token"));
        }
    }
}
