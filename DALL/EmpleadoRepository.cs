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
        public EmpleadoRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(Empleado empleado)
        {
            string ssql = @"
        INSERT INTO Usuarios (NombreUsuario, Contraseña, Rol, CorreoElectronico)
        VALUES (@NombreUsuario, @Contraseña, @Rol, @CorreoElectronico);

        INSERT INTO Empleados (Id, NombreCompleto, PorcentajeComision)
        VALUES (SCOPE_IDENTITY(), @NombreCompleto, @PorcentajeComision);
    ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@NombreUsuario", empleado.NombreUsuario);
            cmd.Parameters.AddWithValue("@Contraseña", empleado.Contraseña);
            cmd.Parameters.AddWithValue("@Rol", empleado.Rol.ToString());
            cmd.Parameters.AddWithValue("@CorreoElectronico", empleado.CorreoElectronico);
            cmd.Parameters.AddWithValue("@NombreCompleto", empleado.NombreCompleto);
            cmd.Parameters.AddWithValue("@PorcentajeComision", empleado.PorcentajeComision);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se agregó el empleado {empleado.NombreCompleto}";
            else
                return "No se pudo agregar el empleado";
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
            string ssql = @"
        UPDATE Usuarios SET 
            Contraseña = @Contraseña, 
            CorreoElectronico = @CorreoElectronico
        WHERE Id = @Id;

        UPDATE Empleados SET 
            NombreCompleto = @NombreCompleto, 
            PorcentajeComision = @PorcentajeComision
        WHERE Id = @Id;
    ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", empleado.Id);
            cmd.Parameters.AddWithValue("@Contraseña", empleado.Contraseña ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CorreoElectronico", empleado.CorreoElectronico);
            cmd.Parameters.AddWithValue("@NombreCompleto", empleado.NombreCompleto);
            cmd.Parameters.AddWithValue("@PorcentajeComision", empleado.PorcentajeComision);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se actualizó el empleado {empleado.NombreCompleto}";
            else
                return "No se pudo actualizar el empleado";
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


    }

}
