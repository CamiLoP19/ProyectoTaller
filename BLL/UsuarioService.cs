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
            if (usuario == null)
                return "El usuario no puede ser nulo.";

            if (string.IsNullOrEmpty(usuario.NombreUsuario) || string.IsNullOrEmpty(usuario.Contraseña))
                return "El nombre de usuario y la contraseña son obligatorios.";

            if (usuarioRepository.ExisteUsuarioPorNombreUsuario(usuario.NombreUsuario))
                return "El nombre de usuario ya existe.";

            try
            {
                int idUsuario = usuarioRepository.Insertar(usuario);
                if (idUsuario > 0)
                {
                    return "Usuario registrado exitosamente.";
                }
                else
                {
                    return "No se pudo registrar el usuario.";
                }
            }
            catch (Exception ex)
            {
                return $"Error al registrar el usuario: {ex.Message}";
            }
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