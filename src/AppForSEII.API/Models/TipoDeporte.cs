public class TipoDeporte
{

    public TipoDeporte()
    {
        
    }
    public TipoDeporte(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }
    
[Key]
    public int Id { get; set; }

    public string Nombre { get; set; }
    public List<string> Materiales{get; set;}
    public List<string> Competiciones{get; set;}
    public string  NombreTipoDeporte{get; set;}


    //Atributo necesario para el CU4,id y nombre ya estan implementados
     public string? Descripcion { get; set; }//descripcion puede ser null

    //relacion con ClaseDeportiva da error cuando este en development cambiar
    //public List<ClaseDeportiva> ClasesDeportivas { get; set; } = new List<ClaseDeportiva>();
}