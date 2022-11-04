using NetTopologySuite;
using NetTopologySuite.Geometries;
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
    public class SalasDeCineControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCineA5KmOMenos()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using var _context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false);

            var salasDeCine = new List<SalaDeCine>()
            {
                new SalaDeCine
                {
                    Nombre = "Agora", Ubicacion = geometryFactory.CreatePoint(new Coordinate(-69.9388777, 18.4839233))
                }
             };
            _context.AddRange(salasDeCine);
            await _context.SaveChangesAsync();

            var filtroo = new SalaDeCineCercanoFiltroDTO()
            {
                DistannciaEnKms = 5,
                Latitud = 18.481139,
                Longitud = -69.938950
            };

            var mapper = configurarAutoMapper();
            var controller = new SalasDeCineController(_context, mapper);

            var respuesta = await controller.Cercanos(filtroo);
            var valor = respuesta.Value;
            Assert.AreEqual(2,valor.Count);
        }
    }
}
