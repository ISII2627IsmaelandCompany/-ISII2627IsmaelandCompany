public class Inscripcion
{
    public Inscripcion()
    {
        
    }
    public Inscripcion(int id, string nombreUsuario, string apellidosUsuario, string dni, DateTime fechaInscripcion, string metodoPago, float precioTotal, string telefono)
    {
        Id = id;
        NombreUsuario = nombreUsuario;
        ApellidosUsuario = apellidosUsuario;
        DNI = dni;
        FechaInscripcion = fechaInscripcion;
        MetodoPago = metodoPago;
        PrecioTotal = precioTotal;
        Telefono = telefono;
    }

    public string ApellidosUsuario { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public int Id { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public float PrecioTotal { get; set; }
    public string Telefono { get; set; } = string.Empty;
}