using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using DALL;
namespace BLL
{
    public class SolicitudService
    {
        private SolicitudRepository solicitudRepository;

        public SolicitudService()
        {
            solicitudRepository = new SolicitudRepository();
        }

        public string CrearSolicitud(SolicitudServicio solicitud)
        {
            return solicitudRepository.Insertar(solicitud);
        }

        // Otros métodos si es necesario
        public List<SolicitudServicio> ObtenerSolicitudesPendientes()
        {
            return solicitudRepository.ObtenerSolicitudesPendientes();
        }

        public string ActualizarSolicitud(SolicitudServicio solicitud)
        {
            return solicitudRepository.Actualizar(solicitud);
        }
        public SolicitudServicio ObtenerSolicitudPorId(int id)
        {
            return solicitudRepository.ObtenerPorId(id);
        }

    }

}
