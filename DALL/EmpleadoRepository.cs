using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class EmpleadoRepository
    {
        private readonly SqlConnection _connection;
        string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        private readonly UsuarioRepository usuarioRepository;

        public EmpleadoRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
            usuarioRepository = new UsuarioRepository();

        }

        public string Insertar(Empleado empleado)
        {
            try
            {
                string ssql = "INSERT INTO Empleados (Id, NombreCompleto, PorcentajeComision) VALUES (@Id, @NombreCompleto, @PorcentajeComision)";
                SqlCommand cmd = new SqlCommand(ssql, _connection);
                cmd.Parameters.AddWithValue("@Id", empleado.Id);
                cmd.Parameters.AddWithValue("@NombreCompleto", empleado.NombreCompleto);
                cmd.Parameters.AddWithValue("@PorcentajeComision", empleado.PorcentajeComision);

                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();

                return "Empleado registrado exitosamente.";
            }
            catch (SqlException ex)
            {
                _connection.Close();
                if (ex.Number == 2627) // Violación de restricción única
                {
                    return "Ya existe un empleado con ese nombre completo.";
                }
                else
                {
                    return $"Error al registrar el empleado: {ex.Message}";
                }
            }
        }



        public Empleado ObtenerPorId(int id)
        {
            string ssql = @"
        SELECT u.Id, u.NombreUsuario, u.Contraseña, u.Rol, u.CorreoElectronico,
               e.NombreCompleto, e.PorcentajeComision
        FROM Usuarios u
        INNER JOIN Empleados e ON u.Id = e.Id
        WHERE u.Id = @Id";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Empleado empleado = null;
            if (reader.Read())
            {
                empleado = Mapper(reader);
            }
            _connection.Close();
            return empleado;
        }

        public List<Empleado> ObtenerTodos()
        {
            List<Empleado> empleados = new List<Empleado>();
            string ssql = @"
        SELECT u.Id, u.NombreUsuario, u.Contraseña, u.Rol, u.CorreoElectronico,
               e.NombreCompleto, e.PorcentajeComision
        FROM Usuarios u
        INNER JOIN Empleados e ON u.Id = e.Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                empleados.Add(Mapper(reader));
            }
            _connection.Close();
            return empleados;
        }


        private Empleado Mapper(SqlDataReader reader)
        {
            return new Empleado
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                Contraseña = reader.GetString(reader.GetOrdinal("Contraseña")),
                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader.GetString(reader.GetOrdinal("Rol"))),
                CorreoElectronico = reader.GetString(reader.GetOrdinal("CorreoElectronico")),
                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                PorcentajeComision = reader.GetDecimal(reader.GetOrdinal("PorcentajeComision"))
            };
        }


        public string Actualizar(Empleado empleado)
        {
            try
            {
                // Actualizar datos en la tabla Usuarios
                string ssqlUsuario = "UPDATE Usuarios SET Contraseña = @Contraseña, CorreoElectronico = @CorreoElectronico WHERE Id = @Id";
                SqlCommand cmdUsuario = new SqlCommand(ssqlUsuario, _connection);
                cmdUsuario.Parameters.AddWithValue("@Id", empleado.Id);
                cmdUsuario.Parameters.AddWithValue("@Contraseña", empleado.Contraseña);
                cmdUsuario.Parameters.AddWithValue("@CorreoElectronico", empleado.CorreoElectronico);

                // Actualizar datos en la tabla Empleados
                string ssqlEmpleado = "UPDATE Empleados SET NombreCompleto = @NombreCompleto, PorcentajeComision = @PorcentajeComision WHERE Id = @Id";
                SqlCommand cmdEmpleado = new SqlCommand(ssqlEmpleado, _connection);
                cmdEmpleado.Parameters.AddWithValue("@Id", empleado.Id);
                cmdEmpleado.Parameters.AddWithValue("@NombreCompleto", empleado.NombreCompleto);
                cmdEmpleado.Parameters.AddWithValue("@PorcentajeComision", empleado.PorcentajeComision);

                _connection.Open();
                cmdUsuario.ExecuteNonQuery();
                cmdEmpleado.ExecuteNonQuery();
                _connection.Close();

                return "Empleado actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                _connection.Close();
                return $"Error al actualizar el empleado: {ex.Message}";
            }
        }

        public string Eliminar(int id)
        {
            string ssql = @"
        DELETE FROM Empleados WHERE Id = @Id;
        DELETE FROM Usuarios WHERE Id = @Id;
    ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return "Empleado eliminado correctamente";
            else
                return "No se pudo eliminar el empleado";
        }
        public bool ExisteEmpleadoPorNombreCompleto(string nombreCompleto)
        {
            string ssql = "SELECT COUNT(*) FROM Empleados WHERE NombreCompleto = @NombreCompleto";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);

            _connection.Open();
            int count = (int)cmd.ExecuteScalar();
            _connection.Close();

            return count > 0;
        }


    }

}
