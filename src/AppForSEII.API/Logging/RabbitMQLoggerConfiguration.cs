namespace AppForSEII.API.Logging;


//Esta clase guarda la configuración necesaria para conectarse a RabbitMQ y declarar el exchange.
public class RabbitMQLoggerConfiguration
{
    public string HostName { get; set; } = null!;//el null! es para permitir que sea null porque nosotros le vamos a asignar el valor(en el .Json)
    public int Port { get; set; }
    public string UserName { get; set; } = null!;//Usuario de RabbitMQ (guest)
    public string Password { get; set; } = null!;//(guest)
    public string Exchange { get; set; } = null!;//Nombre del exchange
    public string ExchangeType { get; set; } = null!;//fanout
    public bool Durable { get; set; }//Indica si el exchange debe sobrevivir al reinicio del servidor
}