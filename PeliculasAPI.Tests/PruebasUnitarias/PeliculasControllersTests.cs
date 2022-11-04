using Microsoft.AspNetCore.Http;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class PeliculasControllersTests : BasePruebas
    {
        private string CrearDataPrueba()
        {
            var databaseName = Guid.NewGuid().ToString();
            var _context = ConstruirContext(databaseName);
            var genero = new Genero() { Nombre = "Genero 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){Titulo = "Pelicula 1", FechaEstreno = new DateTime(2010,1,1), EnCines = false},
                new Pelicula(){Titulo = "Pelicula 2", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false},
                new Pelicula(){Titulo = "Pelicula 3", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true}
            };

            var peliculaConGenero = new Pelicula()
            {
                Titulo = "Pelicula con genero",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };

            peliculas.Add(peliculaConGenero);

            _context.Add(genero);
            _context.AddRange(peliculas);
            _context.SaveChanges();

            var peliculaGenero = new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = peliculaConGenero.Id };
            _context.Add(peliculaGenero);
            _context.SaveChanges();

            return databaseName;
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreBD = CrearDataPrueba();
            var _mapper = configurarAutoMapper();
            var _context = ConstruirContext(nombreBD);

            var controller = new PeliculasController(_context, _mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var tituloPelicula = "Pelicula 1";

            var filtroDTO = new FiltroPeliculasDTO() { Titulo = tituloPelicula, CantidadRegistrosPorPagina = 5 };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula 1", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreBD = CrearDataPrueba();
            var _mapper = configurarAutoMapper();
            var _context = ConstruirContext(nombreBD);

            var controller = new PeliculasController(_context, _mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO() { EnCines = true };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula 3", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreBD = CrearDataPrueba();
            var _mapper = configurarAutoMapper();
            var _context = ConstruirContext(nombreBD);

            var controller = new PeliculasController(_context, _mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = _context.Generos.Select(x => x.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                GeneroId = generoId
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;
            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Pelicula con genero", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarOrdenaTituloAscendente()
        {
            var nombreBD = CrearDataPrueba();
            var _mapper = configurarAutoMapper();
            var _context = ConstruirContext(nombreBD);

            var controller = new PeliculasController(_context, _mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtroDTO);
            var peliculas = respuesta.Value;

            var _context2 = ConstruirContext(nombreBD);
            var peliculasDB = _context2.Peliculas.OrderBy(x => x.Titulo).ToList();

            Assert.AreEqual(peliculas.Count, peliculasDB.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = peliculas[i];
                var peliculaDB = peliculas[i];

                Assert.AreEqual(peliculaDB.Id, peliculaDelControlador.Id);
            }
        }
    }
}
