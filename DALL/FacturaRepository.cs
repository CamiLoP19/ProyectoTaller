using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{

    public class FacturaRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        private readonly DetalleFacturaRepository detalleFacturaRepository;

        public FacturaRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
            detalleFacturaRepository = new DetalleFacturaRepository();
        }

        public string Insertar(Factura factura)
        {
            string ssql = "INSERT INTO Facturas (IdCliente, IdEmpleado, Fecha, Total, Pagada) " +
                          "VALUES (@IdCliente, @IdEmpleado, @Fecha, @Total, @Pagada); SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdCliente", factura.IdCliente);
            cmd.Parameters.AddWithValue("@IdEmpleado", factura.IdEmpleado);
            cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
            cmd.Parameters.AddWithValue("@Total", factura.Total);
            cmd.Parameters.AddWithValue("@Pagada", factura.Pagada);

            _connection.Open();
            int idFactura = Convert.ToInt32(cmd.ExecuteScalar());

            // Insertar detalles de factura
            foreach (var detalle in factura.Detalles)
            {
                detalle.IdFactura = idFactura;
                detalleFacturaRepository.Insertar(detalle, _connection);
            }

            _connection.Close();

            return $"Se creó la factura con ID {idFactura}";
        }

        public Factura ObtenerPorId(int id)
        {
            string ssql = "SELECT * FROM Facturas WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Factura factura = null;
            if (reader.Read())
            {
                factura = Mapper(reader);
            }
            reader.Close();

            if (factura != null)
            {
                // Calcular el TotalPagado
                factura.TotalPagado = ObtenerTotalPagado(factura.Id);
            }

            _connection.Close();
            return factura;
        }

        public decimal ObtenerTotalPagado(int idFactura)
        {
            string ssql = "SELECT SUM(Monto) FROM PagosFactura WHERE IdFactura = @IdFactura";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);

            object result = cmd.ExecuteScalar();
            if (result != DBNull.Value && result != null)
                return Convert.ToDecimal(result);
            else
                return 0m;
        }

        public List<Factura> ObtenerTodos()
        {
            List<Factura> facturas = new List<Factura>();
            string ssql = "SELECT * FROM Facturas";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Factura factura = Mapper(reader);
                factura.Detalles = detalleFacturaRepository.ObtenerPorFacturaId(factura.Id, _connection);
                facturas.Add(factura);
            }
            _connection.Close();
            return facturas;
        }

        private Factura Mapper(SqlDataReader reader)
        {
            return new Factura
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                Pagada = reader.GetBoolean(reader.GetOrdinal("Pagada")),
                Detalles = new List<DetalleFactura>()
            };
        }

        // Implementa métodos para Actualizar y Eliminar si es necesario
        public string AbonarFactura(int idFactura, decimal monto)
        {
            // Verificar si la factura ya está pagada
            string ssqlVerificar = "SELECT Pagada FROM Facturas WHERE Id = @IdFactura";
            SqlCommand cmdVerificar = new SqlCommand(ssqlVerificar, _connection);
            cmdVerificar.Parameters.AddWithValue("@IdFactura", idFactura);

            _connection.Open();
            bool pagada = (bool)cmdVerificar.ExecuteScalar();
            _connection.Close();

            if (pagada)
            {
                return "La factura ya está pagada. No se puede realizar más abonos.";
            }

            // Proceder con el abono
            string ssql = @"
    INSERT INTO PagosFactura (IdFactura, Monto, FechaPago)
    VALUES (@IdFactura, @Monto, GETDATE());

    DECLARE @Total DECIMAL(18,2) = (SELECT Total FROM Facturas WHERE Id = @IdFactura);
    DECLARE @TotalPagado DECIMAL(18,2) = (SELECT SUM(Monto) FROM PagosFactura WHERE IdFactura = @IdFactura);

    IF @TotalPagado >= @Total
    BEGIN
        UPDATE Facturas SET Pagada = 1 WHERE Id = @IdFactura;

        -- Registrar la ganancia si no existe
        IF NOT EXISTS (SELECT 1 FROM Ganancias WHERE IdFactura = @IdFactura)
        BEGIN
            INSERT INTO Ganancias (IdFactura, Fecha, Monto)
            VALUES (@IdFactura, GETDATE(), @Total);
        END
    END
    ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdFactura", idFactura);
            cmd.Parameters.AddWithValue("@Monto", monto);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return "Abono registrado exitosamente.";
            else
                return "No se pudo registrar el abono.";
        }





        public List<Factura> ObtenerFacturasPorCliente(int idCliente)
        {
            List<Factura> facturas = new List<Factura>();
            string ssql = "SELECT * FROM Facturas WHERE IdCliente = @IdCliente";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@IdCliente", idCliente);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                facturas.Add(new Factura
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    Pagada = reader.GetBoolean(reader.GetOrdinal("Pagada"))
                });
            }
            _connection.Close();
            return facturas;
        }

        public Factura ObtenerFacturaConDetalles(int idFactura)
        {
            Factura factura = null;
            string ssql = "SELECT * FROM Facturas WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", idFactura);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                factura = new Factura
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    IdCliente = reader.GetInt32(reader.GetOrdinal("IdCliente")),
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    Pagada = reader.GetBoolean(reader.GetOrdinal("Pagada")),
                    Detalles = new List<DetalleFactura>()
                };
            }
            reader.Close();

            if (factura != null)
            {
                // Obtener los detalles de la factura
                ssql = "SELECT * FROM DetalleFacturas WHERE IdFactura = @IdFactura";
                cmd = new SqlCommand(ssql, _connection);
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    factura.Detalles.Add(new DetalleFactura
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        IdFactura = reader.GetInt32(reader.GetOrdinal("IdFactura")),
                        IdProducto = reader.IsDBNull(reader.GetOrdinal("IdProducto")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdProducto")),
                        IdSolicitudServicio = reader.IsDBNull(reader.GetOrdinal("IdSolicitudServicio")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("IdSolicitudServicio")),
                        Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                        Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                        PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario"))
                        // Subtotal se calcula en la propiedad Subtotal del objeto DetalleFactura
                    });
                }
                reader.Close();
            }
            _connection.Close();
            return factura;
        }
        public List<Factura> ObtenerFacturasPagadas()
        {
            List<Factura> facturas = new List<Factura>();
            string ssql = "SELECT * FROM Facturas WHERE Pagada = 1";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Factura factura = Mapper(reader);
                facturas.Add(factura);
            }
            _connection.Close();
            return facturas;
        }


        public decimal ObtenerTotalGanancias()
        {
            string ssql = "SELECT SUM(Total) FROM Facturas WHERE Pagada = 1";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            object result = cmd.ExecuteScalar();
            _connection.Close();

            if (result != DBNull.Value && result != null)
                return Convert.ToDecimal(result);
            else
                return 0m;
        }
        public List<FacturaConComision> ObtenerFacturasPagadasConComision()
        {
            List<FacturaConComision> facturas = new List<FacturaConComision>();
            string ssql = @"
    SELECT f.Id, f.Total, f.IdEmpleado, e.PorcentajeComision
    FROM Facturas f
    INNER JOIN Empleados e ON f.IdEmpleado = e.Id
    WHERE f.Pagada = 1
    ";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                FacturaConComision factura = new FacturaConComision
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                    PorcentajeComision = reader.GetDecimal(reader.GetOrdinal("PorcentajeComision"))
                };
                facturas.Add(factura);
            }
            _connection.Close();
            return facturas;
        }



    }

}
