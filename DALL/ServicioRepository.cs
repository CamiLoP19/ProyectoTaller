using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class ServicioRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        public ServicioRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(Servicio servicio)
        {
            string ssql = "INSERT INTO Servicios (Descripcion, PrecioManoObra) " +
                          "VALUES (@Descripcion, @PrecioManoObra)";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
            cmd.Parameters.AddWithValue("@PrecioManoObra", servicio.PrecioManoObra);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se agregó el servicio {servicio.Descripcion}";
            else
                return "No se pudo agregar el servicio";
        }

        public Servicio ObtenerPorId(int id)
        {
            string ssql = "SELECT * FROM Servicios WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Servicio servicio = null;
            if (reader.Read())
            {
                servicio = Mapper(reader);
            }
            _connection.Close();
            return servicio;
        }


        public List<Servicio> ObtenerTodos()
        {
            List<Servicio> servicios = new List<Servicio>();
            string ssql = "SELECT * FROM Servicios";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                servicios.Add(Mapper(reader));
            }
            _connection.Close();
            return servicios;
        }


        private Servicio Mapper(SqlDataReader reader)
        {
            return new Servicio
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                PrecioManoObra = reader.GetDecimal(reader.GetOrdinal("PrecioManoObra"))
            };
        }


        // Implementa métodos para Actualizar y Eliminar si es necesario
    }

}
