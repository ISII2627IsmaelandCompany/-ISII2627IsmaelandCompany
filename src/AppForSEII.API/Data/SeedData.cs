namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };//roles de la aplicacion

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();//obtener RoleManager para crear y consultar roles del Identity
            try {
                SeedRoles(roleManager, rolesNames);//crea roles si no existen en la base de datos
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");//registramos en el log
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();//obtener UserManager para crear usuarios de Identity
            try {
                SeedUsers(userManager, rolesNames);//crea  usuariossi no existen todavia
              
                
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");//registramos error en log
            }
            try {
            // Inicializa la base de datos con tipos de deporte y clases deportivas del CU4
            //Para probar el select de clases deportivas
            SeedTiposDeporteAndClasesDeportivas(dbContext);
            }
            catch (Exception ex) {
            logger.LogError(ex, "An error occurred seeding TipoDeporte and ClaseDeportiva in the Database.");
            }
            try {//************
            // Inicializa la base de datos con tipos de deporte y clases deportivas del CU4  
            SeedTiposDeporteAndPistas(dbContext);
            }
            catch (Exception ex) {
            logger.LogError(ex, "An error occurred seeding TipoDeporte and Pista in the Database.");//registramos error en el log
            }
            try
            {
            // Inicializa la base de datos con competiciones
            SeedCompeticiones(dbContext);
    
            }
            catch (Exception ex)
            {
            logger.LogError(ex, "An error occurred seeding Competicion in the Database.");
            }
            try
            {
                // Inicializa material y materiales para alquiler
                SeedTiposMaterialAndMateriales(dbContext);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred seeding TipoMaterial and Material in the Database.");
            }
             try
            {
                // Inicializa una  inscripcion y sus clases incritas para el details del CU4
                SeedInscripcionesAndClasesInscritas(dbContext);
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred seeding Inscripcion and ClaseInscrita in the Database.");//registramos cualquier error en el log
            }
            
        }
        

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {//recorremos todos los nombres de roles  recibidos
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {//comprobamos si ese rol todavia no existe en la bbdd
                    IdentityRole role = new IdentityRole();//objeto  identityRole
                    role.Name = roleName;//nombre del rol
                    role.NormalizedName = roleName;//nombre normalizado
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;//guardamos rol con Identity
                }
            }

        }

        //funcion como el de seedgenresandmovies añadiendo ejemplos para la base de  datos del  CU4 (con este comprobamos que funcione el select)
        public static void SeedTiposDeporteAndClasesDeportivas(ApplicationDbContext dbcontext)
    {
        string[] tiposDeporteNombres =["Baloncesto","Futbol","Tenis"];//lista de deportes

        List<TipoDeporte> tiposDeporte = [];//lista  para guardar los encontrados o creados

    foreach (string tipoDeporteNombre in tiposDeporteNombres)//recorremos cada nombre de deporte
    {
        var tipoDeporte = dbcontext.TipoDeportes.FirstOrDefault(t => t.Nombre == tipoDeporteNombre);//buscar primero si ya existe  en la base  de datos

        if (tipoDeporte == null)//si no existe creamos uno y lo añadimos a la lista
            tiposDeporte.Add(new TipoDeporte(tipoDeporteNombre));
        else
            tiposDeporte.Add(tipoDeporte);//si existe usamos  el que estaba guardado
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(//comprobar si existe ya clase baloncesto
        c => c.Descripcion == "Entrenamiento de baloncesto") == null)
    {//sino existe creamos la clase                 descripcion                 baloncesto          fecha y hora                    Lugar     Monitor   Nivel      plazas disponibles precio unitario
        var claseDeportiva = new ClaseDeportiva("Entrenamiento de baloncesto",tiposDeporte[0],new DateTime(2026, 10, 5, 18, 0, 0),"Pista 1","Carlos","Iniciacion",20,8.00m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);//añadimos la clase al db context 
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(c => c.Descripcion == "Entrenamiento de futbol") == null)//comprobar si existe ya clase futbol
    {//sino existe creamos la clase               descripcion            futbol          fecha y hora                          Lugar     Monitor   Nivel      plazas disponibles precio unitario
        var claseDeportiva = new ClaseDeportiva("Entrenamiento de futbol",tiposDeporte[1],new DateTime(2026, 10, 6, 19, 0, 0),"Pista 2","Laura","Intermedio",15,10.00m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);//añadimos la clase al db context 
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(
        c => c.Descripcion == "Clase de tenis") == null)//comprobar si existe ya clase tenis
    {//sino existe creamos la clase               descripcion            tenis          fecha y hora               Lugar(no esta definido)     Monitor   Nivel      plazas disponibles precio unitario
        var claseDeportiva = new ClaseDeportiva("Clase de tenis",tiposDeporte[2],new DateTime(2026, 10, 7, 17, 30, 0),null,"Miguel","Avanzado",10,12.50m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);//añadimos la clase al db context 
    }

    // NUEVA clase sin plazas para probar que el GET no la devuelve
    if (dbcontext.ClasesDeportivas.FirstOrDefault(
        c => c.Descripcion == "Clase de tenis completa") == null)//comprobar si existe ya clase tenis
    {//sino existe creamos la clase               descripcion            tenis          fecha y hora                         Lugar      Monitor   Nivel      plazas disponibles precio unitario
        var claseDeportiva = new ClaseDeportiva("Clase de tenis completa",tiposDeporte[2],new DateTime(2026, 10, 8, 18, 0, 0),"Pista 3","Miguel","Intermedio",0,9.50m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);//añadimos la clase al db context 
    }
    //guarda  las modificaciones de dbcontext en database
    dbcontext.SaveChanges();
}

//***********************************************************************************DANI
public static void SeedTiposDeporteAndPistas(ApplicationDbContext dbcontext)
{
    string[] tiposDeporteNombres= ["Baloncesto", "Futbol", "Tenis"];

    List<TipoDeporte> tiposDeporte =[];

    foreach (string tipoDeporteNombre in tiposDeporteNombres)
    {
        var tipoDeporte = dbcontext.TipoDeportes
            .FirstOrDefault(t => t.Nombre == tipoDeporteNombre);

        if (tipoDeporte== null)
            tiposDeporte.Add(new TipoDeporte(tipoDeporteNombre));
        else
            tiposDeporte.Add(tipoDeporte);
    }

    if (dbcontext.Pistas.FirstOrDefault(
        p => p.NombrePista == "Pista de baloncesto") ==null)
    {
        var pista= new Pista("Pista de baloncesto",12.00,20,0);

        pista.TipoDeporte= tiposDeporte[0];

        dbcontext.Pistas.Add(pista);
    }

    if (dbcontext.Pistas.FirstOrDefault(
        p => p.NombrePista== "Pista de futbol") == null)
    {
        var pista =new Pista("Pista de futbol",15.00,22,1);

        pista.TipoDeporte = tiposDeporte[1];

        dbcontext.Pistas.Add(pista);
    }

    if (dbcontext.Pistas.FirstOrDefault(
        p => p.NombrePista == "Pista de tenis") == null)
    {
        var pista= new Pista("Pista de tenis",10.00,4,1);

        pista.TipoDeporte =tiposDeporte[2];

        dbcontext.Pistas.Add(pista);
    }

    dbcontext.SaveChanges();
}

public static void SeedReservasAndPistasReservadas(ApplicationDbContext dbcontext)
{
    // Buscamos las dos pistas que creamos en el seed anterior (baloncesto y fútbol)
    var cliente = dbcontext.Users.FirstOrDefault(u => u.Email == "peter@uclm.es");
    // Buscamos las pistas deportivas ya creadas en el seed de tipo deporte y pistas
    var pistaBaloncesto = dbcontext.Pistas.FirstOrDefault(p => p.NombrePista == "Pista de baloncesto");
    var pistaFutbol = dbcontext.Pistas.FirstOrDefault(p => p.NombrePista == "Pista de futbol");

    if (cliente != null && pistaBaloncesto != null && pistaFutbol != null && !dbcontext.Reservas.Any())//comprobamos q exista el cliente para hacer la reserva
    {
        // Creamos la reserva pasandole el objeto cliente 
        var reserva = new Reserva(cliente,MetodoPago.Tarjeta,new DateTime(2026,11,02),26.00m);
       
        dbcontext.Reservas.Add(reserva);
        dbcontext.SaveChanges(); // Guardamos para que la reserva obtenga su ID
    
        // 4. Creamos la primera pista reservada 
        var pistaReservada1 = new PistaReservada(pistaBaloncesto,2,24.00)
        {
            Reserva = reserva,
            IdReserva = reserva.Id,
            Observaciones = "Sin observaciones",
        };

        // 5. Creamos la segunda pista reservada 
        var pistaReservada2 = new PistaReservada(pistaFutbol,1,15.00)
        {
            Reserva = reserva,
            IdReserva = reserva.Id,
            Observaciones = "Reserva nocturna",
        };
    

        dbcontext.PistaReservada.AddRange(pistaReservada1, pistaReservada2);
        dbcontext.SaveChanges();
    }
}   

//******************************************************************************************
public static void SeedCompeticiones(ApplicationDbContext dbcontext)
{
    string[] tiposDeporteNombres = ["Baloncesto", "Futbol", "Tenis"];

    List<TipoDeporte> tiposDeporte = [];

    foreach (string tipoDeporteNombre in tiposDeporteNombres)
    {
        var tipoDeporte = dbcontext.TipoDeportes
            .FirstOrDefault(t => t.Nombre == tipoDeporteNombre);

        if (tipoDeporte != null)
            tiposDeporte.Add(tipoDeporte);
    }

    if (dbcontext.Set<Competicion>().FirstOrDefault(
        c => c.Nombre == "Torneo de Baloncesto UCLM") == null)
    {
        var competicion = new Competicion(
            new DateTime(2026, 11, 15),
            "Pabellón Universitario",
            "Torneo de Baloncesto UCLM",
            20,
            15.00m
        );

        competicion.TipoDeporte = tiposDeporte[0];

        dbcontext.Set<Competicion>().Add(competicion);
    }

    if (dbcontext.Set<Competicion>().FirstOrDefault(
        c => c.Nombre == "Torneo de Futbol UCLM") == null)
    {
        var competicion = new Competicion(
            new DateTime(2026, 11, 22),
            "Campo de Fútbol UCLM",
            "Torneo de Futbol UCLM",
            30,
            12.50m
        );

        competicion.TipoDeporte = tiposDeporte[1];

        dbcontext.Set<Competicion>().Add(competicion);
    }

    if (dbcontext.Set<Competicion>().FirstOrDefault(
        c => c.Nombre == "Torneo de Tenis UCLM") == null)
    {
        var competicion = new Competicion(
            new DateTime(2026, 12, 5),
            "Pistas de Tenis UCLM",
            "Torneo de Tenis UCLM",
            16,
            10.00m
        );

        competicion.TipoDeporte = tiposDeporte[2];

        dbcontext.Set<Competicion>().Add(competicion);
    }

    dbcontext.SaveChanges();
}
public static void SeedTiposMaterialAndMateriales(ApplicationDbContext dbcontext)
{
    // Tipos de deporte
    string[] tiposDeporteNombres = ["Baloncesto", "Futbol", "Tenis"];

    List<TipoDeporte> tiposDeporte = [];

    foreach (string tipoDeporteNombre in tiposDeporteNombres)
    {
        var tipoDeporte = dbcontext.TipoDeportes
            .FirstOrDefault(t => t.Nombre == tipoDeporteNombre);

        if (tipoDeporte == null)
        {
            tipoDeporte = new TipoDeporte(tipoDeporteNombre);
            dbcontext.TipoDeportes.Add(tipoDeporte);
        }

        tiposDeporte.Add(tipoDeporte);
    }


    // Tipos de material
    var tipoBalones = dbcontext.TipoMaterials
        .FirstOrDefault(t => t.NombreTipoMaterial == "Balones");

    if (tipoBalones == null)
    {
        tipoBalones = new TipoMaterial
        {
            NombreTipoMaterial = "Balones",
            Materiales = new List<Material>()
        };

        dbcontext.TipoMaterials.Add(tipoBalones);
    }


    var tipoRaquetas = dbcontext.TipoMaterials
        .FirstOrDefault(t => t.NombreTipoMaterial == "Raquetas");

    if (tipoRaquetas == null)
    {
        tipoRaquetas = new TipoMaterial
        {
            NombreTipoMaterial = "Raquetas",
            Materiales = new List<Material>()
        };

        dbcontext.TipoMaterials.Add(tipoRaquetas);
    }


    //Guardamos primero para que SQL Server genere los IDs
    dbcontext.SaveChanges();


    // Material de baloncesto
    if (dbcontext.Materials.FirstOrDefault(
        m => m.Nombre == "Balon de baloncesto") == null)
    {
        var material = new Material
        {
            Nombre = "Balon de baloncesto",
            Precio = 3.00m,
            Cantidad = 10,

            TipoDeporte = tiposDeporte[0],
            TipoDeporteId = tiposDeporte[0].Id,

            TipoMaterial = tipoBalones,
            TipoMaterialId = tipoBalones.IdTipoMaterial
        };

        dbcontext.Materials.Add(material);
    }


    // Material de futbol
    if (dbcontext.Materials.FirstOrDefault(
        m => m.Nombre == "Balon de futbol") == null)
    {
        var material = new Material
        {
            Nombre = "Balon de futbol",
            Precio = 4.00m,
            Cantidad = 12,

            TipoDeporte = tiposDeporte[1],
            TipoDeporteId = tiposDeporte[1].Id,

            TipoMaterial = tipoBalones,
            TipoMaterialId = tipoBalones.IdTipoMaterial
        };

        dbcontext.Materials.Add(material);
    }


    // Material de tenis
    if (dbcontext.Materials.FirstOrDefault(
        m => m.Nombre == "Raqueta de tenis") == null)
    {
        var material = new Material
        {
            Nombre = "Raqueta de tenis",
            Precio = 5.00m,
            Cantidad = 8,

            TipoDeporte = tiposDeporte[2],
            TipoDeporteId = tiposDeporte[2].Id,

            TipoMaterial = tipoRaquetas,
            TipoMaterialId = tipoRaquetas.IdTipoMaterial
        };

        dbcontext.Materials.Add(material);
    }


    dbcontext.SaveChanges();
}

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {//buscamos si elena  ya exites el username comprobando el correo
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");//creamos un usuario
                user.EmailConfirmed = true;//email=comfirmado //  id  nombre         apellidos        email

                var result = userManager.CreateAsync(user, "Password1234%");//crear usuario con esta contrasena
                result.Wait();//esperar operacion  asincrona

                if (result.IsCompletedSuccessfully) {//si se ha  creado correctamente
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();//le asignamos primer rol de la lista Administrador
                }
            }

                //lo  mismo con  peter
            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }


public static void SeedInscripcionesAndClasesInscritas(ApplicationDbContext dbcontext)//metodo para el details
{
    // Buscamos un cliente ya creado concretamente peter
    var cliente = dbcontext.Users.FirstOrDefault(u => u.Email == "peter@uclm.es");

    // Buscamos dos clases deportivas ya creadas  concretamente la de baloncesto y futbol  (que son creadas en el seed de tipo  deporte)
    var claseBaloncesto = dbcontext.ClasesDeportivas.FirstOrDefault(c => c.Descripcion == "Entrenamiento de baloncesto");

    var claseFutbol = dbcontext.ClasesDeportivas.FirstOrDefault(c => c.Descripcion == "Entrenamiento de futbol");

    // Solo creamos la inscripción si existen el cliente y las clase de baloncesto y futbol y todavia no  existe ninguna inscripcion
    if (cliente != null &&claseBaloncesto != null &&claseFutbol != null &&!dbcontext.Inscripciones.Any())
    {//inscripcion                       cliente    datos de pago               fecha hora                metodo pago        precio total
        var inscripcion = new Inscripcion(cliente,"Tarjeta terminada en 1234",new DateTime(2026, 10, 20),MetodoPago.Tarjeta,26.00m);

        dbcontext.Inscripciones.Add(inscripcion);//añadimos la inscripcion al dbcontext

        // Guardamos para que la inscripción tenga Id
        dbcontext.SaveChanges();
//creamos la inscripcion para baloncesto reservando dos plazas
//                                               observaciones     plazas reservadas  precio =  2 (plazas  reservadas) * 8 (precio unitario )
        var claseInscrita1 = new ClaseInscrita("Sin observaciones",2,16.00m)
        {
            ClaseDeportiva = claseBaloncesto,//relacionar  clase Inscrita con la  clase baloncesto
            Inscripcion = inscripcion, //relacionar clase Inscrita con  Inscripcion
            ClaseDeportivaId = claseBaloncesto.Id,//guardamos FK clase deportiva
            InscripcionId = inscripcion.Id //Guardamos FK  de  inscripcion
        };
//igual para futbol
        var claseInscrita2 = new ClaseInscrita(null,1,10.00m)
        {
            ClaseDeportiva = claseFutbol,
            Inscripcion = inscripcion,
            ClaseDeportivaId = claseFutbol.Id,
            InscripcionId = inscripcion.Id
        };

        dbcontext.ClasesInscritas.AddRange(claseInscrita1,claseInscrita2);

        dbcontext.SaveChanges();
    }
}


    }

    
}