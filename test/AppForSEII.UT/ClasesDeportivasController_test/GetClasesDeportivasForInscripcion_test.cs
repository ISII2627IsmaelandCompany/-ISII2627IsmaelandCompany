using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.ClaseDeportivaDTOs;

namespace AppForSEII.UT.ClasesDeportivasController_test
{
    public class GetClasesDeportivasForInscripcion_test : AppForSEII4SqliteUT
    {
        //constructorpara crear  la base de  datos local
        public GetClasesDeportivasForInscripcion_test(){
        //creamos los tipos de deporte que usaremos
        var tiposDeporte=new List<TipoDeporte>()
        {
          new TipoDeporte("Baloncesto"), //posicion 0
          new TipoDeporte("Futbol"),//posicion 1
          new TipoDeporte("Tenis")//posicion 2
        };
        //creamos las clases deportivas para las pruebas del select
        //cada clase tiene:
        //descripcion
        //deporte
        //fecha 
        //lugar
        //monitor
        //nivel
        //plazs disponibles 
        //precio
        var clases = new List<ClaseDeportiva>(){
              new  ClaseDeportiva("Entrenamiento de baloncesto",//descripcion
              tiposDeporte[0],//baloncesto
              new DateTime (2026,11,5,18,0,0),//fecha
              "Pista 1",//lugar
              "Carlos",//monitor
              "Iniciacion",//nivel
              20,//20 plazas
              8.00m //precio unitario
              ),
              new ClaseDeportiva("Entrenamiento de futbol",
              tiposDeporte[1],//futbol
              new DateTime(2026,11,6,19,0,0),
              "Pista 2",
              "Laura",
              "Intermedio",
              15,//15 plazas
              10.00m
              ),
              new ClaseDeportiva("Clase de tenis",
              tiposDeporte[2],//tenis
              new DateTime(2026,11,7,17,30,0),
              null,//Lugar es opcional
              "Miguel",
              "Avanzado",
              10,//10 plazas
              12.50m
              ),
              new ClaseDeportiva("Clase de tenis completa",//clase sin plazas para comprobar que no lo devuelva el select
              tiposDeporte[2],//tenis
              new DateTime(2026, 11, 8, 18, 0, 0),
            "Pista 3", 
            "Miguel", 
            "Intermedio",
             0,//sin plazas
              9.50m
              )  
            };
             // Guardamos los datos en la base de datos de prueba  (SQLLITE )
    _context.AddRange(tiposDeporte);//los tipos de deporte
    _context.AddRange(clases);//las clases
    _context.SaveChanges();//y guardamos ambos

        }

//metodo que prepara los casos de prueba correctos del Select
//es statico por tanto xUnit puede usarlo
//IEnumerable<object?[]> permite devolver varios arrays con distintos tipos de datos
//cada array un caso de prueba
public static IEnumerable<object?[]> TestCasesFor_GetClasesDeportivasForInscripcion_OK()
{
    // Creamos los DTOs que esperamos recibir del select
    //esto no se guarda en la SQLLITE son los resultados esperados
    var clasesDTOs = new List<ClaseDeportivaForInscripcionDTO>()
    {//primero espera que salga baloncesto
        new ClaseDeportivaForInscripcionDTO(1, "Entrenamiento de baloncesto",
            "Baloncesto", new DateTime(2026,11,5,18,0,0),
            "Pista 1", "Carlos", "Iniciacion", 20, 8.00m),
//segundo espera que salga futbol
        new ClaseDeportivaForInscripcionDTO(2, "Entrenamiento de futbol",
            "Futbol", new DateTime(2026,11,6,19,0,0),
            "Pista 2", "Laura", "Intermedio", 15, 10.00m),
//tercero la clase de tenis pero con plazas
        new ClaseDeportivaForInscripcionDTO(3, "Clase de tenis",
            "Tenis", new DateTime(2026,11,7,17,30,0),
            null, "Miguel", "Avanzado", 10, 12.50m)
    //y no espera  la de sin plazas porque no  debe devolverla
    };

    // caso 1 sin filtros, devuelve todas las clases con plazas
    var clasesDTOsTC1 = new List<ClaseDeportivaForInscripcionDTO>()
    { clasesDTOs[0], clasesDTOs[1], clasesDTOs[2] };//baloncesto futbol y tenis , ordenadas por la  fecha

    // caso 2 filtramos por deporte Tenis
    var clasesDTOsTC2 = new List<ClaseDeportivaForInscripcionDTO>()
    { clasesDTOs[2] };//solo debe aparecer  la unica  clase de tenis con plazas

    // caso 3 filtramos por monitor Carlos
    var clasesDTOsTC3 = new List<ClaseDeportivaForInscripcionDTO>()
    { clasesDTOs[0] };//el unico monitor con ese  nombre es el de baloncesto

    // caso 4 filtramos por fecha , concretamente el 6 de noviembre  (futbol)
    var clasesDTOsTC4 = new List<ClaseDeportivaForInscripcionDTO>()
    { clasesDTOs[1] };

    // guardamos los filtros y los resultados esperados
    var allTests = new List<object?[]>
    {//object?[] permite guardar valores de diferentes tipos y valores null
    //cada  array es un caso y tiene 4 posiciones  1 tipoDeporte 2 monitor 3 fecha 4 lista de DTOs esperamos recibir
        new object?[] { null, null, null, clasesDTOsTC1 },//esperamos las 3 clases de deporte , por eso todo a null
        new object?[] { "Tenis", null, null, clasesDTOsTC2 },//esperamos la clase de tenis
        new object?[] { null, "Carlos", null, clasesDTOsTC3 },//filtramos por  monitor , resultado baloncesto
        new object?[] { null, null, new DateTime(2026,11,6), clasesDTOsTC4 },//filtramos por fecha resultado clase de tenis
        new object?[] {"Tenis", "Miguel",new DateTime(2026, 11, 7),clasesDTOsTC2}//filtramos por deporte, monitor y fecha a la vez , resultado  clase de tenis
    };

    return allTests;//devolvemos los cincos casos y estos se usaran para ejecutar  el Theory
}


[Theory]//indica que vamos a ejecutar varios casos en el mismo metodo
//MemberData obtiene los datos del metodo que hemos creado anteriormente
//nameof indica el nombre del metodo
//se ejecutaran 5 pruebas
[MemberData(nameof(TestCasesFor_GetClasesDeportivasForInscripcion_OK))]
//etiqueta para identificar que usamos base de datos sin fixture
[Trait("Database", "WithoutFixture")]
[Trait("LevelTesting", "Unit Testing")]//etiqueta para clasificar estas pruebas como unitarias

//metodo que comprueba que el select devuelve los resultados correctos
//es asincrono por lo tanto podemos usar await
// task representa la operacion asincrona
public async Task GetClasesDeportivasForInscripcion_OK_test(
    string? tipoDeporte,  //filtro por deporte  , puede ser null
    string? monitor,//filtro por monitor , puede  ser null
    DateTime? fecha,//  filtro por fecha , puede ser null
    IList<ClaseDeportivaForInscripcionDTO> expectedClases)//clases esperadas
{
    // Arrange: preparamos el controlador para hacer la prueba
    //mock es para simular  una dependencia sin usar un logger real
    var mock = new Mock<ILogger<ClasesDeportivasController>>();
    //obtenemos el objeto  logger  simulado del mock
    //se lo pasamos al controctor del controlador
    ILogger<ClasesDeportivasController> logger = mock.Object;
    //creamos una instancia real del controlador que queremos probar
    //_context tiene las clases deportivas guardadas en el SQLLite
    //logger objeto simulado que acabamos de crear
    var controller = new ClasesDeportivasController(_context, logger);

    // Act: ejecutamos el metodo SELECT , usando filtros que recibe cada caso
    //await espera a que termine la consulta a la base de datos
    //result guardara la respuesta que devuelve el controlador
    var result = await controller.GetClasesDeportivasForInscripcion(tipoDeporte, monitor, fecha);

    // Assert: comprobamos que devuelve OK y las clases esperadas
    //comprobamos que devuelve un objeto  tipo OkObjetctResult
    //el 200 ok
    //si devuelve otra cosa , la prueba falla
    var okResult = Assert.IsType<OkObjectResult>(result);
    //Obtenemos el contenido de la respuesta mediante value
    //comprobamos que wsea una lista de ClaseDeportivaForInscripcionDTO
    //guardamos en clasesActuales
    //si no son iguales la prueba falla
    var clasesActuales = Assert.IsType<List<ClaseDeportivaForInscripcionDTO>>(okResult.Value);
    //comparamos las clases
    Assert.Equal(expectedClases, clasesActuales);
}

//metodo que prepara los casos que deben devolver NotFound
public static IEnumerable<object?[]> TestCasesFor_GetClasesDeportivasForInscripcion_NotFound()
{
    // creamos una lista con dos casos de erro
    //cada array contiene tres filtros: deporte, monitor y fecha
    //no necesitamos dtos esperados porque buscamos un error 404
    var allTests = new List<object?[]>
    {
        //caso 1 clase que no existe  en nuestra SQLLITE
        // deberia devolver Not found
        new object?[] { "Natacion", null, null },
        //caso 2 clase de tenis completa, existe en SQLLITE
        // pero el  select no debe devolverlo porque no hay plazas
        new object?[] { "Tenis", "Miguel", new DateTime(2026, 11, 8) }
    };

    return allTests;//devolvemos los dos casos de error para usarlos en el Theory
}

[Theory]//ejecutaremos varios casos
//memberdata obtiene los dos casos  de error preparaodsen el metodo anterior
[MemberData(nameof(TestCasesFor_GetClasesDeportivasForInscripcion_NotFound))]
[Trait("LevelTesting", "Unit Testing")]//etiqueta  para  indicar  que son pruebas unitarias
[Trait("Database", "WithoutFixture")]//etiqueta para decir que usamos base de datos sin fixture

//metodo que comprueba cuando el select no encuentra clases disponibles
//recibe los tres filtros de cada caso de prueba
public async Task GetClasesDeportivasForInscripcion_NotFound_test( string? tipoDeporte,//deporte que queremos buscar
 string? monitor, //monitor que queremos buscar
 DateTime? fecha)//fecha de la clase que buscamos
{
    // Arrange: preparamos el controlador 
    //creamos un mock para simular el loggerdel controlador
    var mock = new Mock<ILogger<ClasesDeportivasController>>();
    //obtenemos el logger simulado  del mock
    ILogger<ClasesDeportivasController> logger = mock.Object;
    //creamos el controladorcon nuestra base SQLLite y el logger
    var controller = new ClasesDeportivasController(_context, logger);

   // Act: buscamos clases con los filtros de cada caso
   //llamamos al controlador buscando las clases con los filtros indicados
   //result teien la respuesta por el controlador
var result = await controller.GetClasesDeportivasForInscripcion(
    tipoDeporte, monitor, fecha);

    // Assert: comprobamos que devuelve NotFound
    //debe dar  404 Not found
    //si devuelve otra cosa error
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

    // Comprobamos el mensaje de error
    var mensaje = Assert.IsType<string>(notFoundResult.Value);
    //comparamos el mensaje recibido con el que esperamos debe coincidir exactamente con  el mensaje del controlador
    //si es distinto error
    Assert.Equal("No se han encontrado clases deportivas con plazas disponibles", mensaje);
}




    }
}