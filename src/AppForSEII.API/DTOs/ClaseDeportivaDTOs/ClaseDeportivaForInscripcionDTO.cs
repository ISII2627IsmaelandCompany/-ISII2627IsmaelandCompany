using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppForSEII.API.DTOs.ClaseDeportivaDTOs//namespace de  donde esta el dto de  clase deportiva
{   //dto  para devolver al cliente datos, util para clase deportiva e inscribirse
    public class ClaseDeportivaForInscripcionDTO
    {
        public ClaseDeportivaForInscripcionDTO()//constructor vacio para crear objetos sin  parametro
        {
        }

        public ClaseDeportivaForInscripcionDTO(int id, string descripcion, string tipoDeporte, DateTime fechaHora, string? lugar, string monitor, string nivel, int plazasDisponibles, decimal precioUnitario)
        {//constructor  con los datos que devolveremos en el get
            Id = id;
            Descripcion = descripcion;
            TipoDeporte = tipoDeporte;
            FechaHora = fechaHora;
            Lugar = lugar;
            Monitor = monitor;
            Nivel = nivel;
            PlazasDisponibles = plazasDisponibles;
            PrecioUnitario = precioUnitario;
        }

        public int Id { get; set; }//id de   la clase deportiva
        [Required]
    public string Descripcion { get; set; } = string.Empty;//descipcion de  la actividad

    public string TipoDeporte { get; set; } = string.Empty;//nombre deporte . ej: futbol
    [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime FechaHora { get; set; }//fecha y hora de la clase

    public string? Lugar { get; set; }//lugar donde se hace puede ser null(opcional)
    [Required]
    public string Monitor { get; set; } = string.Empty;//Monitor que imparte clase
    [Required]
    public string Nivel { get; set; } = string.Empty;//dificultad
    [Range(0, 30)]
    public int PlazasDisponibles { get; set; }//plazas disponibles
    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    [Range(0, 999.99)]
    public decimal PrecioUnitario { get; set; }//precio  clase
        
        // override object.Equals
        public override bool Equals(object? obj)
        {//comprueba si el objeto es recibido es otro DTO del mismo tipo
        //y si todas sus propiedades tienen el mismo valor si es asi devolvera true por lo contrario false

         return obj is ClaseDeportivaForInscripcionDTO dTO &&
            Id == dTO.Id &&
           Descripcion == dTO.Descripcion &&
           TipoDeporte == dTO.TipoDeporte &&
           FechaHora == dTO.FechaHora &&
           Lugar == dTO.Lugar &&
           Monitor == dTO.Monitor &&
           Nivel == dTO.Nivel &&
           PlazasDisponibles == dTO.PlazasDisponibles &&
           PrecioUnitario == dTO.PrecioUnitario;  
            }
        
        public override int GetHashCode()//hascode come el de appformovie, lo que hace es generar un numero a partir de los atributos del objeto
        {//para poder identificarlo y compararlo 
            return HashCode.Combine(HashCode.Combine(Id, Descripcion, TipoDeporte, FechaHora, Lugar, Monitor,Nivel,PlazasDisponibles),PrecioUnitario);//se tiene que poner de esta forma porque tiene 9 atributos y por cada hashcode combine solo admite 8
        }

    }

}