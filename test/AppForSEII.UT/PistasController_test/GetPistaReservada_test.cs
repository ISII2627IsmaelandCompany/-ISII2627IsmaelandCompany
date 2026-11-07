using AppForSEII.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AppForSEII.API.DTOs.PistaReservadaDTOs;

namespace AppForSEII.UT.PistaReservadaController_test
{//clase que contiene las pruebas del details
//hereda para obtener la base de datos SQLLITE


 public class GetPistaReservada_test : AppForSEII4SqliteUT
    {
        // Constructor: se ejecuta antes de cada prueba
        // Preparamos los datos que van a utilizar
    public GetPistaReservada_test()
    {
    // Creamos el usuario que realiza la reserva
        ApplicationUser user = new ApplicationUser(
                "1", //id del usuario
                "Peter",//nombre
                "Jackson",//apellidos
                "peter@uclm.es"//correo
        );


    // asigno su DNI e información para el applicationUser
        user.DNI = "12345678Z";
        user.Age = 25;//edad
        user.Sex = "Hombre";
        user.PhoneNumber = "600123456";//numero de telefono

    // añado el usuario al contexto de SQLLITE
    _context.ApplicationUsers.Add(user);

    // creao el tipo de deporte Tenis utilizando su constructor
    var tenis = new TipoDeporte("Tenis");
    // asigno el nombre obligatorio del tipo de deporte
    tenis.NombreTipoDeporte = "Tenis";

     _context.Add(tenis);//añadimos el deporte a la base de datos de pruebas


     //creo una pista de tenis: 1-NombrePista, 2-Precio, 3-NPersonas, 4-stock
       var pistaTenis = new Pista("Pista de tenis",20,4,10)
        {
        TipoDeporte = tenis//para asociar la pista con el tipo de deporte Tenis
        };
        _context.Add(pistaTenis);// la añado a la base de datos de pruebas


           // Creo una reserva de Peter. le pasamos el usuario, metodo de pago, fecha() de noviembre de 2026 a las 12:00) y 40 que es el precio
        var reserva = new Reserva(user,MetodoPago.Tarjeta,new DateTime(2026, 11, 1, 12, 0, 0),40m);      

         _context.Add(reserva);


    // Guardo los datos para que se generen los identificadores
    _context.SaveChanges();

        //creo la pista que va a ser reservada
        var pistaReservada = new PistaReservada(pistaTenis,reserva,2,40)//constructor de PistaReservada solo admite cuatro parámetros
        {
            Observaciones = "Reserva de tenis"
        };


            // Añado esta pista a las reservas
            reserva.PistasReservadas.Add(pistaReservada);

            _context.Add(pistaReservada);// la añado a la base de datos de pruebas

            _context.SaveChanges();
    }


[Fact]//indica que es una prueba unitaria
[Trait("Database", "WithoutFixture")]//indicamos que utilizamos una bbdd sin fixture compartida
[Trait("LevelTesting", "Unit Testing")]//clasificamos esta prueba como Unit Testing
public async Task GetPistaReservada_Found_test()
{
// Arrange: preparamos el logger y el controlador
var mock = new Mock<ILogger<PistaReservadaController>>();//creamos un objeto simulado del logger
ILogger<PistaReservadaController> logger = mock.Object;//obtenemos el logger simulado para pasarselo al controlador

var controller = new PistaReservadaController(_context, logger);//creamos el controlador usando la base SQLLITE y el logger simulado
// creamos la lista de pistas reservadas que esperamos recibir del GET
var pistasEsperadas = new List<PistaReservadaItemDTO>();


            // Creo el ItemDTO con los datos de la pista de tenis
            var pistaEsperada = new PistaReservadaItemDTO(
                1, //id pista
                "Pista de tenis",//NombrePista
                "Tenis",//TipoDeporte
                "Reserva de tenis",//Observaciones
                40,//Precio
                2//Cantidad
            );

       pistasEsperadas.Add(pistaEsperada); // añadimos la pista a la lista esperada       

        // Creos el DTO con los datos que esperamos recibir del GET
            var expectedPistaReservada = new PistaReservadaDetailDTO(
                1,  //ID de la reserva
                "Peter", //nombre del cliente
                "Jackson", //apellido del cliente
                "12345678Z", //dni del cliente
                "peter@uclm.es", //correo del cliente
                "600123456", //numero de telefono
                new DateTime(2026, 11, 1, 12, 0, 0),//fecha de la reserva
                40m, //precio de la reserva
                MetodoPago.Tarjeta, //metodo de pago
                pistasEsperadas //lista de pistas esperadas
            );
            
            // Act: buscamos la pista reservada con sus dos identificadores
            var result = await controller.GetPistaReservada(1); //el id de la reserva

            // Assert: comprobamos que devuelve 200 OK
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Comprobamos que contiene un DTO de detalle
            var pistaReservadaActual =Assert.IsType<PistaReservadaDetailDTO>(okResult.Value);

            // Comparamos el DTO esperado con el obtenido usando el equals
            Assert.True(expectedPistaReservada.Equals(pistaReservadaActual));
        
}


//not found
    [Fact]//indica que es una prueba unitaria
    [Trait("Database", "WithoutFixture")]//indicamos que utilizamos una bbdd sin fixture compartida
    [Trait("LevelTesting", "Unit Testing")]//clasificamos esta prueba como Unit Testing
    public async Task GetPistaReservada_NotFound_test()
    {
        // Arrange: preparamos el controlador y sus dependencias
            var mock = new Mock<ILogger<PistaReservadaController>>();//creamos un objeto simulado del logger
            ILogger<PistaReservadaController> logger = mock.Object;//obtenemos el logger simulado para pasarselo al controlador
            
            var controller = new PistaReservadaController(_context, logger);//creamos el controlador que vamos a probar

        // Act: buscamos una  reserva que no existe
            var result = await controller.GetPistaReservada(0);

        // Assert: comprobamos que devuelve 404 NotFound
            Assert.IsType<NotFoundResult>(result);
    }   
    
    }
}