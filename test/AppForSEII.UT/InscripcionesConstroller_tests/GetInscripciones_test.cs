using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AppForSEII.UT.Inscripciones_Test
{
    public class GetInscripciones_test : AppForSEII4SqliteUT//hereda como dice en el seminario
    {
        public GetInscripciones_test()//en este constructor creo la base de datos local
        {
            // Arrange: crear el cliente
            var cliente = new ApplicationUser(
                "1",
                "Ismael",
                "Garcia",
                "ismael@gmail.com"
            )
            {
                DNI = "12345678A",
                Age = 20,
                Sex = "Hombre"
            };

            // Arrange: crear la competición
            var competicion = new Competicion(
                new DateTime(2026, 11, 10),
                "Albacete",
                "Carrera Popular",
                100,
                15m
            );

            // Corregimos la fecha porque el constructor de Competicion
            // asigna DateTime.Now independientemente de la fecha recibida.
            competicion.Fecha = new DateTime(2026, 11, 10);

            // Guardar las entidades principales
            _context.Add(cliente);
            _context.Add(competicion);
            _context.SaveChanges();

            // Crear la inscripción
            var inscripcion = new Inscripcion(
                cliente,
                "Datos de pago",
                new DateTime(2026, 10, 30),
                MetodoPago.Tarjeta,
                15m
            );

            _context.Add(inscripcion);
            _context.SaveChanges();

            // Crear la relación entre inscripción y competición
            var competicionInscripcion = new CompeticionInscripcion(
                competicion.Id,
                inscripcion.Id,
                "Ninguno"
            )
            {
                Competicion = competicion,
                Inscripcion = inscripcion
            };
            //guardamos los datos
            _context.Add(competicionInscripcion);
            _context.SaveChanges();
        }
        [Fact]//indicamos que es una prueba no parametrizada
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetInscripcionOK_test()
        {
            //arrange
            var mock = new Mock<ILogger<InscripcionesController>>();//lo utilizamos para realizar la simulación de ese tipo
            ILogger<InscripcionesController> logger = mock.Object;//registro de mensajes

            var controller = new InscripcionesController(_context, logger);//creamos el controlador

            var competicionEsperada = new InscripcionItemDTO(//creamos la competicion esperada
                "Carrera Popular",
                "Albacete",
                new DateTime(2026, 11, 10),
                15m,
                "Ninguno"
            );

            var inscripcionDTOEsperada = new InscripcionDetailDTO(//creamos la inscripcion esperada
                1,
                "Ismael",
                "Garcia",
                "12345678A",
                "ismael@gmail.com",
                null,
                new DateTime(2026, 10, 30),
                15m,
                MetodoPago.Tarjeta,
                new List<InscripcionItemDTO> { competicionEsperada }
            );

            //act
            var result = await controller.GetInscripcion(1);//ejecutamos la operacion que queremos probar

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);//comprobamos que devulva 200
            var inscripcionOctual = Assert.IsType<InscripcionDetailDTO>(okResult.Value);//tipo de contenido
            Assert.Equal(inscripcionDTOEsperada, inscripcionOctual);//comparamos el resultado esperado con el actual
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetInscripcionNotFound_test()
        {
            // arrange, esta vez no hace falta crear dtos
            var mock = new Mock<ILogger<InscripcionesController>>();
            ILogger<InscripcionesController> logger = mock.Object;

            var controller = new InscripcionesController(_context, logger);

            // act
            var result = await controller.GetInscripcion(2);// es el que no va a salir, por lo que será not found porque no existe

            // assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}