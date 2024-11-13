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
        private readonly UsuarioRepository usuarioRepository;

        public EmpleadoService()
        {
            empleadoRepository = new EmpleadoRepository();
            usuarioRepository = new UsuarioRepository();
        }

        public string RegistrarEmpleado(Empleado empleado)
        {
            if (empleado == null)
                return "El empleado no puede ser nulo.";

            // Validaciones de campos obligatorios
            if (string.IsNullOrWhiteSpace(empleado.NombreUsuario))
                return "El nombre de usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(empleado.Contraseña))
                return "La contraseña es obligatoria.";

            if (string.IsNullOrWhiteSpace(empleado.CorreoElectronico))
                return "El correo electrónico es obligatorio.";

            if (string.IsNullOrWhiteSpace(empleado.NombreCompleto))
                return "El nombre completo es obligatorio.";

            // Validar que no exista un empleado con el mismo NombreCompleto
            if (empleadoRepository.ExisteEmpleadoPorNombreCompleto(empleado.NombreCompleto))
                return "Ya existe un empleado con ese nombre completo.";

            // Validar que no exista el NombreUsuario y CorreoElectronico en Usuarios
            if (usuarioRepository.ExisteUsuarioPorNombreUsuario(empleado.NombreUsuario))
                return "El nombre de usuario ya existe.";

            if (usuarioRepository.ExisteUsuarioPorCorreo(empleado.CorreoElectronico))
                return "El correo electrónico ya está en uso.";

            try
            {
                // Insertar en Usuarios y obtener el Id
                int idUsuario = usuarioRepository.Insertar(empleado);

                // Asignar el Id al empleado
                empleado.Id = idUsuario;

                // Insertar en Empleados
                string resultadoEmpleado = empleadoRepository.Insertar(empleado);
                return resultadoEmpleado;
            }
            catch (Exception ex)
            {
                return $"Error al registrar el empleado: {ex.Message}";
            }
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