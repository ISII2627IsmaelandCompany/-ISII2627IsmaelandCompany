 using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace AppForSEII.LogViewer;
//la clase Subscriber que contendrá toda la lógica necesaria para conectar con el bróker de mensajería, abrir un canal y permanecer a la espera de nuevos logs sobre el 
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

    public Subscriber(//Constructor que recibe los datos necesarios para conectarse a RabbitMQ y los guarda en los atributos de la clase
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
    public async Task ConnectAsync()//Método asíncrono que establece la conexión con RabbitMQ  y configura el suscriptor para empezar a recibir mensajes
{
    // Configuramos los datos necesarios para establecer la conexión con RabbitMQ
    var factory = new ConnectionFactory
    {
        HostName = _hostName,
        Port = _port,
        UserName = _userName,
        Password = _password
    };

    // Establecemos la conexión con el servidor RabbitMQ
    _connection = await factory.CreateConnectionAsync();

    // Creamos un canal de comunicación
    _channel = await _connection.CreateChannelAsync();

    // Declaramos el exchange fanout (distribuye cada mensaje a todas las colas vinculadas)
    await _channel.ExchangeDeclareAsync(
        exchange: _exchangeName,
        type: ExchangeType.Fanout,
        durable: true,
        autoDelete: false //evita que se elimine automáticamente al quedarse sin usuario
    );
    // Creamos una cola temporal exclusiva para este suscriptor
var tempQueue = await _channel.QueueDeclareAsync(
    queue: "", //RabbitMQ genere automáticamente el nombre
    durable: false,//la cola no sobrevive a un reinicio del bróker
    exclusive: true, //solo esta conexión pueda utilizarla
    autoDelete: true//permite que se elimine cuando el suscriptor se desconecte
//los logs publicados cuando el suscriptor está desconectado no se guardan para que los reciba después.
);

// Recuperamos el nombre generado por RabbitMQ
var queueName = tempQueue.QueueName;

// Vinculamos la cola temporal al exchange de logs, para que  los mensajes publicados en el exchange Fanout lleguen a la cola
await _channel.QueueBindAsync(
    queue: queueName,
    exchange: _exchangeName,
    routingKey: string.Empty//vacía porque el exchange Fanout no utiliza claves de enrutamiento
);
// Creamos un consumidor que recibirá los mensajes de forma asíncrona
var consumer = new AsyncEventingBasicConsumer(_channel);

// Evento que se ejecuta cada vez que recibimos un mensaje. subscripcion al evento ReceivedAsync
consumer.ReceivedAsync += async (sender, args) =>
{
    // Convertimos los bytes recibidos a texto
    var message = Encoding.UTF8.GetString(args.Body.ToArray());// Convertir el contenido del mensaje que llega en bytes a una cadena de texto

    // Interpretamos el mensaje JSON
    using var document = JsonDocument.Parse(message);
    var log = document.RootElement;

    // Extraemos la información del log
    var timestamp = log.GetProperty("Timestamp").GetString();
    var level = log.GetProperty("LogLevel").GetString();
    var category = log.GetProperty("Category").GetString();
    var text = log.GetProperty("Message").GetString();

    // Mostramos el log por consola la información del registro recibido
    Console.WriteLine($"[{timestamp}] [{level}] [{category}] {text}");

    await Task.CompletedTask;
};

// Activamos el consumidor para escuchar nuestra cola
await _channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,//RabbitMQ confirma automáticamente la recepción del mensaje
    consumer: consumer
);

//informar que el suscriptor ya está conectado y escuchando
Console.WriteLine("Suscriptor conectado. Esperando logs...");
}


}