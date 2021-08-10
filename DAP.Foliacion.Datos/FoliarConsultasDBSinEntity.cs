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
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select quincena, nomina, adicional, ruta, rutanomina, coment, id_nom from interfaces.dbo.bitacora where QUINCENA ="+NumeroQuincena+" and importado = 1 and Foliado is null order by QUINCENA, id_nom", connection);
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



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosIdNominaPagomatico(string NumeroNomina ,string AnSegunBitacora, int quincena, List<string> NombresBanco)
        {
            List<DatosReporteRevisionNominaDTO> ListaDatosReporteFoliacionPorNomina = new List<DatosReporteRevisionNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, CASE when a.TARJETA <>'' then '"+NombresBanco[0]+ "' when a.SERFIN <>'' then '"+NombresBanco[1]+ "'  when a.BANCOMER <>'' then '"+NombresBanco[3]+ "' when a.BANORTE <>'' then '"+NombresBanco[2]+ "' when a.HSBC <>'' then '"+NombresBanco[4]+"' end as 'CUENTABANCARIA' from interfaces.dbo." + AnSegunBitacora+" as a where TARJETA <> '' OR SERFIN <> '' OR BANCOMER <> '' OR BANORTE <> '' OR HSBC <> '' order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        DatosReporteRevisionNominaDTO NuevoDatoReporte = new DatosReporteRevisionNominaDTO();

                        NuevoDatoReporte.Id = ++i;

                        NuevoDatoReporte.Partida = reader[1].ToString().Trim();
                        NuevoDatoReporte.Num = reader[2].ToString().Trim();
                        NuevoDatoReporte.Nombre = reader[3].ToString().Trim();
                        NuevoDatoReporte.Deleg = reader[4].ToString().Trim();
                        NuevoDatoReporte.Nom = NumeroNomina;
                        NuevoDatoReporte.Num_Che = reader[2].ToString().Trim()+quincena;
                        NuevoDatoReporte.Liquido = reader[7].ToString().Trim();
                        NuevoDatoReporte.CuentaBancaria = reader[8].ToString().Trim();
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


        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosIdNominaPenalPagomatico(string NumeroNomina,string AnSegunBitacora, int quincena, List<string> NombresBanco)
        {
            List<DatosReporteRevisionNominaDTO> ListaDatosReporteFoliacionPorNomina = new List<DatosReporteRevisionNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select  '' 'ID'  , Substring(PARTIDA, 1, 6) 'PARTIDA',  NUM, NOMBRE, DELEG,'' 'NOMINA' ,NUM_CHE, LIQUIDO,  CASE when a.TARJETA <>'' then '"+NombresBanco[5]+"' when a.SERFIN <>'' then '"+NombresBanco[1]+"'  when a.BANCOMER <>'' then '"+NombresBanco[3]+"' when a.BANORTE <>'' then '"+NombresBanco[2]+"' when a.HSBC <>'' then '"+NombresBanco[4]+"' end as 'CUENTABANCARIA'    from interfaces.dbo." + AnSegunBitacora+" as a where TARJETA <> '' OR SERFIN <>'' OR BANCOMER <> '' OR BANORTE <>'' OR HSBC <>'' order by JUZGADO , NOMBRE collate SQL_Latin1_General_CP1_CI_AS", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int i = 0;
                    while (reader.Read())
                    {
                        DatosReporteRevisionNominaDTO NuevoDatoReporte = new DatosReporteRevisionNominaDTO();

                        NuevoDatoReporte.Id = ++i;
                        NuevoDatoReporte.Partida = reader[1].ToString().Trim();
                        NuevoDatoReporte.Num = reader[2].ToString().Trim();
                        NuevoDatoReporte.Nombre = reader[3].ToString().Trim();
                        NuevoDatoReporte.Deleg = reader[4].ToString().Trim();
                        NuevoDatoReporte.Nom = NumeroNomina;
                        NuevoDatoReporte.Num_Che = reader[2].ToString().Trim()+quincena;
                        NuevoDatoReporte.Liquido = reader[7].ToString().Trim();
                        NuevoDatoReporte.CuentaBancaria = reader[8].ToString().Trim();
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
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select an, ap, ad from interfaces.dbo.bitacora where importado = 1 and  id_nom ="+IdNum+" ", connection);
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
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select ruta, rutaNomina from interfaces.dbo.bitacora where importado = 1 and id_nom ="+IdNomina+" order by id_nom", connection);
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


        public static string ObtenerNumeroNominaXIdNum(int IdNum)
        {
            string numeroNomina = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select nomina from interfaces.dbo.bitacora where importado = 1 and  id_nom ="+IdNum+" ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        numeroNomina = reader[0].ToString().Trim();
                      
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = " ObtenerNumeroNominaXIdNum";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener el dato de an de la bitacora para la foliacion, verifique que exista el campo nomina en la nomina "+IdNum+"";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return numeroNomina;
        }



        public static List<DatosRevicionTodasNominasDTO> ObtenerListaDTOTodasNominasXquincena(string NumeroQuincena) 
        {
            List<DatosRevicionTodasNominasDTO> revicionTodasNominas = new List<DatosRevicionTodasNominasDTO>(); 
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select NOMINA, AN , AP, AD, ID_NOM from interfaces.dbo.bitacora where quincena = "+NumeroQuincena+" and importado = 1 order by id_nom ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {

                        DatosRevicionTodasNominasDTO nuevaRevicioNomina = new DatosRevicionTodasNominasDTO();

                        nuevaRevicioNomina.Nomina = reader[0].ToString().Trim();
                        nuevaRevicioNomina.An = reader[1].ToString().Trim();
                        nuevaRevicioNomina.Ap = reader[2].ToString().Trim();
                        nuevaRevicioNomina.Ad = reader[3].ToString().Trim();
                        nuevaRevicioNomina.Id_Nom = reader[4].ToString().Trim();

                        revicionTodasNominas.Add(nuevaRevicioNomina);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerListaDTOTodasNominasXquincena";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener los datos de la bitacora para la foliacion, verifique que existan datos para quincena " + NumeroQuincena + "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return revicionTodasNominas;
        }













        //Metodos para cheques Revicion y Foliacion
        public static DatosBitacoraParaCheque ObtenerAnBitacoraParaCheques(int IdNomina) 
        {
            DatosBitacoraParaCheque DatosNominaBitacora = new DatosBitacoraParaCheque();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(" select quincena, nomina, coment, id_nom, an, importado  from interfaces.dbo.bitacora where Importado = 1  and id_nom= "+IdNomina+" order by id_nom ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {

                      

                        DatosNominaBitacora.Quincena    = reader[0].ToString().Trim();
                        DatosNominaBitacora.Nomina      = reader[1].ToString().Trim();
                        DatosNominaBitacora.Comentario  = reader[2].ToString().Trim();
                        DatosNominaBitacora.Id_nom      = Convert.ToInt32( reader[3].ToString().Trim());
                        DatosNominaBitacora.An          = reader[4].ToString().Trim();
                        DatosNominaBitacora.Importado   = Convert.ToBoolean( reader[5].ToString().Trim());

                        return DatosNominaBitacora;
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerAnBitacoraParaCheques";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener los datos de la bitacora para la foliacion, verifique que existan datos para id_nom " + IdNomina+ "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }

            DatosNominaBitacora.Comentario = "Sin Datos";

            return DatosNominaBitacora;

        }





        public static string ObtenerDetalledeNominaConSindicatoCheques(int IdNomina)
        {
            DatosBitacoraParaCheque DatosNominaBitacora = new DatosBitacoraParaCheque();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(" select quincena, nomina, coment, id_nom, an, importado  from interfaces.dbo.bitacora where Importado = 1  and id_nom= " + IdNomina + " order by id_nom ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {



                        DatosNominaBitacora.Quincena = reader[0].ToString().Trim();
                        DatosNominaBitacora.Nomina = reader[1].ToString().Trim();
                        DatosNominaBitacora.Comentario = reader[2].ToString().Trim();
                        DatosNominaBitacora.Id_nom = Convert.ToInt32(reader[3].ToString().Trim());
                        DatosNominaBitacora.An = reader[4].ToString().Trim();
                        DatosNominaBitacora.Importado = Convert.ToBoolean(reader[5].ToString().Trim());

                        return DatosNominaBitacora;
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerAnBitacoraParaCheques";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener los datos de la bitacora para la foliacion, verifique que existan datos para id_nom " + IdNomina + "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }

            DatosNominaBitacora.Comentario = "Sin Datos";

            return DatosNominaBitacora;

        }
    }
}
