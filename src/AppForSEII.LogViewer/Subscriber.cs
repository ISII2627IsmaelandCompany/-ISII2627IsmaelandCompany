using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace AppForSEII.LogViewer;

public class Subscriber
{
    // Configuración necesaria para conectarnos a RabbitMQ
    private readonly string _hostName;
    private readonly int _port;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _exchangeName;

    // Conexión y canal de comunicación
    private IConnection? _connection;
    private IChannel? _channel;

    public Subscriber(
        string hostName,
        int port,
        string userName,
        string password,
        string exchangeName)
    {
        _hostName = hostName;
        _port = port;
        _userName = userName;
        _password = password;
        _exchangeName = exchangeName;
    }
    public async Task ConnectAsync()
{
    // Configuramos la conexión con RabbitMQ
    var factory = new ConnectionFactory
    {
        HostName = _hostName,
        Port = _port,
        UserName = _userName,
        Password = _password
    };

    // Establecemos la conexión con el servidor
    _connection = await factory.CreateConnectionAsync();

    // Creamos un canal de comunicación
    _channel = await _connection.CreateChannelAsync();

    // Declaramos el exchange fanout
    await _channel.ExchangeDeclareAsync(
        exchange: _exchangeName,
        type: ExchangeType.Fanout,
        durable: true,
        autoDelete: false
    );
    // Creamos una cola temporal exclusiva para este suscriptor
var tempQueue = await _channel.QueueDeclareAsync(
    queue: "",
    durable: false,
    exclusive: true,
    autoDelete: true
);

// Recuperamos el nombre generado por RabbitMQ
var queueName = tempQueue.QueueName;

// Vinculamos la cola al exchange de logs
await _channel.QueueBindAsync(
    queue: queueName,
    exchange: _exchangeName,
    routingKey: string.Empty
);
// Creamos un consumidor que recibirá los mensajes de forma asíncrona
var consumer = new AsyncEventingBasicConsumer(_channel);

// Evento que se ejecuta cada vez que recibimos un mensaje
consumer.ReceivedAsync += async (sender, args) =>
{
    // Convertimos los bytes recibidos a texto
    var message = Encoding.UTF8.GetString(args.Body.ToArray());

    // Interpretamos el mensaje JSON
    using var document = JsonDocument.Parse(message);
    var log = document.RootElement;

    // Extraemos la información del log
    var timestamp = log.GetProperty("Timestamp").GetString();
    var level = log.GetProperty("LogLevel").GetString();
    var category = log.GetProperty("Category").GetString();
    var text = log.GetProperty("Message").GetString();

    // Mostramos el log por consola
    Console.WriteLine($"[{timestamp}] [{level}] [{category}] {text}");

    await Task.CompletedTask;
};

// Activamos el consumidor para escuchar nuestra cola
await _channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Suscriptor conectado. Esperando logs...");
}


}