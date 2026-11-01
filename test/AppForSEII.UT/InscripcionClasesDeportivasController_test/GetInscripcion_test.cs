
using AppForSEII.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs;

namespace AppForSEII.UT.InscripcionClasesDeportivasController_test
{//clase que contiene las pruebas del details
//hereda para obtener la base de datos SQLLITE
    public class GetInscripcion_test : AppForSEII4SqliteUT

    {
        //constructor se ejecuta antes de cada prueba
        //preparamos los datos que van a utilizar
        public GetInscripcion_test()
{
    // creamos el usuario que va a realizar la inscripcion
    ApplicationUser user = new ApplicationUser(
        "1",//id
        "Peter",//nombre
        "Jackson",//Apellidos
        "peter@uclm.es"//correo
    );

    // asignamos su DNI que nos lo devuelve el details
    user.DNI = "12345678Z";

    // añadimos el usuario al contexto de SQLLITE
    _context.ApplicationUsers.Add(user);
    // creamos el tipo de deporte baloncesto utilizando su constructor
var baloncesto = new TipoDeporte("Baloncesto");

// asignamos el nombre obligatorio del tipo de deporte
baloncesto.NombreTipoDeporte = "Baloncesto";

// creamos una clase deportiva asociada a baloncesto
var claseBaloncesto = new ClaseDeportiva(
    "Entrenamiento de baloncesto",      // descripcion
    baloncesto,                         // tipo de deporte
    new DateTime(2026, 11, 5, 18, 0, 0), // fecha 
    "Pabellon deportivo",               // lugar
    "Carlos",                           // monitor
    "Principiante",                     // nivel
    20,                                 // plazas disponibles
    8m                                  // precio por plaza
);

// añadimos el deporte y la clase a la base de datos de pruebas
_context.Add(baloncesto);
_context.Add(claseBaloncesto);
// creamos una inscripcion de peter 
var inscripcion = new Inscripcion(
    user,                                // usuario que se inscribe
    "Tarjeta terminada en 1234",         // datos del pago
    new DateTime(2026, 11, 1, 12, 0, 0), // fecha de la inscripcion
    MetodoPago.Tarjeta,                  // metodo de pago
    16m                                  // precio total: 2 plazas a 8 euros
);
// creamos la clase inscrita peter reserva 2 plazas de baloncesto
var claseInscrita = new ClaseInscrita(
    "Primera inscripcion", // observaciones
    2,                     // plazas reservadas
    16m                    // precio total: 2 plazas x 8 euros
)
{
    ClaseDeportiva = claseBaloncesto, // clase deportiva seleccionada
    Inscripcion = inscripcion        // inscripcion a la que pertenece
};

// añadimos la clase inscrita a la lista de la inscripcion
inscripcion.ClasesInscritas.Add(claseInscrita);
// añadimos la inscripcion al contexto junto con su clase inscrita
_context.Add(inscripcion);

// guardamos todos los datos de prueba en SQLLITE
_context.SaveChanges();
}
[Fact]//indica que es una prueba unitaria
[Trait("Database", "WithoutFixture")]//indicamos que utilizamos una bbdd sin fixture compartida
[Trait("LevelTesting", "Unit Testing")]//clasificamos esta prueba como Unit Testing
public async Task GetInscripcion_Found_test()
{
    // Arrange: preparamos el logger y el controlador
    var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();//creamos un objeto simulado del logger
    ILogger<InscripcionClasesDeportivasController> logger = mock.Object;//obtenemos el logger simulado para pasarselo al controlador

    var controller = new InscripcionClasesDeportivasController(_context, logger);//creamos el controlador usando la base SQLLITE y el logger simulado
    // creamos la lista de clases inscritas que esperamos recibir del GET
var clasesEsperadas = new List<InscripcionClaseDeportivaItemDTO>();

// creamos el ItemDTO con los datos de la clase de baloncesto
var claseEsperada = new InscripcionClaseDeportivaItemDTO(
    1,                                    // id de la clase deportiva
    "Entrenamiento de baloncesto",        // descripcion
    "Baloncesto",                         // tipo de deporte
    new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
    2,                                    // plazas reservadas
    16m,                                  // precio total
    "Primera inscripcion"                 // observaciones
);

// añadimos la clase a la lista esperada
clasesEsperadas.Add(claseEsperada);
// Creamos el DTO con los datos que esperamos recibir del GET
var expectedInscripcion = new InscripcionClaseDeportivaDetailDTO(
    1,                                    // id de la inscripcion
    "Peter",                              // nombre del cliente
    "Jackson",                            // apellidos
    "12345678Z",                          // DNI
    "peter@uclm.es",                      // correo electronico
    null,                                 // telefono, no hay ninguno por eso es null
    new DateTime(2026, 11, 1, 12, 0, 0), // fecha de la inscripcion
    16m,                                  // precio total
    MetodoPago.Tarjeta,                   // metodo de pago
    clasesEsperadas                       // lista de clases inscritas
);

    // Act: buscamos la inscripcion de Peter, que tiene el id 1
    var result = await controller.GetInscripcion(1);

    // Assert: comprobamos que devuelve 200 OK
    var okResult = Assert.IsType<OkObjectResult>(result);
    // Comprobamos que el resultado contiene un DTO de detalle de inscripcion
var inscripcionActual = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(okResult.Value);

// Comparamos el DTO esperado con el obtenido usando Equals
Assert.True(expectedInscripcion.Equals(inscripcionActual));
}

//not found
        [Fact]//indica que es una prueba unitaria
        [Trait("Database", "WithoutFixture")]//indicamos que utilizamos una bbdd sin fixture compartida
        [Trait("LevelTesting", "Unit Testing")]//clasificamos esta prueba como Unit Testing
        public async Task GetInscripcion_NotFound_test()
        {
            // Arrange: preparamos el controlador y sus dependencias
            var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();//creamos un objeto simulado del logger
            ILogger<InscripcionClasesDeportivasController> logger = mock.Object;//obtenemos el logger simulado para pasarselo al controlador

            var controller = new InscripcionClasesDeportivasController(_context, logger);//creamos el controlador que vamos a probar

            // Act: buscamos una inscripcion que no existe
            var result = await controller.GetInscripcion(0);

            // Assert: comprobamos que devuelve 404 NotFound
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
