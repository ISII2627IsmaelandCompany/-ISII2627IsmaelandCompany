using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;


namespace AppForSEII.API.Logging;

//se conecta a RabbitMQ
public class RabbitMQLogger : ILogger, IDisposable //La declaración de la clase. ILogger: indica que nuestra clase implementa el sistema estándar de logs de .NET. - IDisposable: permite liberar los recursos utilizados cuando terminamos de trabajar con RabbitMQ.
{
    private readonly string _name;//Nombre de la categoría del logger.
    private readonly RabbitMQLoggerConfiguration _config;//Configuración de RabbitMQ.
    private IConnection _connection;//Conexión con el servidor RabbitMQ.
    private IChannel _channel;//Canal utilizado para comunicarse con RabbitMQ.
    private BasicProperties _properties;//Propiedades que tendrán los mensajes enviados.
    public async Task InitializateAsync()//Este método prepara la conexión con RabbitMQ.
    {
        var factory = new ConnectionFactory//fábrica que sabe cómo conectarse al servidor RabbitMQ.
        {
            HostName = _config.HostName,//dirección del servidor.
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password
        };
        _connection = await factory.CreateConnectionAsync();//Creamos la conexión con RabbitMQ.
        _channel = await _connection.CreateChannelAsync();//Creamos un canal dentro de esa conexión.

       await _channel.ExchangeDeclareAsync(exchange: _config.Exchange,
        type: _config.ExchangeType,
        durable: _config.Durable);
/*declaramos el exchange.
Un exchange es el componente de RabbitMQ que recibe los mensajes del productor y decide a qué colas enviarlos.
un exchange de tipo fanout. significa que distribuye una copia del mensaje a cada cola que esté vinculada a él
*/

    }
    public RabbitMQLogger(string name, RabbitMQLoggerConfiguration config)//cosntructor, se ejecuta al crear un objeto RabbitMQLogger 
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));//nombre del logger.
        _config = config ?? throw new ArgumentNullException(nameof(config));//configuración de RabbitMQ.
//El operador ?? comprueba si el valor es null. Si lo es, lanza una excepción para indicar que falta un dato obligatorio.

        ValidateConfiguration(_config);//comprueba que la configuración sea correcta.

        _properties = new BasicProperties//Creamos las propiedades que acompañarán a los mensajes
        {
            Persistent = _config.Durable,//indica si el mensaje se marca como persistente.
            ContentType = "application/json"//indica que el contenido del mensaje es JSON.
        };
           InitializateAsync().GetAwaiter().GetResult();//ejecuta el método asíncrono de inicialización y espera a que termine.
    }

    private static void ValidateConfiguration(RabbitMQLoggerConfiguration config)
    {
        if (string.IsNullOrEmpty(config.HostName))//comprueba si HostName está vacío o es nulo
            throw new ArgumentException("RabbitMQ HostName is required", nameof(config));
        if (config.Port <= 0)//port debe ser mayor que cero.
            throw new ArgumentException("RabbitMQ Port must be greater than 0", nameof(config));
        if (string.IsNullOrEmpty(config.UserName))//no puede estar vacío.
            throw new ArgumentException("RabbitMQ UserName is required", nameof(config));
        if (string.IsNullOrEmpty(config.Password))//no puede estar vacío.
            throw new ArgumentException("RabbitMQ Password is required", nameof(config));
        if (string.IsNullOrEmpty(config.Exchange))//debe tener nombre
            throw new ArgumentException("RabbitMQ Exchange is required", nameof(config));
        if (string.IsNullOrEmpty(config.ExchangeType))//debe tener tipo.
            throw new ArgumentException("RabbitMQ ExchangeType is required", nameof(config));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;//Permite trabajar con ámbitos de registro de logs.

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;//Comprueba si el nivel del log está habilitado

    public void Log<TState>(//este metodo se ejecuta cuando la aplicación quiere registrar un log.
        LogLevel logLevel,
        EventId eventId,
        TState state,//Información asociada al mensaje.
        Exception? exception,//Excepción, si se ha producido alguna.
        Func<TState, Exception?, string> formatter)//Función que convierte el estado en texto.
    {
        if (!IsEnabled(logLevel))//Si el nivel no está habilitado, no hace nada y termina el método.
            return;

        try
        {//si el nivel si esta habilitado, construye el mensaje:
            var logEntry = new
            {//Aquí crea un objeto con toda la información del log.
                Timestamp = DateTime.UtcNow,
                LogLevel = logLevel.ToString(),
                Category = _name,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            };
            var message = JsonSerializer.Serialize(logEntry);//Convierte el objeto en una cadena JSON
            var body = Encoding.UTF8.GetBytes(message);//Convierte esa cadena JSON en bytes

            _channel.BasicPublishAsync(//se envía el mensaje
            exchange: _config.Exchange, //exchange al que enviamos el mensaje.
            routingKey: "", //clave de enrutamiento. En fanout se ignora.
            mandatory: false, //indica si RabbitMQ debe devolver el mensaje cuando no pueda enrutarlo
            basicProperties: _properties, //propiedades del mensaje
            body: body).GetAwaiter().GetResult();//contenido del mensaje en bytes.


        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error publishing log message to RabbitMQ: {ex.Message}");
        }
    }

    public void Dispose()//Este método cierra y libera los recursos utilizados.
    {
        try
        {//primer cierra el canal y despues la conexion
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _channel?.Dispose();
            _connection?.CloseAsync().GetAwaiter().GetResult();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error disposing RabbitMQ logger: {ex.Message}");
        }
        GC.SuppressFinalize(this);
    }
}