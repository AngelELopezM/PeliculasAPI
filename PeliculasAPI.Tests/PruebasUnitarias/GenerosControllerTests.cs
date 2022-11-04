using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class GenerosControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenertodosLosGeneros()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            _context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            _context.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await _context.SaveChangesAsync();

            //Aqui creamos otro contexto porque en el 1er contexto ya estan los datos cargados
            //en memoria de esta manera nos aseguramos de que si "hacemos consulta a la BD"
            var _context2 = ConstruirContext(nombreBD);

            //Prueba
            var controller = new GenerosController(_context2, _mapper);
            var respuesta = await controller.Get();

            //Verificacion
            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var controller = new GenerosController(_context, _mapper);

            //Prueba
            var respuesta = await controller.Get(1);

            //Verificacion
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            _context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            _context.Generos.Add(new Genero() { Nombre = "Genero 2" });
            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreBD);

            //Prueba
            var controller = new GenerosController(_context2, _mapper);
            var id = 2;
            var respuesta = await controller.Get(id);

            //Verificacion
            var resulatado = respuesta.Value;
            Assert.AreEqual(id, resulatado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "Nuevo genero" };

            //Prueba
            var controller = new GenerosController(_context, _mapper);
            var respuesta = await controller.Post(nuevoGenero);

            //Verificacion
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            _context.Generos.Add(new Genero() { Nombre = "Genero 1" });
            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreBD);

            //Prueba
            var controller = new GenerosController(_context2, _mapper);
            var generoCreacionDTO = new GeneroCreacionDTO() { Nombre = "Este es el nuevoo" };

            var id = 1;
            var respuesta = await controller.Put(id, generoCreacionDTO);

            //verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var _context3 = ConstruirContext(nombreBD);
            var existe = await _context3.Generos.AnyAsync(x => x.Nombre == "Este es el nuevoo");
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroNoexistente()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            //Prueba
            var controller = new GenerosController(_context, _mapper);
            var respuesta = await controller.Delete(1);

            //Verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task IntentaBorrarGeneroExistente()
        {
            //Preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            _context.Generos.Add(new Genero() { Nombre = "Genero 1"});
            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreBD);

            //Prueba
            var controller = new GenerosController(_context2, _mapper);
            var id = 1;
            var respuesta = await controller.Delete(id);

            //Verificacion
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var _contexto3 = ConstruirContext(nombreBD);

            var vacia = await _contexto3.Generos.AnyAsync();
            Assert.IsFalse(vacia);
        }
    }
}
