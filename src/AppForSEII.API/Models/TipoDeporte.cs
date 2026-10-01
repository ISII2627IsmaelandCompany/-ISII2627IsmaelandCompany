namespace AppForSEII.API.Models
{

public class TipoDeporte
{
    //atributos de la clase


    //Primarykey
    [Key]
    public int Id { get; set; }

    public List<Pista> Pistas { get; set; } = new List<Pista>(); //Relacion 1--N  

    public int IdPista { get; set; }// FK para relacionar TipoDeporte con Pista


    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string Nombre { get; set; }= string.Empty;//para no tener null

    [Required]
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.", MinimumLength = 1)]
    public string NombreTipoDeporte { get; set; }= string.Empty;//para no tener null

    public List<Competicion> Competiciones { get; set; } = new List<Competicion>();//Relacion 1--N

    public List<Material> Materiales { get; set; } = new List<Material>();




    //Atributo necesario para el CU4,id y nombre ya estan implementados
     public string? Descripcion { get; set; }//descripcion puede ser null

    //relacion con ClaseDeportiva  
    public List<ClaseDeportiva> ClasesDeportivas { get; set; } = new List<ClaseDeportiva>(); //relacion TipoDeporte 1 ---- N ClaseDeportiva
    //constructores 
    public TipoDeporte()
        {
            
        } 
        //constructor como el de genre del ejemplo github
    public TipoDeporte(string nombre) {
         this.Nombre = nombre; 
        }
}
}