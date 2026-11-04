using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.PistaDTOs;
using AppForSEII.API.Models;

namespace AppForSEII.UT.PistasController_test;
    public class GetPistas_test: AppForSEII4SqliteUT{
    
        
        //constructor para crear  la base de  datos local
        public GetPistas_test(){
        
            //creamos los tipos de deporte que usaremos
          var tiposDeporte=new List<TipoDeporte>(){
          new TipoDeporte("Baloncesto"){NombreTipoDeporte="Baloncesto"}, //posicion 0
          new TipoDeporte("Futbol"){NombreTipoDeporte="Futbol"},//pos 1
          new TipoDeporte("Tenis"){NombreTipoDeporte="Tenis"}//pos 2
          };

            //creo las pistas para las pruebas del select. Cada pista tiene: Nombre, precio, numero de personas max y stock
            var pistas = new List<Pista>()
            {
                new Pista("Pista de baloncesto 1", 12.00, 10, 1) { TipoDeporte = tiposDeporte[0] },//id 1, stock 1
                new Pista("Pista de futbol 1", 15.00, 22, 2) { TipoDeporte = tiposDeporte[1] },//id 2, stock 2
                new Pista("Pista de tenis 1", 10.00, 4, 1) { TipoDeporte = tiposDeporte[2] },//id 3, stock 1
                new Pista("Pista de tenis sin stock", 10.00, 4, 0) { TipoDeporte = tiposDeporte[2] }//id 4, stock 0 ->el select no debria devolverla nunca
            };

            //creo el usuario que realizara las reservas
            var usuario = new ApplicationUser()
            {
                Id = "1",
                UserName = "usuario1",
                Email = "usuario1@gmail.com",
                DNI = "12345678A",
                Age = 25,
                Sex = "Hombre"
            };

            //creo las reservas 
            var reservas = new List<Reserva>()
            {                         //el date time: año, mes, dia, hora, minutos y segundos. El 12 es el precio
                new Reserva(usuario, MetodoPago.Tarjeta, new DateTime(2026, 11, 10, 10, 0, 0), 12.00m),

                new Reserva(usuario, MetodoPago.Efectivo, new DateTime(2026,11, 11, 10, 0, 0), 15.00m),

                new Reserva()//es lo mismo
                {
                    Cliente = usuario,
                    MetodoPago = MetodoPago.Bizum,
                    FechaReserva = new DateTime(2026, 11, 10, 12, 0, 0),
                    PrecioTotal = 10.00m
                }                
            };

           

            // Guardo los datos en la base de datos de prueba  (SQLLITE )
            _context.AddRange(tiposDeporte);
            _context.AddRange(pistas);
            _context.Add(usuario);
            _context.AddRange(reservas);
            _context.SaveChanges();


             //creo las pistas reservadas dejajo para que reciban los identificadores reales (y no valores 0 que pdrian tener)
            var pistasReservadas = new List<PistaReservada>()
            {//El constructor de PistaReservada: PistaReservada(Pista pista, Reserva reserva, int cantidad, double precio)
                new PistaReservada(pistas[0], reservas[0], 1, 12.00),//pista baloncesto reservada el 10 de noviembre, cantidad 1, precio 12
                new PistaReservada(pistas[1], reservas[1], 1, 15.00),//pista futbol reservada el 11 de noviembre, cantidad 1, precio 15
                new PistaReservada(pistas[2], reservas[2], 1, 10.00)//pista tenis reservada el 10 de noviembre, cantidad 1, precio 10
            };
            _context.AddRange(pistasReservadas);
            _context.SaveChanges();
        }


//metodo que prepara los casos de prueba correctos del Select
//es statico por tanto xUnit puede usarlo
//IEnumerable<object?[]> permite devolver varios arrays con distintos tipos de datos
//cada array un caso de prueba
        
public static IEnumerable<object?[]> TestCasesFor_GetPistas_OK()
{
// Creamos los DTOs que esperamos recibir del select
//esto no se guarda en la SQLLITE son los resultados esperados
    var pistasDTOs = new List<PistaDTO>()
    {
        new PistaDTO(1, "Pista de baloncesto 1", 10, 12.00, 1, "Baloncesto"),
        new PistaDTO(2, "Pista de futbol 1", 22, 15.00, 2, "Futbol"),
        new PistaDTO(3, "Pista de tenis 1", 4, 10.00, 1, "Tenis")    
    };


    // caso 1 ->sin filtros, buscamos todas las pistas disponibles 
    var pistasDTOsTC1 = new List<PistaDTO>()
    {pistasDTOs[0], pistasDTOs[1], pistasDTOs[2]};//baloncesto futbol y tenis , ordenadas por la  fecha

    // caso 2 ->filtro por deporte(tenis )
    var pistasDTOsTC2 = new List<PistaDTO>()
    { pistasDTOs[2] };// solo debe aparecer la unica pista de tenis con stock
    
    // caso 3 filtro por fecha, el 10 de noviembre
    var pistasDTOsTC3 = new List<PistaDTO>()
    { pistasDTOs[1] };// fútbol no tiene reserva el día 10, las otras ya están reservadas

    // caso 4 filtro por fecha, el 11 de noviembre
    var pistasDTOsTC4 = new List<PistaDTO>()
    { pistasDTOs[0], pistasDTOs[1], pistasDTOs[2] };//baloncesto no tiene reserva el 11, fultbol si tiene reserva pero su strok =2, tenis no tiene reserva el 11  y la de tenis sin stock no se devuelve nunca

// caso 5 voy a filtrar por un intervalo de fechas, del 10 al 11 de noviembre
    var pistasDTOsTC5 = new List<PistaDTO>()
    { pistasDTOs[1] };
    /*
El controlador comprueba cuántas reservas tiene cada pista entre el día 10 y el 11. La pista debe estar disponible todo el intervalo
Baloncesto: tiene  reserva el día 10. su stock es 1, no está disponible para todo el intervalo.
Fútbol: tiene  reserva el día 11.  su stock es 2, todavía queda una plaza disponible.
Tenis: tiene  reserva el día 10.  su stock es 1, no está disponible para todo el intervalo.
*/
    // guardamos los filtros y los resultados esperados
    var allTests = new List<object?[]>
    {
         //cada array tiene 4 posiciones. 1-tipoDeporte, 2-from, 3-to y 4 -lista de DTOs esperados
        new object?[] { null, null, null, pistasDTOsTC1 },//Todas las pistas disponibles
        new object?[] { "Tenis", null, null, pistasDTOsTC2 },//filtro por tenis
        new object?[] { null, new DateTime(2026,11,10), new DateTime(2026,11,10), pistasDTOsTC3 },// Pistas disponibles el día 10
        new object?[] { null, new DateTime(2026,11,11), new DateTime(2026,11,11), pistasDTOsTC4 },// Pistas disponibles el día 11
        new object?[] { null, new DateTime(2026,11,10), new DateTime(2026,11,11), pistasDTOsTC5 }// Pistas disponibles del 10 al 11
    };
    
    return allTests;//devolvemos los cincos casos y estos se usaran para ejecutar  el Theory

}   


[Theory]//indica que vamos a ejecutar varios casos en el mismo metodo. es método que ejecutará los cinco casos de prueba.
//MemberData obtiene los datos del metodo que hemos creado anteriormente
//nameof indica el nombre del metodo
//se ejecutaran 5 pruebas
[MemberData(nameof(TestCasesFor_GetPistas_OK))]
//etiqueta para identificar que usamos base de datos sin fixture
[Trait("Database", "WithoutFixture")]
[Trait("LevelTesting", "Unit Testing")]//etiqueta para clasificar estas pruebas como unitarias

//metodo que comprueba que el select devuelve los resultados correctos
//es asincrono por lo tanto podemos usar await
// task representa la operacion asincrona    
public async Task GetPistas_OK_test(
    string? tipoDeporte,//filtro por deporte, puede ser null
     DateTime? fromDate, // filtro por fecha, puede ser null
      DateTime? toDate, //filtro por facha, puede ser null
    IList<PistaDTO> expectedPistas) //clases esperadas
{
    // Arrange: preparamos el controlador para hacer la prueba
    //mock es para simular  una dependencia sin usar un logger real
    var mock = new Mock<ILogger<PistasController>>();
    //obtenemos el objeto  logger  simulado del mock
    //se lo pasamos al contructor del controlador
    ILogger<PistasController> logger = mock.Object;
    //creamos una instancia real del controlador que queremos probar
    //_context tiene las pistas guardadas en el SQLLite
    //logger objeto simulado que acabamos de crear
    var controller = new PistasController(_context, logger);
    
    
    // Act: ejecutamos el metodo SELECT , usando filtros que recibe cada caso
    //await espera a que termine la consulta a la base de datos
    //result guardara la respuesta que devuelve el controlador
    var result = await controller.GetPistas(tipoDeporte, fromDate, toDate);
    

    // Assert: comprobamos que devuelve OK y las pistas esperadas
    //comprobamos que devuelve un objeto  tipo OkObjetctResult
    //el 200 ok
    //si devuelve otra cosa , la prueba falla
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    //Obtenemos el contenido de la respuesta mediante value
    //comprobamos que wsea una lista de PistaDTO
    //guardamos en pistasActuales
    //si no son iguales la prueba falla
    var pistasActuales = Assert.IsType<List<PistaDTO>>(okResult.Value);
    //comparamos las pistas
    // comprobamos que las dos listas tienen el mismo numero de pistas
    Assert.Equal(expectedPistas, pistasActuales);
}


//metodo que prepara los casos que deben devolver badrequest
public static IEnumerable<object?[]> TestCasesFor_GetPistas_BadRequest()
{
    // creamos una lista con dos casos de error
    //cada array contiene tres filtros: deporte, fecha inicial y fecha final
    //no necesitamos dtos esperados porque buscamos una respuesta de error
    var allTests = new List<object?[]>
    {
        //caso 1 deporte que no existe  en nuestra SQLLITE
        // deberia devolver bad request
        new object?[] {  "Padel", null, null },

        // Caso 2: la fecha inicial es posterior a la fecha final
         // Debería devolver BadRequest
        new object?[] { null, new DateTime(2026, 11, 12), new DateTime(2026, 11, 10)
         }
    };

    return allTests;//devolvemos los dos casos de error para usarlos en el Theory
}    


[Theory]//ejecutaremos varios casos
//memberdata obtiene los dos casos  de error preparaodsen el metodo anterior
[MemberData(nameof(TestCasesFor_GetPistas_BadRequest))]
[Trait("LevelTesting", "Unit Testing")]//etiqueta  para  indicar  que son pruebas unitarias
[Trait("Database", "WithoutFixture")]//etiqueta para decir que usamos base de datos sin fixture

//metodo que comprueba cuando el select no encuentra clases disponibles
//recibe los tres filtros de cada caso de prueba
public async Task GetPistas_BadRequest_test(
    string? tipoDeporte,//deporte que queremos buscar
    DateTime? fromDate,//fecha inicio que queremos buscar
    DateTime? toDate)//fecha final que queremos buscar

{

    // Arrange: preparamos el controlador 
    //creamos un mock para simular el loggerdel controlador
    var mock = new Mock<ILogger<PistasController>>();
    //obtenemos el logger simulado  del mock
    ILogger<PistasController> logger = mock.Object;
    //creamos el controladorcon nuestra base SQLLite y el logger
    var controller = new PistasController(_context, logger);


   // Act: buscamos pistas con los filtros de cada caso
   //llamamos al controlador buscando las pistas con los filtros indicados
   //result tiene la respuesta por el controlador
    var result = await controller.GetPistas(tipoDeporte, fromDate, toDate);



    // Assert: comprobamos que devuelve BadRequest
    //debe dar  badrequest
    //si devuelve otra cosa error
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

    //comprobamos el mensaje de error validationsproblemdetails
    var mensaje = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
    //comparamos el mensaje recibido con el que esperamos debe coincidir exactamente con  el mensaje del controlador
    //si es distinto error
    Assert.NotNull(mensaje.Errors);
    Assert.NotEmpty(mensaje.Errors);
    }



}