using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Ubicacion
    {
        private CD_Ubicacion objCapaDatos = new();

        public List<Departamento> ObtenerDepartamento()
        {
            return objCapaDatos.ObtenerDepartamento();
        }

        public List<Provincia> ObtenerProvincia(string idDepartamento)
        {
            return objCapaDatos.ObtenerProvincia(idDepartamento);
        }

        public List<Distrito> ObtenerDistrito(string idDepartamento, string idProvincia)
        {
            return objCapaDatos.ObtenerDistrito(idDepartamento, idProvincia);
        }
    }
}
