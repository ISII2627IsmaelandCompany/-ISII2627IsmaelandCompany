using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.MaterialDTOs;

namespace AppForSEII.UT.MaterialesController_test
{
    public class GetMateriales_test : AppForSEII4SqliteUT
    {
        // crea los datos en SQLite en memoria
        public GetMateriales_test()
        {
            // Material necesita un TipoDeporte (por el FK), asi que se crea
            var tiposDeporte = new List<TipoDeporte>()
            {
                new TipoDeporte("Baloncesto"), // posicion 0
                new TipoDeporte("Futbol"),     // posicion 1
                new TipoDeporte("Tenis")       // posicion 2
            };

            //TipoMaterial(idTipoMaterial, nombreTipoMaterial)
            // los ids se pasan explicitamente, asi el filtro por idTipoMaterial es predecible
            var tiposMaterial = new List<TipoMaterial>()
            {
                new TipoMaterial(1, "Balones"),      // id 1
                new TipoMaterial(2, "Raquetas"),     // id 2
                new TipoMaterial(3, "Protecciones")  // id 3
            };

            // Material(cantidad, idMaterial, nombre, precio)
            // TipoDeporte y TipoMaterial se asignan con inicializador porque el constructor no los recibe
            var materiales = new List<Material>()
            {
                new Material(10, 1, "Balon de baloncesto", 15.00m)
                    { TipoDeporte = tiposDeporte[0], TipoMaterial = tiposMaterial[0] },
                new Material(8, 2, "Balon de futbol", 12.00m)
                    { TipoDeporte = tiposDeporte[1], TipoMaterial = tiposMaterial[0] },
                new Material(5, 3, "Raqueta de tenis", 45.50m)
                    { TipoDeporte = tiposDeporte[2], TipoMaterial = tiposMaterial[1] },
                new Material(30, 4, "Pelota de tenis", 3.00m)
                    { TipoDeporte = tiposDeporte[2], TipoMaterial = tiposMaterial[0] },
                new Material(12, 5, "Espinillera", 8.00m)
                    { TipoDeporte = tiposDeporte[1], TipoMaterial = tiposMaterial[2] }
            };

            _context.AddRange(tiposDeporte);
            _context.AddRange(tiposMaterial);
            _context.AddRange(materiales);
            _context.SaveChanges();
        }

        // CASOS OK
        public static IEnumerable<object?[]> TestCasesFor_GetMateriales_OK()
        {
            // DTOs esperados
            var dtos = new List<MaterialDTO>()
            {
                new MaterialDTO(1, "Balon de baloncesto", 15.00m, 10), // 0
                new MaterialDTO(2, "Balon de futbol", 12.00m, 8),      // 1
                new MaterialDTO(3, "Raqueta de tenis", 45.50m, 5),     // 2
                new MaterialDTO(4, "Pelota de tenis", 3.00m, 30),      // 3
                new MaterialDTO(5, "Espinillera", 8.00m, 12)           // 4
            };

            // Orden por Nombre porque el controller lo hace con orderby:
            // Balon de baloncesto, Balon de futbol, Espinillera, Pelota de tenis, Raqueta de tenis
            var todos = new List<MaterialDTO>() { dtos[0], dtos[1], dtos[4], dtos[3], dtos[2] };

            // cada caso tiene 5 posiciones, en el MISMO orden que los parametros del test:
            // cada caso con idMaterial, nombre, precioMax, idTipoMaterial, lista esperada
            return new List<object?[]>
            {
                // 1 Sin filtros -> todos
                new object?[] { null, null, null, null, todos },

                // 2 Nombre vacio (rama string.IsNullOrEmpty) -> todos
                new object?[] { null, "", null, null, todos },

                // 3 Solo por idMaterial
                new object?[] { 3, null, null, null, new List<MaterialDTO>() { dtos[2] } },

                // 4 Solo por nombre ("tenis" -> pelota y raqueta)
                new object?[] { null, "tenis", null, null, new List<MaterialDTO>() { dtos[3], dtos[2] } },

                // 5 Solo por precioMax (12.00 incluye el limite: balon futbol, espinillera, pelota)
                new object?[] { null, null, 12.00m, null, new List<MaterialDTO>() { dtos[1], dtos[4], dtos[3] } },

                // 6 Solo por tipo de material (1 = Balones)
                new object?[] { null, null, null, 1, new List<MaterialDTO>() { dtos[0], dtos[1], dtos[3] } },

                // 7 Combinacion nombre + precioMax + tipo (solo el balon de futbol)
                new object?[] { null, "Balon", 12.00m, 1, new List<MaterialDTO>() { dtos[1] } },

                // 8 Los cuatro filtros a la vez
                new object?[] { 4, "tenis", 5.00m, 1, new List<MaterialDTO>() { dtos[3] } },

                // 9 Sin coincidencias por nombre -> Ok con lista vacia (no NotFound)
                new object?[] { null, "padel", null, null, new List<MaterialDTO>() },

                // 10 Sin coincidencias por precio (ninguno cuesta 0) -> lista vacia
                new object?[] { null, null, 0m, null, new List<MaterialDTO>() }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetMateriales_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        // async Task porque el controller es asincrono (usamos await)
        // los parametros reciben, en orden, las 5 posiciones de cada array object?
        public async Task GetMateriales_OK_test(
            int? idMaterial,
            string? nombre,
            decimal? precioMax,
            int? idTipoMaterial,
            IList<MaterialDTO> expectedMateriales)
        {
            // arrange, preparar el controller
            var mock = new Mock<ILogger<MaterialesController>>();
            ILogger<MaterialesController> logger = mock.Object;
            var controller = new MaterialesController(_context, logger);

            // act, ejecuta el metodo con los filtros del caso
            var result = await controller.GetMateriales(idMaterial, nombre, precioMax, idTipoMaterial);
            
            //assert, comprobar el resultado
            // el metodo devuelve ActionResult<IList<MaterialDTO>>, el OkObjectResult esta en result.Result 
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actuales = Assert.IsType<List<MaterialDTO>>(okResult.Value);

            // equal compara elemento a elemento (usa el Equals del DTO) y tambien el orden
            Assert.Equal(expectedMateriales, actuales);
        }

        // CASOS BADREQUEST
        public static IEnumerable<object?[]> TestCasesFor_GetMateriales_BadRequest()
        {
            // Cada caso: idMaterial, nombre, precioMax, idTipoMaterial, clave del error, mensaje esperado
            return new List<object?[]>
            {
                // idMaterial = 0
                new object?[] { 0, null, null, null,
                    "idMaterial", "El ID del material debe ser mayor a 0." },

                // idMaterial negativo
                new object?[] { -5, null, null, null,
                    "idMaterial", "El ID del material debe ser mayor a 0." },

                // precioMax negativo
                new object?[] { null, null, -0.01m, null,
                    "precioMax", "El precio máximo no puede ser negativo." },

                // idTipoMaterial = 0
                new object?[] { null, null, null, 0,
                    "idTipoMaterial", "El ID del tipo de material debe ser mayor a 0." },

                // idTipoMaterial negativo
                new object?[] { null, null, null, -1,
                    "idTipoMaterial", "El ID del tipo de material debe ser mayor a 0." },

                // cuando varios filtros no son validos el controller devuelve en el primero el de idMaterial
                new object?[] { 0, null, -1m, -1,
                    "idMaterial", "El ID del material debe ser mayor a 0." }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetMateriales_BadRequest))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetMateriales_BadRequest_test(
            int? idMaterial,
            string? nombre,
            decimal? precioMax,
            int? idTipoMaterial,
            string claveEsperada,
            string mensajeEsperado)
        {
            // arrange, preparar el controller
            var mock = new Mock<ILogger<MaterialesController>>();
            ILogger<MaterialesController> logger = mock.Object;
            var controller = new MaterialesController(_context, logger);

            // act, ejecuta el metodo con los filtros del caso
            var result = await controller.GetMateriales(idMaterial, nombre, precioMax, idTipoMaterial);

            // assert, comprobar el resultado
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // solo debe haber un error, del campo y el mensaje esperado
            Assert.Single(problemDetails.Errors);
            Assert.Equal(claveEsperada, problemDetails.Errors.First().Key);
            Assert.Equal(mensajeEsperado, problemDetails.Errors.First().Value[0]);
        }
    }
}