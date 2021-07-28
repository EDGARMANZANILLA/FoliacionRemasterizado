using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades;

namespace DAP.Foliacion.Datos
{
    public class FoliarConsultasDBSinEntity
    {


        public static List<NombreNominasDTO> ObtenerNombreNominas(string NumeroQuincena)
        {
            List<NombreNominasDTO> NombresNominasEncontradas = new List<NombreNominasDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select quincena, nomina, adicional, ruta, rutanomina, coment, id_nom from interfaces.dbo.bitacora where QUINCENA ="+NumeroQuincena+" and Foliado is null order by QUINCENA, id_nom", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        NombreNominasDTO NuevoNombreNomina = new NombreNominasDTO();

                        NuevoNombreNomina.Quincena = reader[0].ToString().Trim();
                        NuevoNombreNomina.Nomina = reader[1].ToString().Trim();
                        NuevoNombreNomina.Adicional = reader[2].ToString().Trim();
                        NuevoNombreNomina.Ruta = reader[3].ToString().Trim();
                        NuevoNombreNomina.RutaNomina = reader[4].ToString().Trim();
                        NuevoNombreNomina.Coment = reader[5].ToString().Trim();
                        NuevoNombreNomina.Id_nom = reader[6].ToString().Trim();

                        NombresNominasEncontradas.Add(NuevoNombreNomina);
                    }
                }


            }
            catch (Exception E)
            {
                NombreNominasDTO NuevoNombreNomina = new NombreNominasDTO();
                NuevoNombreNomina.Quincena = E.ToString();

                NombresNominasEncontradas.Add( NuevoNombreNomina);
                return NombresNominasEncontradas;
            }

        
            return NombresNominasEncontradas;
        }



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosIdNominaPagomatico(string AnSegunBitacora, int quincena, string NombreBanco)
        {
            List<DatosReporteRevisionNominaDTO> ListaDatosReporteFoliacionPorNomina = new List<DatosReporteRevisionNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select '' 'Id' , Substring(PARTIDA, 1, 6) 'PARTIDA', NOMBRE, DELEG, NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA', num  from interfaces.dbo." + AnSegunBitacora+" where TARJETA <> '' OR SERFIN <>'' OR BANCOMER <> '' OR BANORTE <>'' OR HSBC <>'' order by  IIF( isnull(NOM_ESP,0) =1 ,'1','2') , DELEG,  SUBSTRING(PARTIDA,2,8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        DatosReporteRevisionNominaDTO NuevoDatoReporte = new DatosReporteRevisionNominaDTO();

                        NuevoDatoReporte.Id = ++i;
                        NuevoDatoReporte.Num = reader[7].ToString().Trim();
                        NuevoDatoReporte.Partida = reader[1].ToString().Trim();
                        NuevoDatoReporte.Nombre = reader[2].ToString().Trim();
                        NuevoDatoReporte.Deleg = reader[3].ToString().Trim();
                        NuevoDatoReporte.Num_Che = reader[7].ToString().Trim()+quincena;
                        NuevoDatoReporte.Liquido = reader[5].ToString().Trim();
                        NuevoDatoReporte.CuentaBancaria = NombreBanco;
                        ListaDatosReporteFoliacionPorNomina.Add(NuevoDatoReporte);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosIdNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe para crear la revicion del pdf de cada nomina";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

                //agregar un dato si hay un error para que el usuario se entere que hubo un error y avise al administrador del sistema
                DatosReporteRevisionNominaDTO NuevoErrorDatoReporte = new DatosReporteRevisionNominaDTO();

                NuevoErrorDatoReporte.Id = 1;
                NuevoErrorDatoReporte.Partida = "";
                NuevoErrorDatoReporte.Nombre = "Verifique que la nomina que desea";
                NuevoErrorDatoReporte.Deleg = "";
                NuevoErrorDatoReporte.Num_Che = "foliar";
                NuevoErrorDatoReporte.Liquido = "exista";
                NuevoErrorDatoReporte.CuentaBancaria = "";

                ListaDatosReporteFoliacionPorNomina.Add(NuevoErrorDatoReporte);

                return ListaDatosReporteFoliacionPorNomina;
            }


            return ListaDatosReporteFoliacionPorNomina;
        }


        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosIdNominaPenalPagomatico(string AnSegunBitacora, int quincena, string NombreBanco)
        {
            List<DatosReporteRevisionNominaDTO> ListaDatosReporteFoliacionPorNomina = new List<DatosReporteRevisionNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select  '' 'ID' ,  NUM , Substring(PARTIDA, 1, 6) 'PARTIDA', NOMBRE, DELEG, NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA'   from interfaces.dbo."+AnSegunBitacora+" where TARJETA <> '' OR SERFIN <>'' OR BANCOMER <> '' OR BANORTE <>'' OR HSBC <>'' order by JUZGADO , NOMBRE collate SQL_Latin1_General_CP1_CI_AS", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        DatosReporteRevisionNominaDTO NuevoDatoReporte = new DatosReporteRevisionNominaDTO();

                        NuevoDatoReporte.Id = ++i;
                        NuevoDatoReporte.Num = reader[1].ToString().Trim();
                        NuevoDatoReporte.Partida = reader[2].ToString().Trim();
                        NuevoDatoReporte.Nombre = reader[3].ToString().Trim();
                        NuevoDatoReporte.Deleg = reader[4].ToString().Trim();
                        NuevoDatoReporte.Num_Che = reader[5].ToString().Trim()+quincena;
                        NuevoDatoReporte.Liquido = reader[6].ToString().Trim();
                        NuevoDatoReporte.CuentaBancaria = NombreBanco;
                        ListaDatosReporteFoliacionPorNomina.Add(NuevoDatoReporte);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = " ObtenerDatosIdNominaPENAL";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o la nomina no existe para crear la revicion del pdf de cada nomina";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

                //agregar un dato si hay un error para que el usuario se entere que hubo un error y avise al administrador del sistema
                DatosReporteRevisionNominaDTO NuevoErrorDatoReporte = new DatosReporteRevisionNominaDTO();

                NuevoErrorDatoReporte.Id = 1;
                NuevoErrorDatoReporte.Partida = "";
                NuevoErrorDatoReporte.Nombre = "No se pudieron leer los datos Comuniquese con el";
                NuevoErrorDatoReporte.Deleg = " ";
                NuevoErrorDatoReporte.Num_Che = "Admin del sistema";
                NuevoErrorDatoReporte.Liquido = "o ";
                NuevoErrorDatoReporte.CuentaBancaria = " reintente";

                ListaDatosReporteFoliacionPorNomina.Add(NuevoErrorDatoReporte);

                return ListaDatosReporteFoliacionPorNomina;
            }


            return ListaDatosReporteFoliacionPorNomina;
        }




        public static List<string> ObtenerAnBitacoraPorIdNumConexion(int IdNum) 
        {
            string an = null;
            string ap = null;
            string ad = null;

            List<string> anApAd = new List<string>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select an, ap, ad from interfaces.dbo.bitacora where  id_nom ="+IdNum+" ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                   
                    while (reader.Read())
                    {
                        an = reader[0].ToString().Trim();
                        ap = reader[1].ToString().Trim();
                        ad = reader[2].ToString().Trim();

                        anApAd.Add(an);
                        anApAd.Add(ap);
                        anApAd.Add(ad);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerAnBitacoraPorIdNumConexion";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener el dato de an de la bitacora para la foliacion";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }



            return anApAd;
        }




        public static string ObtenerRutaIdNomina(int IdNomina)
        {
            string rutaArchivo = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select ruta, rutaNomina from interfaces.dbo.bitacora where id_nom ="+IdNomina+" order by id_nom", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                       
                       rutaArchivo = reader[0].ToString().Trim()+ reader[1].ToString().Trim();
                        return rutaArchivo;
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRutaIdNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener el dato de an de la bitacora para la foliacion";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return rutaArchivo;
        }





    }
}
