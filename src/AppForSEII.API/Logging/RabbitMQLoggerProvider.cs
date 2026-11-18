using Microsoft.Extensions.Logging;//para utilizar las interfaces y clases del sistema de logs de .NET
using Microsoft.Extensions.Options;//para utilizar IOptions para recibir la configuración

namespace AppForSEII.API.Logging;

[ProviderAlias("RabbitMQ")]//para indicar/identificar que el proveedor tiene el alias de "RabbitMQ"

//esta clase es el proveedor que crea y administra los loggers, cuando otra parte de la API quiere registrar un acontecimiento este proveedor le proporciona un RabbitMQLogger
public class RabbitMQLoggerProvider : ILoggerProvider//Crear el proveedor de logs de RabbitMQ. ILoggerProvider indica que la clase implementa la interfaz de ILoggerProvider
{
    private readonly RabbitMQLoggerConfiguration _config;//para guardar la configuración de RabbitMQ(guarda los datos de la conexion y la configuración)
    private readonly Dictionary<string, RabbitMQLogger> _loggers = new(); //este es el dicionario que almacena los loggers que se crean (puede guardar un logger de un controlador y demas)
    private readonly Lock _lock = new Lock();//para controlar el acceso simultáneo al diccionario(para que varios hilos no modifiquen el diccionario a la vez)

    public RabbitMQLoggerProvider(IOptions<RabbitMQLoggerConfiguration> config)//es el constructor que recibe la configuracion del RabbitMQ
    {
        _config = config.Value;//eel config.value tiene los valores de la configuración
    }
    public ILogger CreateLogger(string categoryName)// crea o recupera un logger de una categoria
    {
        lock (_lock)//Bloqueamos el acceso al diccionario
        {
            if (!_loggers.TryGetValue(categoryName, out var logger))//comprueba si ya existe un logger de esa categoría. El out var logger es una variable donde se guardará el logger encontrado, si existe
            {
                logger = new RabbitMQLogger(categoryName, _config);//// Si no existe se crea un logger nuevo.
                _loggers[categoryName] = logger;//guardar el logger en el diccionario
            }

            return logger;//devuelve el logger creado
        }
    }

    public void Dispose()//este metodo libera los recursos utilizados por el proveedor
    {
        foreach (var logger in _loggers.Values)//se recorre todos los loggers que hay guardados.
        {
            logger.Dispose();//Libera los recursos de cada logger
        }

        _loggers.Clear();//Vacía el diccionario
    }
}