using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppForSEII.API.Logging;

// Esta clase permite registrar(el proovedor) nuestro logger de RabbitMQ
// en el sistema de logging de .NET.
public static class RabbitMQLoggerExtensions//static = no necesita crear objetos. proporciona funcionalidades a otras clases
{
    public static ILoggingBuilder AddRabbitMQ(//ILoggingBuilder es el tipo de objeto que devuelve el método
        this ILoggingBuilder builder,//indica que es un método de extensión de ILoggingBuilder(permite añadir una funcionalidad a una clase sin modificarla)
        IConfigurationSection config)//recibe la sección de configuración de RabbitMQ
    {
        builder.Services.Configure<RabbitMQLoggerConfiguration>(config);//Esta instrucción(.Configure) conecta la configuración del JSON con la clase RabbitMQLoggerConfiguration
        builder.Services.AddSingleton<ILoggerProvider, RabbitMQLoggerProvider>();//registra el RabbitMQLoggerProvider en el sistema de inyección de dependencias de el .NET.
        //Singleton:solo se mantiene una instancia de ese proveedor durante la vida de la aplicación(como el main). Tambien está scoped que es para una instancia por cada petición
        //el .services es para dejarlo registrado
        return builder;//Devuelve el objeto ILoggingBuilder
    }
    //lo usamos en el program.cs: builder.Logging.AddRabbitMQ(builder.Configuration.GetSection("RabbitMQ"));
}