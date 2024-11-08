using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class UsuarioRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        public UsuarioRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(Usuario usuario)
        {
            string ssql = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Rol, CorreoElectronico) " +
                          "VALUES (@NombreUsuario, @Contraseña, @Rol, @CorreoElectronico)";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
            cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
            cmd.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());
            cmd.Parameters.AddWithValue("@CorreoElectronico", usuario.CorreoElectronico);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se agregó el usuario {usuario.NombreUsuario}";
            else
                return "No se pudo agregar el usuario";
        }

        public Usuario ObtenerPorId(int id)
        {
            string ssql = "SELECT * FROM Usuarios WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Usuario usuario = null;
            if (reader.Read())
            {
                usuario = Mapper(reader);
            }
            _connection.Close();
            return usuario;
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = new List<Usuario>();
            string ssql = "SELECT * FROM Usuarios";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(Mapper(reader));
            }
            _connection.Close();
            return usuarios;
        }

        private Usuario Mapper(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                Contraseña = reader.GetString(reader.GetOrdinal("Contraseña")),
                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader.GetString(reader.GetOrdinal("Rol"))),
                CorreoElectronico = reader.GetString(reader.GetOrdinal("CorreoElectronico"))
            };
        }

        // Implementa métodos para Actualizar y Eliminar si es necesario
    }

}