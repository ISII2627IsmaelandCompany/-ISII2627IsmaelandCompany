using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.InscripcionDTOs;
using AppForSEII.UT;

public class CrearInscripcion_test : AppForSEII4SqliteUT
{
    public CrearInscripcion_test()
    {
        // Preparar usuario y competiciones
        var usuario = new ApplicationUser(//usuario de prueba
            "1",
            "Jose",
            "Garcia",
            "jose.garcia@uclm.es"
        )
        {//propiedades del usuario
            DNI = "12345678A",
            Age = 25,
            PhoneNumber = "123456789",
            EmailConfirmed = true
        };

        var competicion = new Competicion(//competicion de prueba
            DateTime.Now,
            "Albacete",
            "Torneo de Voley",
            100,
            15m
        );
        var competicionSinPlazas = new Competicion(//competicion de prueba sin plazas
            DateTime.Now,
            "Albacete",
            "Torneo de Voley",
            0,
            20m
        );
        //guardar usuario y competiciones en la base de datos
        _context.Users.Add(usuario);
        _context.Competiciones.Add(competicion);
        _context.Competiciones.Add(competicionSinPlazas);
        _context.SaveChanges();
    }
    public static IEnumerable<object[]> CasosTestPara_CrearInscripcion()//esta función devuelve una lista de casos de prueba para el método CrearInscripcion del controlador InscripcionesController.
    //cada caso de prueba consiste en un objeto CrearInscripcionDTO con datos inválidos y el mensaje de error esperado.
    {
        var SinCompeticiones = new CrearInscripcionDTO//caso de prueba para crear una inscripción sin competiciones seleccionadas
        {
            NombreUsuarioCliente = "jose.garcia@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>()
        };
        var CompeticionDuplicada = new CrearInscripcionDTO//caso de prueba para crear una inscripción con competiciones duplicadas
        {
            NombreUsuarioCliente = "jose.garcia@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>
            {
                new CompeticionSeleccionadaDTO { IdCompeticion = 1, ProblemasFisicos = "Ninguno" },
                new CompeticionSeleccionadaDTO { IdCompeticion = 1, ProblemasFisicos = "Ninguno" }
            }
        };
        var CompeticionInexistente = new CrearInscripcionDTO//caso de prueba para crear una inscripción con una competición que no existe en la base de datos
        {
            NombreUsuarioCliente = "jose.garcia@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>
            {
                new CompeticionSeleccionadaDTO { IdCompeticion = 999, ProblemasFisicos = "Ninguno" }
            }
        };
        var CompeticionSinPlazas = new CrearInscripcionDTO//caso de prueba para crear una inscripción con una competición sin plazas disponibles
        {
            NombreUsuarioCliente = "jose.garcia@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>
            {
                new CompeticionSeleccionadaDTO { IdCompeticion = 2, ProblemasFisicos = "Ninguno" }
            }
        };
        var TodasLasInscripciones = new List<Object[]>//lista de casos de prueba para el método CrearInscripcion del controlador InscripcionesController
        {
            new object[] { SinCompeticiones, "Debe seleccionar al menos una competición." },
            new object[] { CompeticionDuplicada, "No se puede seleccionar una misma competición varias veces." },
            new object[] { CompeticionInexistente, "Una o varias competiciones no existen." },
            new object[] { CompeticionSinPlazas, "No quedan plazas disponibles para la competición Torneo de Voley." }
        };
        return TodasLasInscripciones;
    }

    [Theory]//esta prueba unitaria verifica que el método CrearInscripcion del controlador InscripcionesController devuelve un BadRequestObjectResult con el mensaje de error esperado cuando se intenta crear una inscripción con datos inválidos.
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    [MemberData(nameof(CasosTestPara_CrearInscripcion))]
    public async Task CrearInscripcion_CasosDeError(CrearInscripcionDTO crearInscripcionDTO, string errorEsperado)
    {// pasamos parametros de prueba a la función CrearInscripcion_CasosDeError, que son un objeto CrearInscripcionDTO con datos inválidos y el mensaje de error esperado.
        // Arrange
        var mock = new Mock<ILogger<InscripcionesController>>();
        ILogger<InscripcionesController> logger = mock.Object;
        var controller = new InscripcionesController(_context, logger);

        // Act
        var result = await controller.CrearInscripcion(crearInscripcionDTO);// llamamos al método CrearInscripcion del controlador InscripcionesController con el objeto CrearInscripcionDTO con datos inválidos.

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);// verificamos que el resultado devuelto sea un BadRequestObjectResult.
        var detallesError = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);// verificamos que el valor del BadRequestObjectResult sea un ValidationProblemDetails.
        var errorActual = detallesError.Errors.FirstOrDefault().Value.FirstOrDefault();// obtenemos el primer mensaje de error del ValidationProblemDetails.

        Assert.StartsWith(errorEsperado, errorActual);// verificamos que el mensaje de error actual comience con el mensaje de error esperado.
    }
    [Fact]//esta prueba unitaria verifica que el método CrearInscripcion del controlador InscripcionesController devuelve un BadRequestResult cuando se intenta crear una inscripción para un cliente que no existe en la base de datos.
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task CrearInscripcion_ClienteNoExiste_test()// pasamos parametros de prueba a la función CrearInscripcion_ClienteNoExiste_test, que son un objeto CrearInscripcionDTO con datos válidos pero con un cliente que no existe en la base de datos.
    {// pasamos parametros de prueba a la función CrearInscripcion_ClienteNoExiste_test, que son un objeto CrearInscripcionDTO con datos válidos pero con un cliente que no existe en la base de datos.
        // Arrange
        var crearInscripcionDTO = new CrearInscripcionDTO
        {
            NombreUsuarioCliente = "usuario.inexistente@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>
        {
            new CompeticionSeleccionadaDTO
            {
                IdCompeticion = 1,
                ProblemasFisicos = "Ninguno"
            }
        }
        };

        var mock = new Mock<ILogger<InscripcionesController>>();
        ILogger<InscripcionesController> logger = mock.Object;
        var controller = new InscripcionesController(_context, logger);

        // Act
        var result = await controller.CrearInscripcion(crearInscripcionDTO);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    [Trait("Database", "WithoutFixture")]
    public async Task CrearInscripcion_InscripcionCorrecta_test()// pasamos parametros de prueba a la función CrearInscripcion_InscripcionCorrecta_test, que son un objeto CrearInscripcionDTO con datos válidos y un cliente que existe en la base de datos.
    {
        // Arrange
        var crearInscripcionDTO = new CrearInscripcionDTO// creamos un objeto CrearInscripcionDTO con datos válidos y un cliente que existe en la base de datos.
        {
            NombreUsuarioCliente = "jose.garcia@uclm.es",
            MetodoPago = MetodoPago.Bizum,
            CompeticionesSeleccionadas = new List<CompeticionSeleccionadaDTO>
            {
                new CompeticionSeleccionadaDTO
                {
                    IdCompeticion = 1,
                    ProblemasFisicos = "Ninguno"
                }
            }
        };

        var mock = new Mock<ILogger<InscripcionesController>>();
        ILogger<InscripcionesController> logger = mock.Object;
        var controller = new InscripcionesController(_context, logger);

        //act
        var result = await controller.CrearInscripcion(crearInscripcionDTO);

        //assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(InscripcionesController.GetInscripcion), createdResult.ActionName);
        var inscripcionDetail = Assert.IsType<InscripcionDetailDTO>(createdResult.Value);


        var competicionesEsperadas = new List<InscripcionItemDTO>
{
    new InscripcionItemDTO(//creamos un objeto InscripcionItemDTO con los datos esperados de la inscripción creada.
        "Torneo de Voley",
        "Albacete",
        _context.Competiciones.First().Fecha,
        15m,
        "Ninguno"
    )
};

        var inscripcionEsperada = new InscripcionDetailDTO(//creamos un objeto InscripcionDetailDTO con los datos esperados de la inscripción creada.
            inscripcionDetail.Id,
            "Jose",
            "Garcia",
            "12345678A",
            "jose.garcia@uclm.es",
            "123456789",
            inscripcionDetail.FechaInscripcion,
            15m,
            MetodoPago.Bizum,
            competicionesEsperadas
        );

        Assert.Equal(inscripcionEsperada, inscripcionDetail);// verificamos que el objeto InscripcionDetailDTO devuelto por el método CrearInscripcion sea igual al objeto InscripcionDetailDTO esperado.

    }
}



