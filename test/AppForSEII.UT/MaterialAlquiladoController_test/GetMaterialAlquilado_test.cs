using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.MaterialAlquiladoDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.MaterialAlquiladoController_test
{
    //pruebas unitarias del details
    public class GetMaterialAlquilado_test : AppForSEII4SqliteUT
    {
        private readonly DateTime _fechaAlquiler = new DateTime(2026, 11, 10, 10, 0, 0);

        // el constructor se ejecuta ANTES de cada prueba.
        // insertamos en la base de datos los datos que el método va a consultar.
        public GetMaterialAlquilado_test()
        {
            ApplicationUser user = new ApplicationUser("1", "Peter", "Jackson", "peter@uclm.es")
            {
                DNI = "12345678Z"
            };
            // Tipo de deporte y tipo de material a los que pertenece el material
            var padel = new TipoDeporte("Padel");
            var raqueta = new TipoMaterial(1, "Raqueta");

            // Material: (cantidad en stock, idMaterial, nombre, precio)
            // se le asignan sus relaciones con TipoDeporte y TipoMaterial
            var pala = new Material(10, 1, "Pala", 10m)
            {
                TipoDeporte = padel,
                TipoMaterial = raqueta
            };

            // Alquiler: (cliente, fecha, idAlquiler, lista de materiales, método de pago, precio total)
            // La lista de materiales empieza vacía y se rellena más abajo
            var alquiler = new Alquiler(user, _fechaAlquiler, 1,
                                        new List<MaterialAlquilado>(),
                                        MetodoPago.Tarjeta, 20);
            // El constructor de Alquiler uso DateTime.Now e ignora el parámetro de fecha,
            // por eso la asignamos aquí para que sea la fecha fija de la prueba
            alquiler.FechaAlquiler = _fechaAlquiler;

            // MaterialAlquilado (una línea del alquiler):
            // (cantidad, descripción, idMaterial, idAlquiler, precio de la línea)
            // 2 palas x 10 € = 20 €
            var materialAlquilado = new MaterialAlquilado(2, "Alquiler de dos palas de padel", 1, 1, 20m)
            {
                Material = pala,      // material al que se refiere la línea
                Alquiler = alquiler   // alquiler al que pertenece la línea
            };
            // anadimos la línea a la lista de materiales del alquiler
            alquiler.MaterialesAlquilados.Add(materialAlquilado);

            // anadimos todo al contexto de la BD de pruebas
            _context.ApplicationUsers.Add(user);
            _context.Add(padel);
            _context.Add(raqueta);
            _context.Add(pala);
            _context.Add(alquiler);

            // guardamos los datos
            _context.SaveChanges();
        }

        // PRUEBA 1: el alquiler existe y el método devuelve 200 OK con el DTO de detalle
        [Fact] // indica que es una prueba unitaria sin parámetros
        [Trait("Database", "WithoutFixture")]     // usa una BD propia, no compartida
        [Trait("LevelTesting", "Unit Testing")]   // clasifica la prueba como unitaria
        public async Task GetMaterialAlquilado_Found_test()
        {
            // logger simulado. El controller lo necesita en su constructor
            // porque lo usa dentro del método, pero aquí no nos interesa lo que registre.
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;

            // creamos el controller con la BD SQLite de pruebas y el logger simulado
            var controller = new AlquileresController(_context, logger);

            // lista de materiales que esperamos recibir en el DTO:
            // (nombreMaterial, tipoMaterial, tipoDeporte, cantidad, precio)
            var materialesEsperados = new List<MaterialAlquiladoItemDTOs>
            {
                new MaterialAlquiladoItemDTOs("Pala", "Raqueta", "Padel", 2, 20m)
            };

            // DTO completo que esperamos recibir. Debe coincidir con los datos
            // insertados en el constructor de la clase.
            // (idAlquiler, nombre, apellidos, dni, email, teléfono, fecha, precioTotal, métodoPago, materiales)
            // el teléfono es null porque no se lo asignamos al usuario.
            var expected = new MaterialAlquiladoDetailDTOs(
                1, "Peter", "Jackson", "12345678Z", "peter@uclm.es", null,
                _fechaAlquiler, 20, MetodoPago.Tarjeta, materialesEsperados);

            //ejecutamos el método que queremos probar con el id 1
            var result = await controller.GetAlquiler(1);


            // la respuesta debe ser un 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            // el contenido de la respuesta debe ser el DTO de details
            var actual = Assert.IsType<MaterialAlquiladoDetailDTOs>(okResult.Value);
            // el DTO obtenido debe ser igual al esperado (usa el Equals del DTO)
            Assert.Equal(expected, actual);
        }

        // PRUEBA 2: el alquiler no existe y el método devuelve 404 NotFound
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMaterialAlquilado_NotFound_test()
        {
            // logger simulado y controller, igual que en la prueba anterior
            var mock = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = mock.Object;
            var controller = new AlquileresController(_context, logger);

            // pedimos un alquiler con id 0, que no existe en la BD
            var result = await controller.GetAlquiler(0);

            // el resultado debe ser un 404 NotFound
            Assert.IsType<NotFoundResult>(result);
        }
    }
}