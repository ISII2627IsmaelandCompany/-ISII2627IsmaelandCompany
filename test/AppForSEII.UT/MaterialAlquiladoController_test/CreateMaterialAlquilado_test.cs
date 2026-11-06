using AppForSEII.API.Controllers;                       // controlador que vamos a probar
using AppForSEII.API.DTOs.MaterialAlquiladoDTOs;        // DTOs del POST y del detalle
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;                     // para utilizar ILogger
using Moq;                                              // para simular el logger
using Xunit;                                            // Fact, Theory y Assert

namespace AppForSEII.UT.MaterialAlquiladoController_test
{
    // pruebas unitarias del metodo POST CreateAlquiler
    public class CreateMaterialAlquilado_test : AppForSEII4SqliteUT
    {
        public CreateMaterialAlquilado_test()
        {
            // usuario que realizara los alquileres
            ApplicationUser user = new ApplicationUser("1", "Peter", "Jackson", "peter@uclm.es");
            user.DNI = "12345678Z";
            user.Email = "peter@uclm.es"; // el controlador devuelve user.Email en el detalle
            _context.ApplicationUsers.Add(user);

            // tipos de deporte
            var baloncesto = new TipoDeporte("Baloncesto");
            var tenis = new TipoDeporte("Tenis");

            // tipos de material
            var balones = new TipoMaterial(1, "Balones");
            var raquetas = new TipoMaterial(2, "Raquetas");

            // material 1: balon de baloncesto, 10 unidades a 15 euros
            var balon = new Material(10, 1, "Balon de baloncesto", 15m)
            {
                TipoDeporte = baloncesto,
                TipoMaterial = balones
            };

            // material 2: raqueta de tenis, solo 2 unidades a 20 euros
            var raqueta = new Material(2, 2, "Raqueta de tenis", 20m)
            {
                TipoDeporte = tenis,
                TipoMaterial = raquetas
            };

            _context.AddRange(baloncesto, tenis, balones, raquetas, balon, raqueta);
            _context.SaveChanges();
        }

        // DTO valido por defecto, cada caso de error cambia solo UN dato
        private static MaterialAlquiladoForCreateDTO CrearDTO(
            string clienteUserName = "peter@uclm.es",
            string dni = "12345678Z",
            string nombre = "Peter",
            string apellidos = "Jackson",
            string telefono = "600123456",
            DateTime? fecha = null,
            MetodoPago metodoPago = MetodoPago.Tarjeta,
            IList<MaterialAlquiladoItemForCreateDTO>? materiales = null)
        {
            return new MaterialAlquiladoForCreateDTO(
                clienteUserName,
                dni,
                nombre,
                apellidos,
                telefono,
                fecha ?? DateTime.Today.AddDays(1),
                metodoPago,
                "Entrenamiento del equipo",
                materiales ?? new List<MaterialAlquiladoItemForCreateDTO>()
                {
                    new MaterialAlquiladoItemForCreateDTO(1, 1)
                });
        }

        // casos de error, DTO de entrada y el mensaje
        public static IEnumerable<object[]> TestCasesFor_CreateAlquiler()
        {
            var allTests = new List<object[]>
            {
                // sin materiales
                new object[] {
                    CrearDTO(materiales: new List<MaterialAlquiladoItemForCreateDTO>()),
                    "Error, Debe seleccionar al menos un material"
                },

                // mismo material dos veces
                new object[] {
                    CrearDTO(materiales: new List<MaterialAlquiladoItemForCreateDTO>()
                    {
                        new MaterialAlquiladoItemForCreateDTO(1, 1),
                        new MaterialAlquiladoItemForCreateDTO(1, 2)
                    }),
                    "Error, No se puede seleccionar el mismo material dos veces"
                },

                // fecha anterior a hoy
                new object[] {
                    CrearDTO(fecha: DateTime.Today.AddDays(-1)),
                    "Error, La fecha del alquiler no puede ser anterior a la fecha actual"
                },

                // metodo de pago inexistente
                new object[] {
                    CrearDTO(metodoPago: (MetodoPago)999),
                    "Error, El método de pago seleccionado no es válido"
                },

                // usuario no registrado
                new object[] {
                    CrearDTO(clienteUserName: "usuarioinexistente@uclm.es"),
                    "Error, El usuario no está registrado"
                },

                // datos obligatorios vacios
                new object[] { CrearDTO(dni: ""),        "Error, El DNI es obligatorio" },
                new object[] { CrearDTO(nombre: "   "),  "Error, El nombre es obligatorio" },
                new object[] { CrearDTO(apellidos: ""),  "Error, Los apellidos son obligatorios" },
                new object[] { CrearDTO(telefono: ""),   "Error, El teléfono de contacto es obligatorio" },

                // material que no existe
                new object[] {
                    CrearDTO(materiales: new List<MaterialAlquiladoItemForCreateDTO>()
                    {
                        new MaterialAlquiladoItemForCreateDTO(999, 1)
                    }),
                    "Error, No existe el material con id 999"
                },

                // cantidad menor que 1
                new object[] {
                    CrearDTO(materiales: new List<MaterialAlquiladoItemForCreateDTO>()
                    {
                        new MaterialAlquiladoItemForCreateDTO(1, 0)
                    }),
                    "Error, La cantidad de Balon de baloncesto debe ser al menos 1"
                },

                // mas unidades de las que hay en stock (raqueta: 2 en stock, pedimos 3)
                new object[] {
                    CrearDTO(materiales: new List<MaterialAlquiladoItemForCreateDTO>()
                    {
                        new MaterialAlquiladoItemForCreateDTO(2, 3)
                    }),
                    "Error, No hay suficientes unidades de Raqueta de tenis"
                }
            };

            return allTests;
        }

        [Theory] // se ejecuta una vez por cada caso
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateAlquiler))]
        public async Task CreateAlquiler_Error_test(
            MaterialAlquiladoForCreateDTO alquilerDTO,
            string errorExpected)
        {
            // arrange, preparar el controller
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            // act, ejecuta el metodo con los filtros del caso
            var result = await controller.CreateAlquiler(alquilerDTO);

            //assert, comprobar el resultado, 400 BadRequest con ValidationProblemDetails
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // buscamos el mensaje esperado entre todos los errores devueltos
            var errores = problemDetails.Errors.SelectMany(e => e.Value);
            Assert.Contains(errorExpected, errores);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateAlquiler_Success_test()
        {
            // arrange, preparar el controller
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            // 2 balones (2 x 15 = 30) + 1 raqueta (1 x 20 = 20) => 50 euros
            var alquilerDTO = new MaterialAlquiladoForCreateDTO(
                "peter@uclm.es",
                "12345678Z",
                "Peter",
                "Jackson",
                "600123456",
                DateTime.Today.AddDays(1),
                MetodoPago.Tarjeta,
                "Entrenamiento del equipo",
                new List<MaterialAlquiladoItemForCreateDTO>()
                {
                    new MaterialAlquiladoItemForCreateDTO(1, 2),
                    new MaterialAlquiladoItemForCreateDTO(2, 1)
                });

            // act, ejecuta el metodo con los filtros del caso
            var fechaAntes = DateTime.Now;
            var result = await controller.CreateAlquiler(alquilerDTO);
            var fechaDespues = DateTime.Now;

            // //assert, comprobar el resultado, 201 Created con el detalle del alquiler
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetAlquiler", createdResult.ActionName);
            var alquilerActual = Assert.IsType<MaterialAlquiladoDetailDTOs>(createdResult.Value);

            // precio total calculado en el servidor
            Assert.Equal(50, alquilerActual.PrecioTotal);

            // detalle que esperamos recibir
            var itemsEsperados = new List<MaterialAlquiladoItemDTOs>()
            {
                new MaterialAlquiladoItemDTOs("Balon de baloncesto", "Balones", "Baloncesto", 2, 30m),
                new MaterialAlquiladoItemDTOs("Raqueta de tenis", "Raquetas", "Tenis", 1, 20m)
            };

            var alquilerEsperado = new MaterialAlquiladoDetailDTOs(
                1,                              // id del alquiler
                "Peter",
                "Jackson",
                "12345678Z",
                "peter@uclm.es",
                "600123456",
                alquilerActual.FechaAlquiler,   // fecha generada
                50,
                MetodoPago.Tarjeta,
                itemsEsperados);

            Assert.True(alquilerEsperado.Equals(alquilerActual));

            // el stock se ha descontado en la BBDD    el AsNoTracking lee de SQLite, no de memoria
            var balonActual = await _context.Materials.AsNoTracking().FirstAsync(m => m.IdMaterial == 1);
            var raquetaActual = await _context.Materials.AsNoTracking().FirstAsync(m => m.IdMaterial == 2);
            Assert.Equal(8, balonActual.Cantidad);   // 10 - 2
            Assert.Equal(1, raquetaActual.Cantidad); // 2 - 1

            // el alquiler existe en la BBDD
            var alquilerGuardado = await _context.Alquileres
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAlquiler == alquilerActual.IdAlquiler);
            Assert.NotNull(alquilerGuardado);

            // la fecha esta entre el inicio y el final de la ejecucion
            // el constructor de Alquiler asigna DateTime.Now
            Assert.InRange(alquilerActual.FechaAlquiler, fechaAntes, fechaDespues);
        }
    }
}