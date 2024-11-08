using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;
using DALL;

namespace BLL
{
    public class UsuarioService
    {
        private readonly UsuarioRepository usuarioRepository;

        public UsuarioService()
        {
            usuarioRepository = new UsuarioRepository();
        }

        public string RegistrarUsuario(Usuario usuario)
        {
            // Aquí puedes agregar validaciones o lógica de negocio adicional
            if (usuario == null)
                return "El usuario no puede ser nulo.";

            if (string.IsNullOrEmpty(usuario.NombreUsuario) || string.IsNullOrEmpty(usuario.Contraseña))
                return "El nombre de usuario y la contraseña son obligatorios.";

            // Verificar si el nombre de usuario ya existe
            var usuariosExistentes = usuarioRepository.ObtenerTodos();
            if (usuariosExistentes.Exists(u => u.NombreUsuario == usuario.NombreUsuario))
                return "El nombre de usuario ya existe.";

            return usuarioRepository.Insertar(usuario);
        }

        public Usuario IniciarSesion(string nombreUsuario, string contraseña)
        {
            var usuarios = usuarioRepository.ObtenerTodos();
            var usuario = usuarios.Find(u => u.NombreUsuario == nombreUsuario && u.Contraseña == contraseña);
            return usuario;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return usuarioRepository.ObtenerPorId(id);
        }


        // Implementa métodos adicionales según sea necesario
    }

}