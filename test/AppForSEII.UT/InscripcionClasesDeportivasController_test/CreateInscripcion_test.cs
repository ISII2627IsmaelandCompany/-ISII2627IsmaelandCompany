using AppForSEII.API.Controllers; // controlador que vamos a probar
using AppForSEII.API.DTOs.InscripcionClaseDeportivaDTOs; // DTOs del POST
using AppForSEII.API.Models; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Logging; // para utilizar ILogger
using Moq; // para simular el logger
using Xunit; // para utilizar Fact, Theory y Assert

namespace AppForSEII.UT.InscripcionClasesDeportivasController_test//donde estan las pruebas
{
    // pruebas unitarias del metodo POST CreateInscripcion
    public class CreateInscripcion_test : AppForSEII4SqliteUT
    {
public CreateInscripcion_test()
{
    // creamos un usuario que utilizaremos para realizar las inscripciones
    ApplicationUser user = new ApplicationUser(
        "1",              // id
        "Peter",          // nombre
        "Jackson",        // apellidos
        "peter@uclm.es"   // nombre de usuario
    );

    // datos personales del usuario 
    user.DNI = "12345678Z";
    user.PhoneNumber = "600123456";
    user.Age = 30;
    user.Sex = "Hombre";
    // añadimos el usuario a la base de datos SQLLITE de pruebas
    _context.ApplicationUsers.Add(user);
    // creamos dos tipos de deporte
var baloncesto = new TipoDeporte("Baloncesto");
baloncesto.NombreTipoDeporte = "Baloncesto"; //nombre del deporte

var tenis = new TipoDeporte("Tenis");
tenis.NombreTipoDeporte = "Tenis";

// creamos una clase de baloncesto con 5 plazas disponibles
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

// creamos una clase de tenis que ya no tiene plazas
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

var claseBaloncesto2 = new ClaseDeportiva(
    "Entrenamiento de baloncesto Remota",       // descripcion
    baloncesto,                          // tipo de deporte
    new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
    "Pabellon deportivo",                // lugar
    "Carlos",                            // monitor
    "Principiante",                      // nivel
    5,                                   // plazas disponibles
    8m                                   // precio por plaza
);
var claseBaloncesto3 = new ClaseDeportiva(
    "Entrenamiento de baloncesto Online",       // descripcion
    baloncesto,                          // tipo de deporte
    new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
    "Pabellon deportivo",                // lugar
    "Carlos",                            // monitor
    "Principiante",                      // nivel
    5,                                   // plazas disponibles
    8m                                   // precio por plaza
);


// añadimos los deportes y las clases a SQLite
_context.AddRange(baloncesto, tenis, claseBaloncesto, claseTenis,claseBaloncesto2,claseBaloncesto3);

// guardamos los datos de prueba
_context.SaveChanges();
}
// metodo que prepara los casos de error para probar el POST
public static IEnumerable<object[]> TestCasesFor_CreateInscripcion()
{
    // creamos un DTO sin ninguna clase deportiva seleccionada
    var carritoVacio = new InscripcionClaseDeportivaForCreateDTO(
        "peter@uclm.es",                          // usuario
        "Tarjeta terminada en 1234",              // datos del pago
        MetodoPago.Tarjeta,                       // metodo de pago
        new List<InscripcionClaseDeportivaItemDTO>() // lista vacia
    );

    // guardamos los datos de entrada y el mensaje de error esperado
    var allTests = new List<object[]>
    {
        new object[] {
            carritoVacio,
            "Error, Debe incluir al menos una clase deportiva"
        }
    };
    // creamos una inscripcion con un usuario que no existe en la base de datos
var usuarioInexistente = new InscripcionClaseDeportivaForCreateDTO(
    "usuarioinexistente@uclm.es", // correo no registrado
    "Tarjeta terminada en 1234",  // datos del pago
    MetodoPago.Tarjeta,           // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        // añadimos una clase para que el carrito no este vacio
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

// añadimos el segundo caso de error a nuestra lista
allTests.Add(new object[]
{
    usuarioInexistente,
    "Error, El usuario no está registrado"
});
// creamos una inscripcion con una clase deportiva que no existe
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

// añadimos el nuevo caso y el mensaje que esperamos recibir
allTests.Add(new object[]
{
    claseInexistente,
    "Error, No existe la clase deportiva con id 999"
});
// creamos una inscripcion para una clase que no tiene plazas disponibles
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

// añadimos el caso de plazas insuficientes y el error esperado
allTests.Add(new object[]
{
    plazasInsuficientes,
    "Error, No hay suficientes plazas disponibles para la clase Clase de tenis completa"
});
// creamos una inscripcion que intenta reservar 0 plazas
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

// añadimos el caso de plazas invalidas y el error esperado
allTests.Add(new object[]
{
    plazasInvalidas,
    "Error, Debe reservar entre 1 y 3 plazas"
});
// creamos una inscripcion que intenta reservar 4 plazas
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

// añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    plazasSuperiores,
    "Error, Debe reservar entre 1 y 3 plazas"
});
// creamos una inscripcion seleccionando dos veces la misma clase deportiva
var clasesDuplicadas = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Tarjeta,          // metodo de pago
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        // primera seleccion de baloncesto
        new InscripcionClaseDeportivaItemDTO(
            1, "Entrenamiento de baloncesto", "Baloncesto",
            new DateTime(2026, 11, 5, 18, 0, 0),
            1, 8m, null
        ),

        // segunda seleccion de la misma clase (id 1 baloncesto)
        new InscripcionClaseDeportivaItemDTO(
            1, "Entrenamiento de baloncesto", "Baloncesto",
            new DateTime(2026, 11, 5, 18, 0, 0),
            1, 8m, null
        )
    }
);

// añadimos el caso de clases duplicadas y su mensaje esperado
allTests.Add(new object[]
{
    clasesDuplicadas,
    "Error, No se puede seleccionar la misma clase deportiva dos veces"
});

// creamos una inscripcion con un metodo de pago que no existe
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

// añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    pagoInvalido,
    "Error, El metodo de pago seleccionado no es valido"
});

// creamos una inscripcion con un metodo de pago que no existe
var descripcioninvalida = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Bizum,             // metodo de pago invalido
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            4,                                    // id de baloncesto
            "Entrenamiento de baloncesto Online",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            1,                                    // plazas reservadas
            8m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    descripcioninvalida,
    "Error: el tipo de clase deportiva seleccionada no esta soportando en el sistema"
});
var descripcioninvalida2 = new InscripcionClaseDeportivaForCreateDTO(
    "peter@uclm.es",             // usuario existente
    "Tarjeta terminada en 1234", // datos del pago
    MetodoPago.Bizum,             // metodo de pago invalido
    new List<InscripcionClaseDeportivaItemDTO>()
    {
        new InscripcionClaseDeportivaItemDTO(
            3,                                    // id de baloncesto
            "Entrenamiento de baloncesto Remota",        // descripcion
            "Baloncesto",                         // deporte
            new DateTime(2026, 11, 5, 18, 0, 0), // fecha y hora
            1,                                    // plazas reservadas
            8m,                                   // precio
            null                                  // sin observaciones
        )
    }
);

// añadimos el caso y el mensaje de error esperado
allTests.Add(new object[]
{
    descripcioninvalida2,
    "Error: el tipo de clase deportiva seleccionada no esta soportando en el sistema"
});




    // devolvemos los casos que utilizara nuestra prueba
    return allTests;
}

[Theory] //  esta prueba se ejecutara con varios casos diferentes
[Trait("LevelTesting", "Unit Testing")] // prueba unitaria
[Trait("Database", "WithoutFixture")] //  base de datos SQLLITE
[MemberData(nameof(TestCasesFor_CreateInscripcion))] // Obtenemos los casos de prueba del metodo anterior
public async Task CreateInscripcion_Error_test(
    InscripcionClaseDeportivaForCreateDTO inscripcionDTO, // datos que enviamos al POST
    string errorExpected) // mensaje de error que esperamos recibir
{
    // ARRANGE: preparamos el controlador

    // creamos un logger simulado
    var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();

    // obtenemos el logger del mock
    ILogger<InscripcionClasesDeportivasController> logger = mock.Object;

    // creamos el controlador con SQLite y el logger
    var controller = new InscripcionClasesDeportivasController(_context, logger);

    // ACT: ejecutamos el metodo POST con los datos de prueba

// ejecutamos el POST
var result = await controller.CreateInscripcion(inscripcionDTO);

    // ASSERT: comprobamos la respuesta

    // comprobamos que devuelve BadRequest (400)
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

    // obtenemos los errores de validacion devueltos por el controlador
    var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

    // obtenemos todos los mensajes de error de la respuesta
    var errores = problemDetails.Errors.SelectMany(e => e.Value);

    // comprobamos que aparece el mensaje de error esperado
    Assert.Contains(errorExpected, errores);
    
}

[Fact] // esta prueba comprueba un caso de creacion correcta
[Trait("LevelTesting", "Unit Testing")]
[Trait("Database", "WithoutFixture")]
public async Task CreateInscripcion_Success_test()
{
    // ARRANGE: preparamos el controlador y los datos de la inscripcion

    // creamos el logger simulado
    var mock = new Mock<ILogger<InscripcionClasesDeportivasController>>();
    ILogger<InscripcionClasesDeportivasController> logger = mock.Object;

    // creamos el controlador con nuestra base de datos SQLLITE 
    var controller = new InscripcionClasesDeportivasController(_context, logger);

    // creamos la clase deportiva que Peter quiere reservar
    var claseSeleccionada = new InscripcionClaseDeportivaItemDTO(
        1,                                    // id de baloncesto
        "Entrenamiento de baloncesto",        // descripcion
        "Baloncesto",                         // deporte
        new DateTime(2026, 11, 5, 18, 0, 0), // fecha de la clase
        2,                                    // reservamos dos plazas
        16m,                                  // precio esperado
        "Primera inscripcion"                 // observaciones
    );

    // creamos el DTO que enviaremos al POST
    var inscripcionDTO = new InscripcionClaseDeportivaForCreateDTO(
        "peter@uclm.es",
        "Tarjeta terminada en 1234",
        MetodoPago.Tarjeta,
        new List<InscripcionClaseDeportivaItemDTO>() { claseSeleccionada }
    );

    // ACT: ejecutamos el POST para crear la inscripcion
// guardamos la fecha antes de crear la inscripcion
var fechaAntes = DateTime.Now;

// ejecutamos el POST
var result = await controller.CreateInscripcion(inscripcionDTO);

// guardamos la fecha despues de crear la inscripcion
var fechaDespues = DateTime.Now;
    // ASSERT: comprobamos que devuelve 201 Created
    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    // comprobamos que el POST devuelve un DTO con el detalle de la inscripcion
var inscripcionActual = Assert.IsType<InscripcionClaseDeportivaDetailDTO>(createdResult.Value);

// comprobamos que el precio total sea 16 euros (2 plazas a 8 euros)
Assert.Equal(16m, inscripcionActual.PrecioTotal);
// creamos la lista de clases que esperamos recibir del POST
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

// creamos el DTO con los datos esperados de la inscripcion
var expectedInscripcion = new InscripcionClaseDeportivaDetailDTO(
    1,                                    // id de la inscripcion
    "Peter",                              // nombre
    "Jackson",                            // apellidos
    "12345678Z",                          // DNI
    "peter@uclm.es",                      // correo
    "600123456",                                 // telefono
    inscripcionActual.FechaInscripcion,   // fecha generada por el servidor
    16m,                                  // precio total
    MetodoPago.Tarjeta,                   // metodo de pago
    clasesEsperadas                       // clases inscritas esperadas
);
// comprobamos que los datos esperados coincidan con los que devuelve el POST
Assert.True(expectedInscripcion.Equals(inscripcionActual));
// consultamos la clase deportiva directamente en SQLLITE
// asNoTracking evita utilizar el objeto que ya teniamos en memoria
var claseActual = await _context.ClasesDeportivas
    .AsNoTracking()
    .FirstAsync(cd => cd.Id == 1);

// comprobamos que quedan 3 plazas despues de reservar 2
Assert.Equal(3, claseActual.PlazasDisponibles);
// buscamos en SQLLITE la inscripcion que acaba de crear el POST
var inscripcionGuardada = await _context.Set<Inscripcion>()
    .AsNoTracking()
    .FirstOrDefaultAsync(i => i.Id == inscripcionActual.Id);

// comprobamos que la inscripcion existe en la base de datos
Assert.NotNull(inscripcionGuardada);
// comprobamos que la fecha de inscripcion esta entre el inicio y el final de la ejecucion
Assert.InRange(inscripcionActual.FechaInscripcion, fechaAntes, fechaDespues);

}


    }
}