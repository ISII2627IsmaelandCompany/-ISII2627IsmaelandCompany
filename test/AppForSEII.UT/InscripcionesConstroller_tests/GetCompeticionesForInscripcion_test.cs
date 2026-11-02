using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII.UT.Inscripciones_Test
{
    public class GetCompeticionesForInscripcion_test : AppForSEII4SqliteUT
    {
        public GetCompeticionesForInscripcion_test()
        {
            // Arrange -> crear una competición
            var competicion = new Competicion(
                new DateTime(2026, 11, 10),
                "Albacete",
                "Carrera Popular",
                100,
                15m
            );
            // Modificamos la fecha de la competición para que sea posterior a la fecha actual
            competicion.Fecha = new DateTime(2026, 11, 10);

            _context.Add(competicion);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompeticionesForInscripcion_OK_test()
        {
            // Arrange
            // Creamos un mock del logger para poder pasarlo al controlador
            var mock = new Mock<ILogger<InscripcionesController>>();
            // Creamos el controlador con el contexto y el logger
            ILogger<InscripcionesController> logger = mock.Object;
            // Creamos el controlador con el contexto y el logger
            var controller = new InscripcionesController(_context, logger);
            // Creamos un objeto CompeticionForInscripcionDTO con los datos esperados de la competición
            var expectedCompeticion = new CompeticionForInscripcionDTO(
                1,
                "Carrera Popular",
                "Albacete",
                new DateTime(2026, 11, 10),
                100,
                15m
            );
            
            // Act
            // Llamamos al método GetCompeticionesForInscripcion con todos los parámetros nulos para obtener todas las competiciones
            var result = await controller.GetCompeticionesForInscripcion(
                null, null, null, null);

            // Assert
            // Comprobamos que devuelva un resultado 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Comprobamos que el valor devuelto sea una lista de CompeticionForInscripcionDTO
            var competicionesActuales =
                Assert.IsType<List<CompeticionForInscripcionDTO>>(okResult.Value);
            // Comprobamos que la lista devuelta tenga un solo elemento
            Assert.Single(competicionesActuales);
            // Comprobamos que los datos de la competición devuelta sean correctos  
            Assert.Equal(expectedCompeticion, competicionesActuales[0]);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompeticionesForInscripcion_BadRequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;
            // Creamos el controlador con el contexto y el logger
            var controller = new InscripcionesController(_context, logger);

            // Act: fromDate es posterior a toDate
            // Llamamos al método GetCompeticionesForInscripcion con fromDate posterior a toDate para provocar un error de validación
            var result = await controller.GetCompeticionesForInscripcion(
                null,
                null,
                new DateTime(2026, 11, 20),
                new DateTime(2026, 11, 10));

            // Assert
            // Comprobamos que devuelva error 400
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // Comprobamos que el error devuelto sea de tipo ValidationProblemDetails
            var validationProblem =
                Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            // Comprobamos que el error corresponde al rango de fechas
            Assert.Contains("fromDate&toDate", validationProblem.Errors.Keys);
        }
    }
}