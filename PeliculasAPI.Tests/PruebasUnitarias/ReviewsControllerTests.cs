using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class ReviewsControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task UsuarioNoPuedeCrearDosReviewsParaLaMismaPelicula()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = _context.Peliculas.Select(x => x.Id).First();
            var review1 = new Review()
            {
                PeliculaId = peliculaId,
                UsuarioId = usuarioPorDefectoId,
                Puntuacion = 5
            };

            _context.Add(review1);
            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreDB);
            var _mapper = configurarAutoMapper();

            var controller = new ReviewController(_context2, _mapper);

            controller.ControllerContext = ConstruirControllerContext();

            var revewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5};

            var respuesta = await controller.Post(peliculaId, revewCreacionDTO);
            var valorRespuesta = respuesta as IStatusCodeActionResult;
            Assert.AreEqual(400, valorRespuesta.StatusCode);
        }

        [TestMethod]
        public async Task UsuarioPuedeCrearReviews()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreDB);
            CrearPeliculas(nombreDB);

            var peliculaId = _context.Peliculas.Select(x => x.Id).First();
            

            var _context2 = ConstruirContext(nombreDB);
            var _mapper = configurarAutoMapper();

            var controller = new ReviewController(_context2, _mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var revewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5 };

            var respuesta = await controller.Post(peliculaId, revewCreacionDTO);
            var valorRespuesta = respuesta as NoContentResult;
            Assert.IsNotNull(valorRespuesta);
        }

        private void CrearPeliculas(string nombreDB)
        {
            var _context = ConstruirContext(nombreDB);

            _context.Peliculas.Add(new Pelicula() { Titulo = "Pelicula 1" });
            _context.SaveChanges();
        }
    }
}
