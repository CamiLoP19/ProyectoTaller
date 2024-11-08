using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALL;
using ENTIDADES;

namespace BLL
{
    public class ProductoService
    {
        private ProductoRepository productoRepository;

        public ProductoService()
        {
            productoRepository = new ProductoRepository();
        }

        public string AgregarProducto(Producto producto)
        {
            // Validaciones de negocio
            return productoRepository.Insertar(producto);
        }

        public string ActualizarProducto(Producto producto)
        {
            // Validaciones adicionales si es necesario
            return productoRepository.Actualizar(producto);
        }

        public string EliminarProducto(int id)
        {
            return productoRepository.Eliminar(id);
        }

        public List<Producto> ObtenerProductos()
        {
            return productoRepository.ObtenerTodos();
        }

        public List<Producto> ObtenerProductosDisponibles()
        {
            return productoRepository.ObtenerProductosDisponibles();
        }

        public string Actualizar(Producto producto)
        {
            return productoRepository.Actualizar(producto);
        }
    }


}
