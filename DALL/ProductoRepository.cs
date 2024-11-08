using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENTIDADES;

namespace DALL
{
    public class ProductoRepository
    {
        private readonly SqlConnection _connection;
        private readonly string cadenaConexion = "Server=.\\SQLEXPRESS;Database=TallerBicicletas;Trusted_Connection=True;";
        public ProductoRepository()
        {
            _connection = new SqlConnection(cadenaConexion);
        }

        public string Insertar(Producto producto)
        {
            string ssql = "INSERT INTO Productos (Nombre, Precio, Stock) " +
                          "VALUES (@Nombre, @Precio, @Stock)";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@Stock", producto.Stock);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return $"Se agregó el producto {producto.Nombre}";
            else
                return "No se pudo agregar el producto";
        }

        public Producto ObtenerPorId(int id)
        {
            string ssql = "SELECT * FROM Productos WHERE Id = @Id";
            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            Producto producto = null;
            if (reader.Read())
            {
                producto = Mapper(reader);
            }
            _connection.Close();
            return producto;
        }

        public List<Producto> ObtenerTodos()
        {
            List<Producto> productos = new List<Producto>();
            string ssql = "SELECT * FROM Productos";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(Mapper(reader));
            }
            _connection.Close();
            return productos;
        }

        private Producto Mapper(SqlDataReader reader)
        {
            return new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock"))
            };
        }

        public string Actualizar(Producto producto)
        {
            string ssql = @"
        UPDATE Productos SET
            Nombre = @Nombre,
            Precio = @Precio,
            Stock = @Stock
        WHERE Id = @Id
    ";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", producto.Id);
            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@Stock", producto.Stock);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return "Producto actualizado correctamente.";
            else
                return "No se pudo actualizar el producto.";
        }

        public string Eliminar(int id)
        {
            string ssql = "DELETE FROM Productos WHERE Id = @Id";

            SqlCommand cmd = new SqlCommand(ssql, _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            _connection.Open();
            int filasAfectadas = cmd.ExecuteNonQuery();
            _connection.Close();

            if (filasAfectadas > 0)
                return "Producto eliminado correctamente";
            else
                return "No se pudo eliminar el producto";
        }
        public List<Producto> ObtenerProductosDisponibles()
        {
            List<Producto> productos = new List<Producto>();
            string ssql = "SELECT * FROM Productos WHERE Stock > 0";
            SqlCommand cmd = new SqlCommand(ssql, _connection);

            _connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(MapearProducto(reader));
            }
            _connection.Close();
            return productos;
        }

        private Producto MapearProducto(SqlDataReader reader)
        {
            return new Producto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock"))
            };
        }
    }

}
