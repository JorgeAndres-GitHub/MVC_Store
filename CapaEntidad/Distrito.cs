using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    //CREATE TABLE DISTRITO(
    //    IdDistrito varchar(6) NOT NULL,
    //    Descripcion varchar(25) NOT NULL,
    //    IdProvincia varchar(4) NOT NULL,
    //    IdDepartamento varchar(2) NOT NULL
    //)

    public class Distrito
    {
        public string IdDistrito { get; set; }
        public string Descripcion { get; set; }
    }
}
