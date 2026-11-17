
using AppForSEII.Web.API;

namespace AppForSEII.Web
{
    public class InscripcionClaseDeportivaStateContainer
    {
        // Inscripción que estamos preparando
        public InscripcionClaseDeportivaForCreateDTO Inscripcion { get; private set; }
            = new InscripcionClaseDeportivaForCreateDTO()
            {
                ClasesInscritas = new List<InscripcionClaseDeportivaItemDTO>()
            };

        // Guardamos los datos originales de las clases seleccionadas
        private readonly Dictionary<int, ClaseDeportivaForInscripcionDTO>
            clasesSeleccionadas = new();

        public IReadOnlyDictionary<int, ClaseDeportivaForInscripcionDTO>
            ClasesSeleccionadas => clasesSeleccionadas;

        // Precio total del carrito
        public double PrecioTotal
        {
            get
            {
                return Inscripcion.ClasesInscritas.Sum(ci => ci.Precio);
            }
        }

        // Notifica los cambios del carrito
        public event Action? OnChange;

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        // Añadir una clase deportiva al carrito
        public void AgregarClase(ClaseDeportivaForInscripcionDTO clase)
        {
            // No permitir clases sin plazas
            if (clase.PlazasDisponibles <= 0)
            {
                return;
            }

            // Evitar añadir dos veces la misma clase
            if (Inscripcion.ClasesInscritas.Any(
                ci => ci.ClaseDeportivaId == clase.Id))
            {
                return;
            }

            Inscripcion.ClasesInscritas.Add(
                new InscripcionClaseDeportivaItemDTO()
                {
                    ClaseDeportivaId = clase.Id,
                    Descripcion = clase.Descripcion,
                    TipoDeporte = clase.TipoDeporte,
                    FechaHora = clase.FechaHora,
                    PlazasReservadas = 1,
                    Precio = clase.PrecioUnitario,
                    Observaciones = null
                });

            clasesSeleccionadas[clase.Id] = clase;

            NotifyStateChanged();
        }

        // Eliminar una clase deportiva del carrito
        public void EliminarClase(int claseDeportivaId)
        {
            var clase = Inscripcion.ClasesInscritas.FirstOrDefault(
                ci => ci.ClaseDeportivaId == claseDeportivaId);

            if (clase != null)
            {
                Inscripcion.ClasesInscritas.Remove(clase);
                clasesSeleccionadas.Remove(claseDeportivaId);

                NotifyStateChanged();
            }
        }

        // Actualizar las plazas reservadas para una clase
        public void ActualizarPlazas(
            ClaseDeportivaForInscripcionDTO clase,
            int plazas)
        {
            var item = Inscripcion.ClasesInscritas.FirstOrDefault(
                ci => ci.ClaseDeportivaId == clase.Id);

            if (item == null ||
                plazas < 1 ||
                plazas > 3 ||
                plazas > clase.PlazasDisponibles)
            {
                return;
            }

            item.PlazasReservadas = plazas;

            // Recalcular el precio de esta clase
            item.Precio = clase.PrecioUnitario * plazas;

            NotifyStateChanged();
        }

        // Vaciar el carrito
        public void VaciarCarrito()
        {
            if (Inscripcion.ClasesInscritas.Any())
            {
                Inscripcion.ClasesInscritas.Clear();
                clasesSeleccionadas.Clear();

                NotifyStateChanged();
            }
        }

        // Limpiar la inscripción después de procesarla
        public void InscripcionProcesada()
        {
            Inscripcion = new InscripcionClaseDeportivaForCreateDTO()
            {
                ClasesInscritas = new List<InscripcionClaseDeportivaItemDTO>()
            };

            clasesSeleccionadas.Clear();

            NotifyStateChanged();
        }
    }
}
