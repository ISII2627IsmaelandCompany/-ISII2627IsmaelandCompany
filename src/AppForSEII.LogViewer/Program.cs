
using Microsoft.Extensions.Configuration;

namespace AppForSEII.LogViewer;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Leemos la configuración desde appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Recuperamos los datos de conexión de RabbitMQ
        var rabbitConfig = config.GetSection("RabbitMQ");

// Permite cambiar el servidor RabbitMQ mediante una variable de entorno
string host = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
              ?? rabbitConfig["HostName"]!;        int port = int.Parse(rabbitConfig["Port"]!);
        string user = rabbitConfig["UserName"]!;
        string password = rabbitConfig["Password"]!;
        string exchange = rabbitConfig["Exchange"]!;

        // Creamos nuestro suscriptor
        var subscriber = new Subscriber(
            host, port, user, password, exchange
        );

        // Nos conectamos a RabbitMQ y comenzamos a escuchar
        await subscriber.ConnectAsync();

       // Mantenemos el suscriptor activo para recibir mensajes
Console.WriteLine("LogViewer escuchando mensajes. Pulsa Ctrl+C para finalizar.");

await Task.Delay(Timeout.InfiniteTimeSpan);
    }
}
/*
1. ¿Qué pasa si el publisher comienza a publicar registros, pero no hay ningún
suscriptor conectado? ¿Se pierden los mensajes o se envían cuando uno o
varios suscriptores se conecten?
Si no hay ningún suscriptor conectado, los mensajes publicados por el publisher se perderán a menos que se configure 
la cola para que sea duradera y los mensajes sean persistentes. En ese caso, 
los mensajes se almacenarán en la cola y estarán disponibles para los suscriptores cuando se conecten.

2. ¿Si un suscriptor estaba conectado y recibiendo mensajes, pero pierde su
conexión y reconecta pasado un tiempo, recupera los mensajes enviados
durante ese periodo de desconexión?
Si el suscriptor pierde su conexión y luego se reconecta, no recuperará automáticamente los mensajes enviados durante
su desconexión a menos que la cola esté configurada para ser duradera y los mensajes sean persistentes. 
En ese caso, los mensajes permanecerán en la cola hasta que el suscriptor los consuma.

3. ¿Qué cambio podría realizarse para evitar pérdidas de mensajes por
desconexiones de suscriptores?
Se podría configurar la cola para que sea duradera y los mensajes sean persistentes. Además, se podrían implementar mecanismos
de confirmación de recepción de mensajes (acknowledgments) para asegurar que los mensajes se procesen correctamente antes de ser eliminados de la cola.

4. ¿Puede un suscriptor estar suscrito a más de un exchange?
Un suscriptor puede estar suscrito a más de un exchange, pero esto depende de cómo se configure la aplicación y la lógica de suscripción. 
En general, un suscriptor puede crear múltiples conexiones o canales para suscribirse a diferentes exchanges, o puede utilizar un patrón 
de enrutamiento que le permita recibir mensajes de varios exchanges a través de una sola cola.

5. Si el publisher se desconecta, ¿qué ocurre con las copias de los mensajes que los
suscriptores tienen en sus colas? ¿Qué ocurre con esas colas?
Si el publisher se desconecta, las copias de los mensajes que los suscriptores tienen en sus colas no se ven afectadas.
Los suscriptores seguirán teniendo acceso a los mensajes que ya han recibido y almacenado en
*/