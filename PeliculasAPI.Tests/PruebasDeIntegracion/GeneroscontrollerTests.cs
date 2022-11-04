using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class GeneroscontrollerTests : BasePruebas
    {
        private static readonly string url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(0, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var _context = ConstruirContext(nombreDB);
            _context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            _context.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await _context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var _context = ConstruirContext(nombreDB);

            _context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await _context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");
            respuesta.EnsureSuccessStatusCode();

            var _context2 = ConstruirContext(nombreDB);
            var existe = await _context2.Generos.AnyAsync();

            Assert.IsFalse(existe);
        }
    }
}
