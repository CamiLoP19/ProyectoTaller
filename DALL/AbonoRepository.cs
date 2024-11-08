using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class AbonoRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";

        public AbonoRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(Abono abono)
        {
            string ssql = "INSERT INTO Abonos (IdFactura, Monto, Fecha) " +
                          "VALUES (@IdFactura, @Monto, @Fecha)";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdFactura", abono.IdFactura);
            cmd.Parameters.AddWithValue("@Monto", abono.Monto);
            cmd.Parameters.AddWithValue("@Fecha", abono.Fecha);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se registró un abono de {abono.Monto} a la factura {abono.IdFactura}";
            else
                return "No se pudo registrar el abono";
        }

        public List<Abono> ObtenerPorFacturaId(int idFactura)
        {
            List<Abono> abonos = new List<Abono>();
            string ssql = "SELECT * FROM Abonos WHERE IdFactura = @IdFactura";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                abonos.Add(Mapper(reader));
            }
            _connection.Close();
            return abonos;
        }

        private Abono Mapper(SqlDataReader reader)
        {
            return new Abono
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdFactura = reader.GetInt32(reader.GetOrdinal("IdFactura")),
                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha"))
            };
        }

        // Implementa métodos para Actualizar y Eliminar si es necesario
    }


}
