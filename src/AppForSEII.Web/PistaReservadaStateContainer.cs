using AppForSEII.Web.API;// Permite utilizar los DTOs generados para comunicarnos con la API

namespace AppForSEII.Web
{
  public class PistaReservadaStateContainer// Clase que almacena y gestiona el estado de la reserva
    {
        // Reserva que estamos preparando
    public PistaReservadaForCreateDTO Reserva { get; private set; }// Permite consultar la reserva, pero solo esta clase puede sustituirla
        = new PistaReservadaForCreateDTO()// Creamos una reserva nueva y vacía
    {
        PistasReservadas = new List<PistaReservadaItemDTO>()// Inicializamos la lista de pistas reservadas
    };

    // Guardamos los datos originales de las pistas seleccionadas
    private readonly Dictionary<int, PistaDTO> pistasSeleccionadas = new();// Diccionario que relaciona el ID de cada pista con sus datos originales

    public IReadOnlyDictionary<int, PistaDTO> PistasSeleccionadas => pistasSeleccionadas;// Permite consultar las pistas seleccionadas sin modificarlas directamente


    // Precio total del carrito
    public double PrecioTotal// Propiedad que devuelve el precio total de las pistas seleccionadas
    {
        get// Se ejecuta cuando consultamos el precio total
        {
        return Reserva.PistasReservadas.Sum(p => p.Precio);// Suma el precio de todas las pistas del carrito
        }
    }
    // Notifica los cambios del carrito
    public event Action? OnChange;// Evento al que se suscriben los componentes para recibir avisos de cambios

    private void NotifyStateChanged()// Método privado que notifica que el estado del carrito ha cambiado
    {
        OnChange?.Invoke();// Ejecuta el evento si hay algún componente suscrito
    }

    // Añadir una pista al carrito
    public void AgregarPista(PistaDTO pista)
    {
        // Evitar añadir pistas sin stock
        if (pista.Stock <= 0)
        {
            return;
        }

        // Evitar añadir dos veces la misma pista
        if (Reserva.PistasReservadas.Any(
            p => p.IdPista == pista.IdPista))
        {
            return;
        }

        Reserva.PistasReservadas.Add(
            new PistaReservadaItemDTO()
            {
                IdPista = pista.IdPista,
                NombrePista = pista.NombrePista,
                TipoDeporte = pista.TipoDeporte,
                Precio = pista.Precio,
                Cantidad = 1,
                Observaciones = null
            });

        pistasSeleccionadas[pista.IdPista] = pista;

        NotifyStateChanged();
    }

    // Eliminar una pista del carrito
    public void EliminarPista(int idPista)
    {
        var pista = Reserva.PistasReservadas.FirstOrDefault(//busca la pista en el carrito por su identificador
            p => p.IdPista == idPista);

        if (pista != null)//comprueba que se haya encontrado.
        {
            Reserva.PistasReservadas.Remove(pista);//elimina la pista de la reserva
            pistasSeleccionadas.Remove(idPista);//también la elimina del diccionario.

            NotifyStateChanged();//avisa a la interfaz para que actualice el carrito y el precio total.
        }
    }

    // Actualizar la cantidad de personas para una pista
    public void ActualizarCantidad(PistaDTO pista, int cantidad)
    {
        // Buscamos la pista en el carrito mediante su identificador
        var item = Reserva.PistasReservadas.FirstOrDefault(
            p => p.IdPista == pista.IdPista);

        // Comprobamos que la pista esté en el carrito
        // y que la cantidad sea como mínimo 1 y no supere el aforo
        if (item == null ||
            cantidad < 1 ||
            cantidad > pista.NPersonas)
        {
            return;
        }

        // Actualizamos la cantidad de personas de la pista
        item.Cantidad = cantidad;

        // Notificamos a la interfaz de que el carrito ha cambiado
        NotifyStateChanged();
    }

    // Vaciar el carrito
    public void VaciarCarrito()
    {
        // Comprobamos si hay pistas en el carrito
        if (Reserva.PistasReservadas.Any())
        {
            // Eliminamos todas las pistas de la reserva
            Reserva.PistasReservadas.Clear();

            // Eliminamos todas las pistas del diccionario de pistas seleccionadas
            pistasSeleccionadas.Clear();

            // Notificamos a la interfaz de que el carrito ha cambiado
            NotifyStateChanged();
        }
    }

    // Limpiar la reserva después de procesarla
    public void ReservaProcesada()
    {
        // Creamos una nueva reserva vacía
        Reserva = new PistaReservadaForCreateDTO()
        {
            PistasReservadas = new List<PistaReservadaItemDTO>()
        };

        // Eliminamos todas las pistas del diccionario de pistas seleccionadas
        pistasSeleccionadas.Clear();

        // Notificamos a la interfaz de que el carrito ha cambiado
        NotifyStateChanged();
    }
  }
  
}