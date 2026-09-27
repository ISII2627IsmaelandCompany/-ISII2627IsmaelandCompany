public class Reserva
{

    public Reserva()
    {
        
    }
    public Reserva(int Id)
    {
        ID = Id;
    }
    
[Key]
    public int Id {get; set;}

    public string NombreCliente { get; set; }
    public string Apellidos { get; set; }
    public string MetodoPago{get; set;}
    public double PrecioTotal {get; set;}
}