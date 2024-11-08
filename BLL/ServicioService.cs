using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALL;
using ENTIDADES;

namespace BLL
{
    public class ServicioService
    {
        private readonly ServicioRepository servicioRepository;

        public ServicioService()
        {
            servicioRepository = new ServicioRepository();
        }

        public string AgregarServicio(Servicio servicio)
        {
            if (servicio == null)
                return "El servicio no puede ser nulo.";

            if (string.IsNullOrEmpty(servicio.Descripcion))
                return "La descripción del servicio es obligatoria.";

            if (servicio.PrecioManoObra <= 0)
                return "El precio de la mano de obra debe ser mayor a cero.";

            return servicioRepository.Insertar(servicio);
        }

        public List<Servicio> ObtenerServicios()
        {
            return servicioRepository.ObtenerTodos();
        }

        public Servicio ObtenerServicioPorId(int id)
        {
            return servicioRepository.ObtenerPorId(id);
        }

        // Implementa métodos para actualizar y eliminar servicios
    }

}
