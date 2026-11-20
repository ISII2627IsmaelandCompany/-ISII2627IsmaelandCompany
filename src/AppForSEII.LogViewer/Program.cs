
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
