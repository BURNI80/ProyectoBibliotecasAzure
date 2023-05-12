using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ProyectoBibliotecas.Models;
using NuguetProyectoBibliotecas.Models;

namespace ProyectoBibliotecas.Services
{
    public class ServiceApiBibliotecas
    {

        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiEmpleados;

        public ServiceApiBibliotecas(IConfiguration configuration)
        {
            this.UrlApiEmpleados = configuration.GetValue<string>("ConnectionStrings:ApiBibliotecas");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
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
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
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
                client.BaseAddress = new Uri(this.UrlApiEmpleados);
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
            string request = "/api/Libros/GetLibrosDispoBiblioteca/" + id + "/" + input + "/" + option;
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
