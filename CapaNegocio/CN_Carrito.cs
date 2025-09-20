using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Carrito
    {
        private CD_Carrito objCapaDatos = new CD_Carrito();

        public bool ExisteCarrito(int idCliente, int idProducto)
        {
            return objCapaDatos.ExisteCarrito(idCliente, idProducto);
        }
        public bool OperacionCarrito(int idCliente, int idProducto, bool sumar, out string Mensaje)
        {
            return objCapaDatos.OperacionCarrito(idCliente, idProducto, sumar, out Mensaje);
        }

        public int CantidadEnCarrito(int idCliente)
        {
            return objCapaDatos.CantidadEnCarrito(idCliente);
        }

        public List<Carrito> ListarProducto(int idCliente)
        {
            return objCapaDatos.ListarProducto(idCliente);
        }

        public bool EliminarCarrito(int idCliente, int idProducto)
        {
            return objCapaDatos.EliminarCarrito(idCliente, idProducto);
        }
    }
}
