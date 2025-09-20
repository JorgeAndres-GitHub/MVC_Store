using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Ubicacion
    {
        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> lista = new();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM DEPARTAMENTO";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Departamento
                            {
                                IdDepartamento = dr["IdDepartamento"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Departamento>();
            }

            return lista;
        }

        public List<Provincia> ObtenerProvincia(string idDepartamento)
        {
            List<Provincia> lista = new();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM PROVINCIA WHERE IdDepartamento = @iddepartamento";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@iddepartamento", idDepartamento); 
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Provincia
                            {
                                IdProvincia = dr["IdProvincia"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Provincia>();
            }

            return lista;
        }

        public List<Distrito> ObtenerDistrito(string idDepartamento, string idProvincia)
        {
            List<Distrito> lista = new();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cn))
                {
                    string query = "SELECT * FROM DISTRITO WHERE IdProvincia = @idprovincia and IdDepartamento = @iddepartamento";

                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.Parameters.AddWithValue("@idprovincia", idProvincia);
                    cmd.Parameters.AddWithValue("@iddepartamento", idDepartamento);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Distrito
                            {
                                IdDistrito = dr["IdDistrito"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),

                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lista = new List<Distrito>();
            }

            return lista;
        }
    }
}
