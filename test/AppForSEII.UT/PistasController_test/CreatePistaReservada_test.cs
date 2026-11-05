using AppForSEII.API.Controllers; // controlador que vamos a probar
using AppForSEII.API.DTOs.PistaReservadaDTOs; // DTOs del POST
using AppForSEII.API.Models; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Extensions.Logging; // para utilizar ILogger
using Moq; // para simular el logger
using Xunit; // para utilizar Fact, Theory y Assert

/*
para esta prueba prepararo un usuario, una pista y un tipo de deporte en SQLite.
pero no una pista reservada previa, porque el POST comprueba que 
esa pista no esté reservada para el día actual
*/
namespace AppForSEII.UT.PistaReservadaController_test//donde estan las pruebas
{

    // pruebas unitarias del metodo POST CreatePistaReservada_test 
    public class CreatePistaReservada_test  : AppForSEII4SqliteUT
    {
        public CreatePistaReservada_test()
        {
        // Creamos un usuario que utilizaremos para realizar la reserva.
        ApplicationUser user = new ApplicationUser("1", "Peter", "Jackson", "peter@uclm.es");
                                                //1-ID del usuario, 2-Nombre, 3-Apellido, 4-Nommbre de usuario(username)


        //Asigno los datos del usuario
        user.DNI =  "12345678Z"; //dni
        user.Age = 25;//edad
        user.Sex = "Hombre";
        user.PhoneNumber = "600123456";//numero de telefono

           
        _context.ApplicationUsers.Add(user);// Para añadir al usuario a la base de datos.

        // Creo el tipo de deporte Tenis.
        TipoDeporte tenis = new TipoDeporte("Tenis");
        tenis.NombreTipoDeporte = "Tenis";

        _context.Add(tenis);// Añado el tipo de deporte a la base de datos.

        // Creo una pista de tenis: Precio: 20 euros, Número de personas: 4,  Stock: 10.
        Pista pistaTenis = new Pista("Pista de tenis", 20, 4, 10);


        
        pistaTenis.TipoDeporte = tenis;// Relacionar la pista con el tipo de deporte Tenis

        
        _context.Add(pistaTenis);// Añadir la pista a la base de datos.

        // Guardar todos los datos de prueba en la base de datos.
        _context.SaveChanges();

        }

// Preparo los casos en los que el POST tiene que devolver un error.
// Cada caso contiene 2 apartados.
//  -Los datos de la reserva que vamos a enviar.
//  -El mensaje de error que esperamos recibir.

public static IEnumerable<object[]> TestCasesFor_CreatePistaReservada()// metodo que prepara los casos de error para probar el POST

{

//1.     
   //Creamos un DTO sin ninguna pista seleccionda por si alguien intenta reservar sin poner la pista
    var carritoVacio = new PistaReservadaForCreateDTO(
    "peter@uclm.es",    // usuario
    MetodoPago.Tarjeta, // metodo de pago
    new List<PistaReservadaItemDTO>()// lista vacia
    );

    // guardamos los datos de entrada y el mensaje de error esperado
    var allTests = new List<object[]>
    {
    new object[] {carritoVacio,"Error, Debe incluir al menos una pista"}
    };

//2.
    // Creo una reserva con un usuario que no existe en la base de datos.
    // estoy creando un usuario q no existe y le estoy poniendo que tiene una pista reservada
//Porque queriero comprobar que el controlador detecta que el usuario no está registrado.
//el carrito tiene que tener datos válidos, salvo el usuario que es lo que quiero probar.
  var usuarioInexistente = new PistaReservadaForCreateDTO(
    "usuarioinexistente@uclm.es",            // correo no registrado (no existe en la base de datos)
    MetodoPago.Tarjeta,                      // metodo de pago
    new List<PistaReservadaItemDTO>()
    {
        // Añadimos una pista para que el carrito no este vacio
        new PistaReservadaItemDTO(
            1,                  // id de la pista
            "Pista de tenis",   // nombre
            "Tenis",            // deporte
            null,               // no hay observaciones
            20,                 // precio
            2                   // cantidad
        )
    }
  );

    // Añado el caso de: usuario Inexistente a la lista y ek mensaje
    allTests.Add(new object[]{usuarioInexistente,"Error, El usuario no está registrado"});


//3. 
    // Creamos una reserva con una pista que no existe
    var pistaInexistente = new PistaReservadaForCreateDTO(
        "peter@uclm.es",    // usuario que si existe
        MetodoPago.Tarjeta,    // metodo de pago
        new List<PistaReservadaItemDTO>()
        {

            new PistaReservadaItemDTO(
                999,                // id de una pista que no existe
                "Pista que no existe",// nombre
                "Tenis",            // deporte
                null,               // No hay observaciones
                20,                 // precio
                2                   // cantidad
            )
        }
    );
    // Añado el caso de: pista insexistente y el mensaje de error esperado
    allTests.Add(new object[]{pistaInexistente,"Error,no existe la pista con id 999 "});

//4.  
// Creamos una reserva para una pista con una cantidad de personas no valida
    var cantidadInvalida = new PistaReservadaForCreateDTO(
        "peter@uclm.es",             // usuario que existe
        MetodoPago.Tarjeta,          // metodo de pago
        new List<PistaReservadaItemDTO>()
        {
            new PistaReservadaItemDTO(
                1,                  // id de la pista de tenis
                "Pista de tenis",   // nombre
                "Tenis",            // deporte
                null,               // sin observaciones
                20,                 // precio
                0                   // cantidad incorrecta
            )
        }
    );

    // Añado el caso de: cantidad invalida y el error esperado
    allTests.Add(new object[]{cantidadInvalida,"Error, Debe reservar para al menos 1 persona"});

//5. 
// Creamos una reserva que supera el aforo de la pista
    var plazasInsuficientes = new PistaReservadaForCreateDTO(
        "peter@uclm.es",             // usuario que existe
        MetodoPago.Tarjeta,          // metodo de pago
        new List<PistaReservadaItemDTO>()
        {
            new PistaReservadaItemDTO(
                1,                  // id de la pista de tenis
                "Pista de tenis",   // nombre
                "Tenis",            // deporte
                null,               // sin observaciones
                20,                 // precio
                7                   // supera el aforo de 4 personas
            )
        }
    );

    // Añado el caso de: plazas insuficientes, y el error esperado
    allTests.Add(new object[]{plazasInsuficientes,"Error, no hay suficientes plazas en la pista Pista de tenis "});

//6.
    // Creo una reserva seleccionando dos veces la misma pista(no permitimos pistas duplicadas)
    var pistasDuplicadas = new PistaReservadaForCreateDTO(
        "peter@uclm.es",             // usuario que existe
        MetodoPago.Tarjeta,          // metodo de pago
        new List<PistaReservadaItemDTO>()
        {
            // Primera reserva de la pista de tenis
            new PistaReservadaItemDTO(
                1, "Pista de tenis", "Tenis", null, 20, 2
            ),

            // Segunda reserva de la pista de tenis
            new PistaReservadaItemDTO(
                1, "Pista de tenis", "Tenis", null, 20, 2
            )
        }
    );

    // Añado el caso de :  pistas duplicadas, y el mensaje esperado
    allTests.Add(new object[]{pistasDuplicadas,"Error, No se puede seleccionar la misma pista dos veces"});

//7. 
    // Creo una reserva y añado un metodo de pago que no existe
    var pagoInvalido = new PistaReservadaForCreateDTO(
        "peter@uclm.es",             // usuario qu eexiste
        (MetodoPago)999,             // metodo de pago que no existe
        new List<PistaReservadaItemDTO>()
        {
            new PistaReservadaItemDTO(
                1,                  // id de la pista
                "Pista de tenis",   // nombre
                "Tenis",            // deporte
                null,               // sin observaciones
                20,                 // precio
                2                   // cantidad
            )
        }
    );
    // Añado el caso: del pago invalido, y el mensaje de error esperado
    allTests.Add(new object[]{pagoInvalido,"Error, El metodo de pago seleccionado no es valido"});


  // DEVOLVER TODOS LOS CASOS QUE VA A UTILIZAR LA PRUEBA////
    return allTests;
}


[Theory] //  esta prueba se ejecutara con varios casos diferentes. se ejecutará varias veces, una por cada caso preparado.
[Trait("LevelTesting", "Unit Testing")] // prueba unitaria
[Trait("Database", "WithoutFixture")] //  base de datos SQLLITE
[MemberData(nameof(TestCasesFor_CreatePistaReservada))] // Obtenemos los casos de prueba del metodo anterior(arrange)

public async Task CreatePistaReservada_Error_test(
PistaReservadaForCreateDTO  reservaDTO, // datos que enviamos al POST
string errorExpected) // mensaje de error que esperamos recibir
{
    
   // ARRANGE: preparamos el controlador
    // creamos un logger simulado
    var mock = new Mock<ILogger<PistaReservadaController>>();
    // obtenemos el logger del mock
    ILogger<PistaReservadaController> logger = mock.Object;
    // creamos el controlador con SQLite y el logger
    var controller = new PistaReservadaController(_context, logger);


   // ACT: ejecutamos el metodo POST con los datos de prueba
    // ejecutamos el POST
    var result = await controller.CreatePistaReservada(reservaDTO);

   // ASSERT: comprobamos la respuesta
    // comprobamos que devuelve un BadRequest (400)
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

    // para obtener los errores de validacion devueltos por el controlador
    var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

    // obtenemos todos los mensajes de error de la respuesta
    var errores = problemDetails.Errors.SelectMany(e => e.Value);

    // comprobamos que aparece el mensaje de error esperado
    Assert.Contains(errorExpected, errores);

    }
    
[Fact] // esta prueba comprueba un caso de creacion correcta, es va a usar varias pruebas en este metodo
[Trait("LevelTesting", "Unit Testing")]
[Trait("Database", "WithoutFixture")]

public async Task CreatePistaReservada_Success_test()
{
  // ARRANGE: preparamos el controlador y los datos de la inscripcion
    // creamos el logger simulado
    var mock = new Mock<ILogger<PistaReservadaController>>();
    ILogger<PistaReservadaController> logger = mock.Object;

    // creamos el controlador con nuestra base de datos SQLLITE 
    var controller = new PistaReservadaController(_context, logger);

    // Creamos la pista que Peter quiere reservar
    var pistaSeleccionada = new PistaReservadaItemDTO(
        1,                  // id de la pista
        "Pista de tenis",   // nombre de la pista
        "Tenis",            // tipo de deporte
        "Primera reserva",  // observaciones
        20,                 // precio
        2                   // cantidad de personas
    );

    // Creamos el DTO que vamos a enviar al POST
    var reservaDTO = new PistaReservadaForCreateDTO(
        "peter@uclm.es", // usuario
        MetodoPago.Tarjeta, // metodo de pago
        new List<PistaReservadaItemDTO>() { pistaSeleccionada }
    );

    
  // ACT: ejecutamos el POST para crear la reserva
    // guardamos la fecha antes de crear la reserva
    var fechaAntes = DateTime.Now;

    // ejecutamos el POST
    var result = await controller.CreatePistaReservada(reservaDTO);

    // guardamos la fecha despues de crear la reserva
    var fechaDespues = DateTime.Now;


  // ASSERT: comprobamos que devuelve 201 Created
    var createdResult = Assert.IsType<CreatedAtActionResult>(result);
    // comprobamos que el POST devuelve un DTO con el detalle de la reserva
    var reservaActual = Assert.IsType<PistaReservadaDetailDTO>(createdResult.Value);

    
    // comprobamos que el precio total sea de 20 euros
    // El controlador utiliza el precio guardado en la base de datos
    Assert.Equal(20m, reservaActual.PrecioTotal);

    // creo la lista de clases que esperamos recibir del POST
       var pistasEsperadas = new List<PistaReservadaItemDTO>()
    {
        new PistaReservadaItemDTO(
            1,                  // El id de la pista
            "Pista de tenis",   // nombre de la pista
            "Tenis",            // tipo de deporte
            "Primera reserva",  // observaciones
            20,                 // precio
            2                   // cantidad de personas
        )
    };

    // Creamos el DTO con los datos esperados de la reserva
    var expectedReserva = new PistaReservadaDetailDTO(
        1,     // id de la reserva
        "Peter",         // nombre
        "Jackson",      // apellidos
        "12345678Z",     // DNI
        "peter@uclm.es",  // correo
        "600123456",     // telefono
        reservaActual.FechaReserva,   // fecha generada por el servidor
        20m,                // precio total
        MetodoPago.Tarjeta,    // metodo de pago
        pistasEsperadas     // pistas reservadas esperadas
    );

    // Comprobamos que los datos esperados coinciden con los que devuelve el POST
    Assert.True(expectedReserva.Equals(reservaActual));

    // Buscamos la reserva guardada directamente en SQLite
    // AsNoTracking evita utilizar el objeto que ya tenemos en memoria
    var reservaGuardada = await _context.Set<Reserva>()
        .AsNoTracking()
        .FirstAsync(r => r.Id == reservaActual.Id);

    // Comprobamos que la reserva existe en la base de datos
    Assert.NotNull(reservaGuardada);

    // Comprobamos que la fecha de la reserva esta entre el inicio y el final de la ejecucion
    Assert.InRange(reservaActual.FechaReserva, fechaAntes, fechaDespues);
    }


//caso 8 intentar reservar una pista que ya está reservada hoy
    // para esste caso necesito tener una reserva previa en la base de datos
[Fact] // prueba aparte: necesita una reserva previa guardada, por eso no va en el Theory de errores
[Trait("LevelTesting", "Unit Testing")]
[Trait("Database", "WithoutFixture")]
public async Task CreatePistaReservada_PistaYaReservadaHoy_test()
{
  // ARRANGE:  preparamos todo lo que necesita la prueba
       // creamos el logger simulado
    var mock = new Mock<ILogger<PistaReservadaController>>();
    ILogger<PistaReservadaController> logger = mock.Object;
    // creamos el controlador con nuestra base de datos SQLLITE 
    var controller = new PistaReservadaController(_context, logger);
    
    //preparo y dejo guardada una reserva de hoy para la pista 1
    var user = _context.ApplicationUsers.First(u => u.UserName == "peter@uclm.es"); //  // Busca en la base de datos al usuario Peter que ya se creó en el constructor de la clase. First() devuelve el primero que cumple la condicion
    var pista = _context.Pistas.First(p => p.IdPista == 1);// Busca la pista de tenis del constructor que tiene id 1
    var reservaPrevia = new Reserva(user, MetodoPago.Efectivo, DateTime.Now, 20m);// Crea la reserva anterior de Peter, con la fecha de hoy. los Parametros: cliente, metodo de pago, fecha de la reserva, precio total
     // Le añadimos una pista reservada a la reserva de arriba: la pista de tenis, 2 personas, precio 20.
    // El constructor de PistaReservada tiene (pista, reserva, cantidad, precio)
    // Observaciones no esta en el constructor  por eso la pongo con llaves { }
    reservaPrevia.PistasReservadas.Add(new PistaReservada(pista, reservaPrevia, 2, 20) { Observaciones = "Reserva previa" });
    _context.Add(reservaPrevia);// Para añadir la reserva a la base de datos
    _context.SaveChanges();//guardar los cambios



    // CREO  el DTO (lo que el cliente enviara) una reserva para esa misma pista
    //La peticion es valida (usuario existe, pista existe, cantidad 2 <  4(MAX))
    //el unico error posible es que la pista ya esta reservada hoy
    var reservaDTO = new PistaReservadaForCreateDTO("peter@uclm.es", MetodoPago.Tarjeta,
    new List<PistaReservadaItemDTO>() { new PistaReservadaItemDTO(1, "Pista de tenis", "Tenis", null, 20, 2) });


  // ACT: ejecutamos el POST (lo que queremos probar)
    // Llamamos al POST como si lo hiciera el cliente. await espera a que termine
    var result = await controller.CreatePistaReservada(reservaDTO);

  // ASSERT:comprobar que el resultado es el esperado
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);//Comprobamos que el controller devuelve un 400 BadRequest. si devuelve otra cosa, la prueba  falla
    var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);//Sacar el contenido del 400: la lista de errores (ValidationProblemDetails). problemDetails.Errors es un diccionario
    Assert.Contains("Error, la pista Pista de tenis ya está reservada para hoy",
        problemDetails.Errors.SelectMany(e => e.Value));//junta todos los mensajes de todas las claves en una sola lista. y el assert.contais comprueba que este nuestro mensaje
}
        }
}