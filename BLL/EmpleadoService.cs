using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using DALL;

namespace BLL
{
    public class EmpleadoService
    {
        private readonly EmpleadoRepository empleadoRepository;

        public EmpleadoService()
        {
            empleadoRepository = new EmpleadoRepository();
        }

        public string RegistrarEmpleado(Empleado empleado)
        {
            // Validaciones específicas para empleados
            if (empleado == null)
                return "El empleado no puede ser nulo.";

            if (string.IsNullOrEmpty(empleado.NombreUsuario) || string.IsNullOrEmpty(empleado.Contraseña))
                return "El nombre de usuario y la contraseña son obligatorios.";

            if (string.IsNullOrEmpty(empleado.NombreCompleto))
                return "El nombre completo es obligatorio.";

            // Verificar si el nombre de usuario ya existe
            var empleadosExistentes = empleadoRepository.ObtenerTodos();
            if (empleadosExistentes.Exists(e => e.NombreUsuario == empleado.NombreUsuario))
                return "El nombre de usuario ya existe.";

            return empleadoRepository.Insertar(empleado);
        }

        public List<Empleado> ObtenerEmpleados()
        {
            return empleadoRepository.ObtenerTodos();
        }

        public Empleado ObtenerEmpleadoPorId(int id)
        {
            return empleadoRepository.ObtenerPorId(id);
        }


        public string ActualizarEmpleado(Empleado empleado)
        {
            // Validaciones adicionales si es necesario
            return empleadoRepository.Actualizar(empleado);
        }

        public string EliminarEmpleado(int id)
        {
            return empleadoRepository.Eliminar(id);
        }

    }

}