using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ProyectoBibliotecas.Models;
using NuguetProyectoBibliotecas.Models;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ProyectoBibliotecas.Services
{
    public class ServiceApiBibliotecas
    {

        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceApiBibliotecas(IConfiguration configuration)
        {
            this.UrlApi = configuration.GetValue<string>("ConnectionStrings:ApiBibliotecas");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    DniUsuario = username,
                    Contraseña = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token = jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiPrivateAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        public async Task<Usuario> GetUsu(string dni, string pass)
        {
            LoginModel res = new LoginModel
            {
                DniUsuario = dni,
                Contraseña = pass
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Auth/GetUsu";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(res);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Usuario usuario = JsonConvert.DeserializeObject<Usuario>(responseString);
                    return usuario;
                }
                else
                {
                    throw new Exception($"Error al intentar obtener usuario: {response.StatusCode}");
                }
            }
        }

        public async Task Register(string nombre, string apellidos, string dni, string usuario, string password, string email, int telefono)
        {
            var myObject = new { Name = "John", Age = 30 };


            var res = new 
            {
                DNI_USUARIO = dni,
                USUARIO = usuario,
                CONTRASEÑA = password,
                ROL = "",
                NOMBRE = nombre,
                APELLIDO = apellidos,
                EMAIL = email,
                TELEFONO = telefono,
                SALT = "",
                PASSWORD = ""
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Usuarios/Register";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(res);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }


        //METODOS


        //BIBLIOTECAS
        public async Task<List<Biblioteca>> GetBibliotecas()
        {
            string request = "/api/Bibliotecas/GetBibliotecas";
            List<Biblioteca> biblios = await this.CallApiAsync<List<Biblioteca>>(request);
            return biblios;
        }

        public async Task<List<Biblioteca>> SearchBiblioteca(string input)
        {
            string request = "/api/Bibliotecas/SearchBibliotecasNombre/" + input;
            List<Biblioteca> biblios = await this.CallApiAsync<List<Biblioteca>>(request);
            return biblios;
        }

        public async Task<Biblioteca> DetailsBiblioteca(int id)
        {
            string request = "/api/Bibliotecas/FindBiblioteca/" + id;
            Biblioteca biblio = await this.CallApiAsync<Biblioteca>(request);
            return biblio;
        }

        public async Task<List<LibroDisponibilidad>> GetLibrosBiblioteca(int id)
        {
            string request = "/api/Libros/GetLibrosDispoBiblioteca/" + id;
            List<LibroDisponibilidad> libros = await this.CallApiAsync<List<LibroDisponibilidad>>(request);
            return libros;
        }

        public async Task<List<LibroDisponibilidad>> SearchLibroBiblioteca(int id, string input, char option)
        {
            string request;
            if (input == null)
            {
                request = "/api/Libros/GetLibrosDispoBiblioteca/" + id + "/null/" + option;

            }
            else
            {
                request = "/api/Libros/GetLibrosDispoBiblioteca/" + id + "/" + input + "/" + option;
            }
            List<LibroDisponibilidad> biblio = await this.CallApiAsync<List<LibroDisponibilidad>>(request);
            return biblio;
        }


        //AUTORES
        public async Task<List<Autor>> GetAutores()
        {
            string request = "/api/Autores";
            List<Autor> autores = await this.CallApiAsync<List<Autor>>(request);
            return autores;
        }

        public async Task<List<Autor>> SearchAutor(string input)
        {
            string request = "/api/Autores/SearchAutorNombre/" + input;
            List<Autor> autores = await this.CallApiAsync<List<Autor>>(request);
            return autores;
        }

        public async Task<List<LibroDefault>> GetLibrosAutor(int id)
        {
            string request = "/api/Libros/GetLibrosAutor/" + id;
            List<LibroDefault> libros = await this.CallApiAsync<List<LibroDefault>>(request);
            return libros;
        }

        public async Task<Autor> GetDatosAutor(int id)
        {
            string request = "/api/Autores/GetDatosAutor/" + id;
            Autor autor = await this.CallApiAsync<Autor>(request);
            return autor;
        }

        public async Task<List<LibroDefault>> SearchLibroAutor(int id, string input)
        {
            string request = "/api/Libros/SearchLibroAutorNombre/" + id + "/" + input;
            List<LibroDefault> autor = await this.CallApiAsync<List<LibroDefault>>(request);
            return autor;
        }



        //LIBROS

        public async Task<Libro> GetDatosLibro(int id)
        {
            string request = "/api/Libros/GetLibro/" + id;
            Libro libro = await this.CallApiAsync<Libro>(request);
            return libro;
        }

        public async Task<LibroDefault> GetDatosLibroDef(int id)
        {
            string request = "/api/Libros/GetLibroDef/" + id;
            LibroDefault libro = await this.CallApiAsync<LibroDefault>(request);
            return libro;
        }

        public async Task<List<BibliotecaSimple>> GetLibroDisponible(int id)
        {
            string request = "/api/Reservas/GetLibroDisponible/" + id;
            return await this.CallApiAsync<List<BibliotecaSimple>>(request);
        }

        public async Task<int> LibroDeseo(int id, string dni)
        {
            string request = "/api/Usuarios/LibroDeseo/" + id + "/" + dni;
            return await this.CallApiAsync<int>(request);
        }

        public async Task AddListaLibro(string dni, int idLibro, int orden, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Usuarios/AddListaLibro/" + dni + "/" + idLibro + "/" + orden;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = "";

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task<List<Reserva>> GetResrevasLibro(int id, int idBiblio, string token)
        {
            string request = "/api/Reservas/GetResrevasLibro/" + id + "/" + idBiblio;
            return await this.CallApiPrivateAsync<List<Reserva>>(request, token);
        }

        public async Task CreateReserva(string dni, int idLibro, int idBiblio, DateTime fechaInicio, DateTime fechaFin, string token)
        {
            Reserva res = new Reserva
            {
                DNI_USUARIO = dni,
                ID_LIBRO = idLibro,
                ID_BIBLIOTECA = idBiblio,
                FECHA_FIN = fechaFin,
                FECHA_INICIO = fechaInicio,
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Reservas/CreateReserva";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        //VALORACIONES & COMENTARIOS

        public async Task<int> GetValoraciones(int id)
        {
            string request = "/api/ComentariosValoraciones/GetValoracionesCount/" + id;
            return await this.CallApiAsync<int>(request);
        }

        public async Task<List<Comentario>> GetComentarios(string dni, int id)
        {
            string request = "";
            if (dni == null)
            {
                request = "/api/ComentariosValoraciones/GetComentariosLibroLikeUsuario/null/" + id;
            }
            else
            {
                request = "/api/ComentariosValoraciones/GetComentariosLibroLikeUsuario/" + dni + "/" + id;
            }
            return await this.CallApiAsync<List<Comentario>>(request);
        }

        public async Task PostComentario(int idLibro, string dni, DateTime fecha, string textoComentario, int rating, string token)
        {
            Comentario comentario = new Comentario
            {
                ID_LIBRO = idLibro,
                USUARIO = dni,
                FECHA_COMENTARIO = fecha,
                MENSAJE = textoComentario,
                COMENTARIO_LIKE = 0,
                ID_COMENTARIO = 0,
                MEGUSTAS = 0
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/ComentariosValoraciones/PostComentario/" + rating;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(comentario);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }


        }

        public async Task LikeComentario(int orden, int id, string dni, string token)
        {
            string request = "/api/ComentariosValoraciones/LikeComentario/" + dni + "/" + id + "/" + orden;
            await this.CallApiPrivateAsync<Task>(request, token);
        }



        //ADMINISTRACION

        public async Task<Usuario> GetUsuario(string id, string token)
        {
            string request = "/api/Usuarios/GetUsuario/" + id;
            return await this.CallApiPrivateAsync<Usuario>(request, token);
        }

        public async Task<int> NReseñas(string id, string token)
        {
            string request = "/api/Usuarios/NumReseñasUsuario/" + id;
            return await this.CallApiPrivateAsync<int>(request, token);
        }

        public async Task<int> NComentarios(string id, string token)
        {
            string request = "/api/Usuarios/NumComentariosUsuario/" + id;
            return await this.CallApiPrivateAsync<int>(request, token);
        }

        public async Task<int> NLibrosLeidos(string id, string token)
        {
            string request = "/api/Usuarios/NLibrosLeidos/" + id;
            return await this.CallApiPrivateAsync<int>(request, token);
        }

        public async Task UpdateUsuario(string dni, string nombre, string apellido, string email, int telefono, string usuario, string token)
        {
            var res = new
            {
                DNI_USUARIO = dni,
                USUARIO = usuario,
                CONTRASEÑA = "",
                ROL = "",
                NOMBRE = nombre,
                APELLIDO = apellido,
                EMAIL = email,
                TELEFONO = telefono,
                SALT = "",
                PASSWORD = ""
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Usuarios/UpdateUsuario";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, content);
            }
        }

        public async Task<List<ComentarioBasico>> GetComentariosUsuario(string id, string token)
        {
            string request = "/api/Usuarios/GetComentariosUsuario/" + id;
            return await this.CallApiPrivateAsync<List<ComentarioBasico>>(request, token);
        }

        public async Task DeleteComentario(int id, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/ComentariosValoraciones/DeleteComentario/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task<List<ReservaUsuario>> GetReservasUsuario(string id, string token)
        {
            string request = "/api/Usuarios/GetReservasUsuario/" + id;
            return await this.CallApiPrivateAsync<List<ReservaUsuario>>(request, token);
        }

        public async Task<List<BibliotecaSimple>> GetBibliotecasEditables(string id, string token)
        {
            string request = "/api/Bibliotecas/GetBibliotecasEditables/" + id;
            return await this.CallApiPrivateAsync<List<BibliotecaSimple>>(request, token);
        }

        public async Task<List<BibliotecaSimple>> GetBibliotecasSimples()
        {
            string request = "/api/Bibliotecas/GetBibliotecasSimples";
            return await this.CallApiAsync<List<BibliotecaSimple>>(request);
        }

        public async Task DeleteReserva(int id, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Reservas/DeleteReserva/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task<Compartido> GetToken(string dni, string token)
        {
            Compartido res = new Compartido
            {
                DNI_USUARIO = dni,
                TOKEN = token
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Usuarios/SetGetToken";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(res);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);

                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Compartido usuario = JsonConvert.DeserializeObject<Compartido>(responseString);
                    return usuario;
                }
                else
                {
                    throw new Exception($"Error al intentar obtener usuario: {response.StatusCode}");
                }
            }
        }

        public async Task<List<LibroDeseo>> GetFavoritos(string id, string token)
        {
            string request = "/api/Usuarios/GetFavoritos/" + id;
            return await this.CallApiPrivateAsync<List<LibroDeseo>>(request, token);
        }

        public async Task<Compartido> GetShare(string id, string token)
        {
            string request = "/api/Usuarios/GetShare/" + id;
            return await this.CallApiPrivateAsync<Compartido>(request, token);
        }

        public async Task DeleteBiblioteca(int id, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Bibliotecas/DeleteBiblioteca/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task AddBiblio(string nombre, string direccion, int telefono, string web, TimeSpan hora_apertura, TimeSpan hora_cierre, string imagen, string token)
        {
            Biblioteca res = new Biblioteca
            {
                NOMBRE = nombre,
                DIRECCION = direccion,
                TELEFONO = telefono,
                WEB = web,
                HORA_APERTURA = hora_apertura,
                HORA_CIERRE = hora_cierre,
                IMAGEN = imagen,
                ID_BIBLIOTECA = 0
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Bibliotecas/AddBiblio";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task UpdateBiblio(int id, string nombre, string direccion, int telefono, string web, TimeSpan hora_apertura, TimeSpan hora_cierre, string imagen, string token)
        {
            Biblioteca res = new Biblioteca
            {
                NOMBRE = nombre,
                DIRECCION = direccion,
                TELEFONO = telefono,
                WEB = web,
                HORA_APERTURA = hora_apertura,
                HORA_CIERRE = hora_cierre,
                IMAGEN = imagen,
                ID_BIBLIOTECA = id
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Bibliotecas/UpdateBiblio";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, content);
            }
        }

        public async Task<List<LibroDefault>> GetLibrosTodos(string token)
        {
            string request = "/api/Libros";
            return await this.CallApiPrivateAsync<List<LibroDefault>>(request, token);
        }

        public async Task DeleteLibro(int id, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Libros/DeleteLibro/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task AddLibro(string nombre, int numpag, string imagen, string urlcompra, string descripcion, string idioma, DateTime fecha_publicacion, int idautor, string token)
        {
            LibroDefault res = new LibroDefault
            {
                NOMBRE = nombre,
                NUM_PAGINAS = numpag,
                IMAGEN = imagen,
                URL_COMPRA = urlcompra,
                DESCRIPCION = descripcion,
                IDIOMA = idioma,
                FECHA_PUBLICACION = fecha_publicacion,
                ID_AUTOR = idautor
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Libros/AddLibro";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task UpdateLibro(int id, string nombre, int numpag, string imagen, string urlcompra, string descripcion, string idioma, DateTime fecha_publicacion, int idautor, string token)
        {
            LibroDefault res = new LibroDefault
            {
                ID_LIBRO = id,
                NOMBRE = nombre,
                NUM_PAGINAS = numpag,
                IMAGEN = imagen,
                URL_COMPRA = urlcompra,
                DESCRIPCION = descripcion,
                IDIOMA = idioma,
                FECHA_PUBLICACION = fecha_publicacion,
                ID_AUTOR = idautor
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Libros/UpdateLibro";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PutAsync(request, content);
            }
        }

        public async Task DeleteAutor(int id, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Autores/DeleteAutor/" + id;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task AddAutor(string nombre, string nacionalidad, DateTime fechaNac, string imagen, string descripcion, int numLibros, string wiki, string token)
        {
            Autor res = new Autor
            {
                NOMBRE = nombre,
                NACIONALIDAD = nacionalidad,
                FECHA_NACIMIENTO = fechaNac,
                IMAGEN = imagen,
                HISTORIA = descripcion,
                NUM_LIBROS = numLibros,
                WIKI = wiki
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Autores/AddAutor";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task UpdateAutor(int id, string nombre, string nacionalidad, DateTime fechaNac, string imagen, string descripcion, int numLibros, string wiki, string token)
        {
            Autor res = new Autor
            {
                ID_AUTOR = id,
                NOMBRE = nombre,
                NACIONALIDAD = nacionalidad,
                FECHA_NACIMIENTO = fechaNac,
                IMAGEN = imagen,
                HISTORIA = descripcion,
                NUM_LIBROS = numLibros,
                WIKI = wiki
            };

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Autores/UpdateAutor";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string json = JsonConvert.SerializeObject(res);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
            }
        }

        public async Task<List<LibroDefault>> GetLibrosNotInBiblioteca(int id, string token)
        {
            string request = "/api/Libros/GetLibrosNotInBiblioteca/" + id;
            return await this.CallApiPrivateAsync<List<LibroDefault>>(request, token);
        }

        public async Task AddLibroBiblio(int idBiblio, int idLibro, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Libros/AddLibroBiblio/" + idBiblio + "/" + idLibro;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task DeleteLibroBiblio(int idBiblio, int idLibro, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Libros/DeleteLibroBiblio/" + idBiblio + "/" + idLibro;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await client.DeleteAsync(request);
            }
        }

        public async Task<List<ReservaNLibro>> GetReservasBiblio(int id, string token)
        {
            string request = "/api/Reservas/GetReservasBiblio/" + id;
            return await this.CallApiPrivateAsync<List<ReservaNLibro>>(request, token);
        }

        public async Task RecogerLibro(int idPrestamo, int idBiblio, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Reservas/RecogerLibro/" + idPrestamo + "/" + idBiblio;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }

        public async Task DevolverLibro(int idPrestamo, int idBiblio, string token)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Reservas/DevolverLibro/" + idPrestamo + "/" + idBiblio;
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
            }
        }



        //OTROS

        public List<string> GetDaysBetween(DateTime fecha_inicio, DateTime fecha_fin)
        {
            var days = new List<string>();
            for (DateTime date = fecha_inicio; date <= fecha_fin; date = date.AddDays(1))
            {
                days.Add(date.ToString("dd/MM/yyyy"));
            }
            return days;
        }

        public string GenerateToken()
        {
            var token = Guid.NewGuid().ToString("N") + new Random().Next(1000, 9999).ToString();
            return token;
        }




        //METODO PROTEGIDO
        //public async Task<List<Empleado>> GetEmpleadosAsync(string token)
        //{
        //    string request = "/api/empleados";
        //    List<Empleado> empleados =
        //        await this.CallApiAsync<List<Empleado>>(request, token);
        //    return empleados;
        //}

        ////METODO LIBRE
        //public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        //{
        //    string request = "/api/empleados/" + idempleado;
        //    Empleado empleado =
        //        await this.CallApiAsync<Empleado>(request);
        //    return empleado;
        //}
    }



}
