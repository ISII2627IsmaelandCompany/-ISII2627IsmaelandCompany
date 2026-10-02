namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
              
                
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }
            try {
            // Inicializa la base de datos con tipos de deporte y clases deportivas del CU4
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
            logger.LogError(ex, "An error occurred seeding TipoDeporte and Pista in the Database.");
            }
            
        }
        

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        //funcion como el de seedgenresandmovies añadiendo ejemplos para la base de  datos del  CU4
        public static void SeedTiposDeporteAndClasesDeportivas(ApplicationDbContext dbcontext)
    {
        string[] tiposDeporteNombres =["Baloncesto","Futbol","Tenis"];

        List<TipoDeporte> tiposDeporte = [];

    foreach (string tipoDeporteNombre in tiposDeporteNombres)
    {
        var tipoDeporte = dbcontext.TipoDeportes.FirstOrDefault(t => t.Nombre == tipoDeporteNombre);

        if (tipoDeporte == null)
            tiposDeporte.Add(new TipoDeporte(tipoDeporteNombre));
        else
            tiposDeporte.Add(tipoDeporte);
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(
        c => c.Descripcion == "Entrenamiento de baloncesto") == null)
    {
        var claseDeportiva = new ClaseDeportiva("Entrenamiento de baloncesto",tiposDeporte[0],new DateTime(2026, 10, 5, 18, 0, 0),"Pista 1","Carlos","Iniciacion",20,8.00m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(c => c.Descripcion == "Entrenamiento de futbol") == null)
    {
        var claseDeportiva = new ClaseDeportiva("Entrenamiento de futbol",tiposDeporte[1],new DateTime(2026, 10, 6, 19, 0, 0),"Pista 2","Laura","Intermedio",15,10.00m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);
    }

    if (dbcontext.ClasesDeportivas.FirstOrDefault(
        c => c.Descripcion == "Clase de tenis") == null)
    {
        var claseDeportiva = new ClaseDeportiva("Clase de tenis",tiposDeporte[2],new DateTime(2026, 10, 7, 17, 30, 0),null,"Miguel","Avanzado",10,12.50m);

        dbcontext.ClasesDeportivas.Add(claseDeportiva);
    }
    //guarda  las modificaciones de dbcontext en database
    dbcontext.SaveChanges();
}
//***********************************************
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
        var pista= new Pista("Pista de baloncesto",12.00,20,1);

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
//************************************

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


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





    }
}