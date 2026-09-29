namespace AppForSEII.API.Models
{

public class TipoDeporte
{
    //atributos de la clase


    //Primarykey
    [Key]
    public int Id { get; set; }

    public Pista Pista { get; set; } = null!; //el null! es para mas adelante para la hora de hacer la relacion entre las clases

    public int IdPista { get; set; }// FK para relacionar TipoDeporte con Pista


    public string Nombre { get; set; }= string.Empty;//para no tener null

    [Required]
    public string NombreTipoDeporte { get; set; }= string.Empty;//para no tener null

    public string? Competiciones { get; set; }//para no tener null

    public string? Materiales { get; set; }= string.Empty;//para no tener null. quiza no necesitan material


//relacion con Pista
    



    //Atributo necesario para el CU4,id y nombre ya estan implementados
     public string? Descripcion { get; set; }//descripcion puede ser null

    //relacion con ClaseDeportiva  
    public List<ClaseDeportiva> ClasesDeportivas { get; set; } = new List<ClaseDeportiva>(); //relacion TipoDeporte 1 ---- N ClaseDeportiva
}
}