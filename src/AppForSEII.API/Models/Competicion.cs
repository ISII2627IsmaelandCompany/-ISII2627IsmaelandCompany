public class Competicion
{
    public Competicion()
    {
    }
    public Competicion(int id, string nombre, string lugar, DateTime fecha, int plazas, float precio)
    {
        Id = id;
        Nombre = nombre;
        Lugar = lugar;
        Fecha = fecha;
        Plazas = plazas;
        Precio = precio;
    }


    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Fecha { get; set; } 
    
    public int Id { get; set; }

    public string Lugar { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;
    [Range (0, int.MaxValue, ErrorMessage = "Las plazas deben ser mayor o igual a 0")]

    public int Plazas { get; set; }

    [Range (0, float.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
    public float Precio { get; set; }

}