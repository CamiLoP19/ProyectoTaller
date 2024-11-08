using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class SolicitudRepository
    {
        private readonly SqlConnection _connection;
        string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";

        public SolicitudRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(SolicitudServicio solicitud)
        {
            string ssql = @"
            INSERT INTO SolicitudesServicio (IdCliente, Descripcion, FechaSolicitud, FechaServicio, Estado)
            VALUES (@IdCliente, @Descripcion, @FechaSolicitud, @FechaServicio, @Estado);
        ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdCliente", solicitud.IdCliente);
            cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);
            cmd.Parameters.AddWithValue("@FechaSolicitud", solicitud.FechaSolicitud);
            cmd.Parameters.AddWithValue("@FechaServicio", solicitud.FechaServicio);
            cmd.Parameters.AddWithValue("@Estado", solicitud.Estado.ToString());

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return "Solicitud de servicio creada exitosamente.";
            else
                return "No se pudo crear la solicitud de servicio.";
        }

        // Otros métodos si es necesario
        public List<SolicitudServicio> ObtenerSolicitudesPendientes()
        {
            List<SolicitudServicio> solicitudes = new List<SolicitudServicio>();
            string ssql = @"
                        SELECT Id, IdCliente, Descripcion, FechaSolicitud, FechaServicio, Estado, IdEmpleado, PrecioEstimado
                        FROM SolicitudesServicio
                        WHERE Estado = @Estado";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Estado", EstadoSolicitud.Pendiente.ToString());

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                solicitudes.Add(MapearSolicitud(reader));
            }
            _connection.Close();
            return solicitudes;
        }
        public string Actualizar(SolicitudServicio solicitud)
        {
            string ssql = @"
        UPDATE SolicitudesServicio SET
            Descripcion = @Descripcion,
            FechaServicio = @FechaServicio,
            Estado = @Estado,
            IdEmpleado = @IdEmpleado
        WHERE Id = @Id;
    ";

            using (SqlCommand cmd = new SqlCommand(ssql, _connection))
            {
                cmd.Parameters.AddWithValue("@Id", solicitud.Id);
                cmd.Parameters.AddWithValue("@Descripcion", solicitud.Descripcion);
                cmd.Parameters.AddWithValue("@FechaServicio", solicitud.FechaServicio);
                cmd.Parameters.AddWithValue("@Estado", solicitud.Estado.ToString());

                // Agregar el parámetro @IdEmpleado correctamente
                if (solicitud.IdEmpleado.HasValue)
                {
                    cmd.Parameters.Add("@IdEmpleado", SqlDbType.Int).Value = solicitud.IdEmpleado.Value;
                }
                else
                {
                    cmd.Parameters.Add("@IdEmpleado", SqlDbType.Int).Value = DBNull.Value;
                }

                try
                {
                    _connection.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();
                    return filasAfectadas > 0 ? "Solicitud actualizada correctamente." : "No se pudo actualizar la solicitud.";
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    return $"Error al actualizar la solicitud: {ex.Message}";
                }
                finally
                {
                    _connection.Close();
                }
            }
        }


        public SolicitudServicio ObtenerPorId(int id)
        {

            SolicitudServicio solicitud = null;
            string ssql = "SELECT * FROM SolicitudesServicio WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            try
            {
                _connection.Open();
                Console.WriteLine($"Ejecutando consulta: {ssql} con Id = {id}");
    
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    solicitud = MapearSolicitud(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la solicitud: {ex.Message}");
            }
            finally
            {
                _connection.Close();
            }

            return solicitud;
        }




        private SolicitudServicio MapearSolicitud(SqlDataReader reader)
        {
            var solicitud = new SolicitudServicio
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                FechaSolicitud = reader.GetDateTime(reader.GetOrdinal("FechaSolicitud")),
                FechaServicio = reader.GetDateTime(reader.GetOrdinal("FechaServicio")),
                Estado = (EstadoSolicitud)Enum.Parse(typeof(EstadoSolicitud), reader.GetString(reader.GetOrdinal("Estado"))),
                IdEmpleado = reader.IsDBNull(reader.GetOrdinal("IdEmpleado")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                PrecioEstimado = reader.IsDBNull(reader.GetOrdinal("PrecioEstimado")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("PrecioEstimado"))
            };
            return solicitud;
        }


    }

}
