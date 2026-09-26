//Para organizar donde se encuentra
namespace AppForSEII.API.Models;


public class ClaseDeportiva
{
    //atributos de la clase


    //Primarykey
    [Key]
    public int Id { get; set; }

    //comentado porque faltan clases y da error
    //public TipoDeporte TipoDeporte { get; set; } = null!; 
    //public List<ClaseInscrita> ClasesInscritas { get; set; } = new List<ClaseInscrita>();

    //IMP => falta averiguar lo de las foreign key
    public int TipoDeporteId { get; set; }


    
    [Required]//obligatorio
    
    public string Descripcion { get; set; }= string.Empty;//deja en vacio el huceo y no a null

    [Required]
    //IMP=> PREGUNTAR EL ERROR ESTE 
    //[DataType(DataType.Date), Display(Name ="Fecha y hora")] 
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime FechaHora { get; set; }
    public string? Lugar { get; set; } //lugar puede ser un atributo a null por la '?'

    [Required]
    public string Monitor { get; set; }= string.Empty;

    [Required]
    public string Nivel { get; set; }= string.Empty;
    [Range(0,30,ErrorMessage ="plazas disponibles entre 0 y 30")]//selecciona el rango para las plazas, dice max 30 enunciado
    public int PlazasDisponibles { get; set; }

    [Required]
    //Preguntar lo mismo del error
    //[DataType(DataType.Currency)] 
    //[Display(Name = "Precio Unitario")]
    [Precision(5, 2)]//5 digitos 2 decimales
    [Range(0,999.99,ErrorMessage ="precio no puede ser negativo y maximo 999.99")]//rango para que no se salga

    public decimal PrecioUnitario{get;set;}

    //constructores

    public ClaseDeportiva()
    {
        
    }

    public ClaseDeportiva( string descripcion, DateTime fechaHora, string? lugar, string monitor, string nivel, int plazasDisponibles, decimal precioUnitario)
    {
        
        this.Descripcion = descripcion;
        this.FechaHora = fechaHora;
        this.Lugar = lugar;
        this.Monitor = monitor;
        this.Nivel = nivel;
        this.PlazasDisponibles = plazasDisponibles;
        this.PrecioUnitario = precioUnitario;
    }
}