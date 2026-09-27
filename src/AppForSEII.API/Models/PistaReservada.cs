public class PistaReservada
{

    public PistaReservada()
    {
        
    }
    public PistaReservada(int IdPista, int IdReserva)
    {
        ID = IdPista;
        this.IdReserva = IdReserva;
    }
    
[Key]
    public int ID {get; set;}

    public int IdPista { get; set; }
    public int Cantidad { get; set; }
    public int IdReserva{get; set;}
    public string? Observaciones {get; set;}
    public double Precio {get; set;}
}