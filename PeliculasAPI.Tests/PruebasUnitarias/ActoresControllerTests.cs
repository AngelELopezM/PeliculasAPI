using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class ActoresControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerPersonasPaginadas()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            _context.Actores.Add(new Actor() { Nombre = "Actor 1" });
            _context.Actores.Add(new Actor() { Nombre = "Actor 2" });
            _context.Actores.Add(new Actor() { Nombre = "Actor 3" });

            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreBD);

            var controller = new ActoresController(_context2, _mapper, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina1 = await controller.Get(new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 2 });
            var actoresPagina1 = pagina1.Value;
            Assert.AreEqual(2, actoresPagina1.Count);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var pagina2 = await controller.Get(new PaginacionDTO() { Pagina = 2, CantidadRegistrosPorPagina = 2 });
            var actoresPagina2 = pagina2.Value;
            Assert.AreEqual(1, actoresPagina2.Count);
        }

        [TestMethod]
        public async Task CreacActorSinFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var actor = new ActorCreacionDTO() { Nombre = "Angel", FechaNacimiento = DateTime.Now };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(null, null, null, null))
                .Returns(Task.FromResult("URL"));

            var controller = new ActoresController(_context, _mapper, mock.Object);
            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.AreEqual(201, resultado.StatusCode);

            //Verificar si el actor ha sido realmente creado

            var _context2 = ConstruirContext(nombreBD);
            var listado = await _context2.Actores.ToListAsync();
            Assert.AreEqual(1, listado.Count);
            Assert.IsNull(listado[0].Foto);

            Assert.AreEqual(0, mock.Invocations.Count);
        }

        [TestMethod]
        public async Task CreacActorConFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var content = Encoding.UTF8.GetBytes("Imagen prueba");
            var archivo = new FormFile(new MemoryStream(content), 0, content.Length, "Data", "imagen.jpg");
            archivo.Headers = new HeaderDictionary();
            archivo.ContentType = "image/jpg";

            var actor = new ActorCreacionDTO()
            {
                Nombre = "nuevo actor",
                FechaNacimiento = DateTime.Now,
                Foto = archivo
            };

            var mock = new Mock<IAlmacenadorArchivos>();
            mock.Setup(x => x.GuardarArchivo(content, ".jpg", "fotosactores", archivo.ContentType))
                .Returns(Task.FromResult("URL"));

            var controller = new ActoresController(_context, _mapper, mock.Object);
            var respuesta = await controller.Post(actor);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.AreEqual(201, resultado.StatusCode);

        }

        [TestMethod]
        public async Task PatchRetorna404SiActorNoExiste()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var controller = new ActoresController(_context, _mapper, null);
            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            var respuesta = await controller.Patch(1, patchDoc);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task PatchActualizandoUnSoloCampo()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var _context = ConstruirContext(nombreBD);
            var _mapper = configurarAutoMapper();

            var fechaNacimiento = DateTime.Now;
            var actor = new Actor() { Nombre = "Angel", FechaNacimiento = fechaNacimiento };
            _context.Add(actor);
            await _context.SaveChangesAsync();

            var _context2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(_context2, _mapper, null);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));

            controller.ObjectValidator = objectValidator.Object;

            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            patchDoc.Operations.Add(new Operation<ActorPatchDTO>("replace", "/nombre", null, "Pedro"));

            var respuesta = await controller.Patch(1, patchDoc);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);
        }
    }

}
