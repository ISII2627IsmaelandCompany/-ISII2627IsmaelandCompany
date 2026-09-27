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
}