using AppForSEII.API.Controllers; // Controlador que vamos a probar
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs; // DTOs del POST
using AppForSEII.API.Models; // Modelos de nuestra aplicacion
using Microsoft.AspNetCore.Mvc; // Respuestas HTTP
using Microsoft.Extensions.Logging; // Para utilizar ILogger
using Moq; // Para simular el logger
using Xunit; // Para utilizar Fact, Theory y Assert

namespace AppForSEII.UT.InscripcionClasesDeportivasController_test
{
    // Pruebas unitarias del metodo POST CreateInscripcion
    public class CreateInscripcion_test : AppForSEII4SqliteUT
    {
public CreateInscripcion_test()
{
    // Creamos un usuario que utilizaremos para realizar las inscripciones
    ApplicationUser user = new ApplicationUser(
        "1",              // identificador del usuario
        "Peter",          // nombre
        "Jackson",        // apellidos
        "peter@uclm.es"   // nombre de usuario
    );

    // Asignamos su DNI
    user.DNI = "12345678Z";

    // Añadimos el usuario a la base de datos SQLite de pruebas
    _context.ApplicationUsers.Add(user);
    // Creamos dos tipos de deporte
var baloncesto = new TipoDeporte("Baloncesto");
baloncesto.NombreTipoDeporte = "Baloncesto";

var tenis = new TipoDeporte("Tenis");
tenis.NombreTipoDeporte = "Tenis";

// Creamos una clase de baloncesto con 5 plazas disponibles
var claseBaloncesto = new ClaseDeportiva(
    "Entrenamiento de baloncesto",       // descripcion
    baloncesto,                          // tipo de deporte
    new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
    "Pabellon deportivo",                // lugar
    "Carlos",                            // monitor
    "Principiante",                      // nivel
    5,                                   // plazas disponibles
    8m                                   // precio por plaza
);

// Creamos una clase de tenis que ya no tiene plazas
var claseTenis = new ClaseDeportiva(
    "Clase de tenis completa",
    tenis,
    new DateTime(2026, 11, 6, 17, 0, 0),
    "Pistas deportivas",
    "Ana",
    "Intermedio",
    0,                                   // no quedan plazas
    12m
);

// Añadimos los deportes y las clases a SQLite
_context.AddRange(baloncesto, tenis, claseBaloncesto, claseTenis);

// Guardamos los datos de prueba
_context.SaveChanges();
}
// Metodo que prepara los casos de error para probar el POST
public static IEnumerable<object[]> TestCasesFor_CreateInscripcion()
{
    // Creamos un DTO sin ninguna clase deportiva seleccionada
    var carritoVacio = new InscripcionClaseDeportivaForCreateDTO(
        "peter@uclm.es",                          // usuario
        "Tarjeta terminada en 1234",              // datos del pago
        MetodoPago.Tarjeta,                       // metodo de pago
        new List<InscripcionClaseDeportivaItemDTO>() // lista vacia
    );

    // Guardamos los datos de entrada y el mensaje de error esperado
    var allTests = new List<object[]>
    {
        new object[] {
            carritoVacio,
            "Error, Debe incluir al menos una clase deportiva"
        }
    };
    // Creamos una inscripcion con un usuario que no existe en la base de datos
var usuarioInexistente = new InscripcionClaseDeportivaForCreateDTO(
    "usuarioinexistente@uclm.es", // correo de un usuario no registrado
    "Tarjeta terminada en 1234",  // datos del pago
    MetodoPago.Tarjeta,           // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        // Añadimos una clase para que el carrito no este vacio
        new InscripcionClaseDeportivaItemDTO(
            1,                                    // id de baloncesto
            "Entrenamiento de baloncesto",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            1,                                    // plazas reservadas
            8m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// Añadimos el segundo caso de error a nuestra lista
allTests.Add(new object[]
{
    usuarioInexistente,
    "Error, El usuario no está registrado"
});
// Creamos una inscripcion con una clase deportiva que no existe
var claseInexistente = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario que si existe
    "Tarjeta terminada en 1234", // datos de pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            999,                                  // id de una clase que no existe
            "Clase inexistente",                  // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            1,                                    // plazas reservadas
            8m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// Añadimos el nuevo caso y el mensaje que esperamos recibir
allTests.Add(new object[]
{
    claseInexistente,
    "Error, No existe la clase deportiva con id 999"
});
// Creamos una inscripcion para una clase que no tiene plazas disponibles
var plazasInsuficientes = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            2,                                    // id de la clase de tenis
            "Clase de tenis completa",            // descripcion
            "Tenis",                              // deporte
            new DateTime(2026, 11, 6, 17, 0, 0), // fecha y hora
            1,                                    // solicitamos una plaza
            12m,                                  // precio
            null                                  // sin observaciones
        )
    }
);

// Añadimos el caso de plazas insuficientes y el error esperado
allTests.Add(new object[]
{
    plazasInsuficientes,
    "Error, No hay suficientes plazas disponibles para la clase Clase de tenis completa"
});
// Creamos una inscripcion que intenta reservar 0 plazas
var plazasInvalidas = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos de pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            1,                                    // id de baloncesto
            "Entrenamiento de baloncesto",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            0,                                    // cantidad incorrecta de plazas
            0m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// Añadimos el caso de plazas invalidas y el error esperado
allTests.Add(new object[]
{
    plazasInvalidas,
    "Error, Debe reservar entre 1 y 3 plazas"
});
// Creamos una inscripcion que intenta reservar 4 plazas
var plazasSuperiores = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            1,                                    // id de baloncesto
            "Entrenamiento de baloncesto",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            4,                                    // supera el maximo de 3 plazas
            32m,                                  // precio de las 4 plazas
            null                                  // sin observaciones
        )
    }
);

// Añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    plazasSuperiores,
    "Error, Debe reservar entre 1 y 3 plazas"
});
// Creamos una inscripcion seleccionando dos veces la misma clase deportiva
var clasesDuplicadas = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        // Primera seleccion de baloncesto
        new InscripcionClaseDeportivaItemDTO(
            1, "Entrenamiento de baloncesto", "Baloncesto",
            new DateTime(2026, 11, 5, 18, 0, 0),
            1, 8m, null
        ),

        // Segunda seleccion de la misma clase (id 1)
        new InscripcionClaseDeportivaItemDTO(
            1, "Entrenamiento de baloncesto", "Baloncesto",
            new DateTime(2026, 11, 5, 18, 0, 0),
            1, 8m, null
        )
    }
);

// Añadimos el caso de clases duplicadas y su mensaje esperado
allTests.Add(new object[]
{
    clasesDuplicadas,
    "Error, No se puede seleccionar la misma clase deportiva dos veces"
});

// Creamos una inscripcion con un metodo de pago que no existe
var pagoInvalido = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    (MetodoPago)999,             // metodo de pago invalido
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            1,                                    // id de baloncesto
            "Entrenamiento de baloncesto",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            1,                                    // plazas reservadas
            8m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// Añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    pagoInvalido,
    "Error, El metodo de pago seleccionado no es valido"
});

    // Devolvemos los casos que utilizara nuestra prueba
    return allTests;
}

[Theory] // Indica que esta prueba se ejecutara con varios casos diferentes
[Trait("LevelTesting", "Unit Testing")] // Indicamos que es una prueba unitaria
[Trait("Database", "WithoutFixture")] // Utilizamos nuestra base de datos SQLite
[MemberData(nameof(TestCasesFor_CreateInscripcion))] // Obtenemos los casos de prueba del metodo anterior
public async Task CreateInscripcion_Error_test(
    InscripcionClaseDeportivaForCreateDTO inscripcionDTO, // Datos que enviamos al POST
    string errorExpected) // Mensaje de error que esperamos recibir
{
    // ARRANGE: preparamos el controlador

    // Creamos un logger simulado
    var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();

    // Obtenemos el logger del mock
    ILogger<InscripcionClasesDeportivasController> logger = mock.Object;

    // Creamos el controlador con SQLite y el logger
    var controller = new InscripcionClasesDeportivasController(_context, logger);

    // ACT: ejecutamos el metodo POST con los datos de prueba
    var result = await controller.CreateInscripcion(inscripcionDTO);

    // ASSERT: comprobamos la respuesta

    // Comprobamos que devuelve BadRequest (400)
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

    // Obtenemos los errores de validacion devueltos por el controlador
    var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

    // Obtenemos todos los mensajes de error de la respuesta
    var errores = problemDetails.Errors.SelectMany(e => e.Value);

    // Comprobamos que aparece el mensaje de error esperado
    Assert.Contains(errorExpected, errores);
    
}

[Fact] // esta prueba comprueba un caso de creacion correcta
[Trait("LevelTesting", "Unit Testing")]
[Trait("Database", "WithoutFixture")]
public async Task CreateInscripcion_Success_test()
{
    // ARRANGE: preparamos el controlador y los datos de la inscripcion

    // Creamos el logger simulado
    var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();
    ILogger<InscripcionClasesDeportivasController> logger = mock.Object;

    // Creamos el controlador con nuestra base de datos SQLite
    var controller = new InscripcionClasesDeportivasController(_context, logger);

    // Creamos la clase deportiva que Peter quiere reservar
    var claseSeleccionada = new InscripcionClaseDeportivaItemDTO(
        1,                                    // id de baloncesto
        "Entrenamiento de baloncesto",        // descripcion
        "Baloncesto",                         // deporte
        new DateTime(2026, 11, 5, 18, 0, 0), // fecha de la clase
        2,                                    // reservamos dos plazas
        16m,                                  // precio esperado
        "Primera inscripcion"                 // observaciones
    );

    // Creamos el DTO que enviaremos al POST
    var inscripcionDTO = new InscripcionClaseDeportivaForCreateDTO(
        "peter@uclm.es",
        "Tarjeta terminada en 1234",
        MetodoPago.Tarjeta,
        new List<InscripcionClaseDeportivaItemDTO>() { claseSeleccionada }
    );

    // ACT: ejecutamos el POST para crear la inscripcion
    var result = await controller.CreateInscripcion(inscripcionDTO);

    // ASSERT: comprobamos que devuelve 201 Created
    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    // Comprobamos que el POST devuelve un DTO con el detalle de la inscripcion
var inscripcionActual = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(createdResult.Value);

// Comprobamos que el precio total sea 16 euros (2 plazas a 8 euros)
Assert.Equal(16m, inscripcionActual.PrecioTotal);
// Creamos la lista de clases que esperamos recibir del POST
var clasesEsperadas = new List<InscripcionClaseDeportivaItemDTO>()
{
    new InscripcionClaseDeportivaItemDTO(
        1,                                    // id de baloncesto
        "Entrenamiento de baloncesto",        // descripcion
        "Baloncesto",                         // tipo de deporte
        new DateTime(2026, 11, 5, 18, 0, 0), // fecha de la clase
        2,                                    // plazas reservadas
        16m,                                  // precio calculado
        "Primera inscripcion"                 // observaciones
    )
};

// Creamos el DTO con los datos esperados de la inscripcion
var expectedInscripcion = new InscripcionClaseDeportivaDetailDTO(
    1,                                    // id de la inscripcion
    "Peter",                              // nombre
    "Jackson",                            // apellidos
    "12345678Z",                          // DNI
    "peter@uclm.es",                      // correo
    null,                                 // telefono
    inscripcionActual.FechaInscripcion,   // fecha generada por el servidor
    16m,                                  // precio total
    MetodoPago.Tarjeta,                   // metodo de pago
    clasesEsperadas                       // clases inscritas esperadas
);
// Comprobamos que los datos esperados coincidan con los que devuelve el POST
Assert.True(expectedInscripcion.Equals(inscripcionActual));
// Consultamos la clase deportiva directamente en SQLite
// AsNoTracking evita utilizar el objeto que ya teniamos en memoria
var claseActual = await _context.ClasesDeportivas
    .AsNoTracking()
    .FirstAsync(cd => cd.Id == 1);

// Comprobamos que quedan 3 plazas despues de reservar 2
Assert.Equal(3, claseActual.PlazasDisponibles);
}


    }
}