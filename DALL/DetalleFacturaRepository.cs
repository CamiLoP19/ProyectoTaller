using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class DetalleFacturaRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        public DetalleFacturaRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public void Insertar(DetalleFactura detalle, SqlConnection connection)
        {
             string ssql = "INSERT INTO DetalleFacturas (IdFactura, IdProducto, IdSolicitudServicio, Descripcion, PrecioUnitario, Cantidad) " +
              "VALUES (@IdFactura, @IdProducto, @IdSolicitudServicio, @Descripcion, @PrecioUnitario, @Cantidad)";


            SqlCommand cmd = new SqlCommand(ssql, connection);
            cmd.Parameters.AddWithValue("@IdFactura", detalle.IdFactura);
            cmd.Parameters.AddWithValue("@IdProducto", (object)detalle.IdProducto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IdSolicitudServicio", (object)detalle.IdSolicitudServicio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Descripcion", detalle.Descripcion);
            cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
            cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);


            cmd.ExecuteNonQuery();
        }


        public List<DetalleFactura> ObtenerPorFacturaId(int idFactura, SqlConnection connection)
        {
            List<DetalleFactura> detalles = new List<DetalleFactura>();
            string ssql = "SELECT * FROM DetalleFacturas WHERE IdFactura = @IdFactura";
            SqlCommand cmd = new SqlCommand(ssql, connection);
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                detalles.Add(Mapper(reader));
            }
            reader.Close();
            return detalles;
        }

        private DetalleFactura Mapper(SqlDataReader reader)
        {
            return new DetalleFactura
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdFactura = reader.GetInt32(reader.GetOrdinal("IdFactura")),
                IdProducto = reader.IsDBNull(reader.GetOrdinal("IdProducto")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdProducto")),
                IdSolicitudServicio = reader.IsDBNull(reader.GetOrdinal("IdSolicitudServicio")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdSolicitudServicio")),
                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"))
            };
        }


        // No necesitas métodos para Insertar o Actualizar independientes si siempre los manejas desde FacturaRepository
    }

}
