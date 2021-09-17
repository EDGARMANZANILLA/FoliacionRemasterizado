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




        public static List<string> ObtenerAnApAdNominaBitacoraPorIdNumConexion(int IdNum) 
        {
            string an = null;
            string ap = null;
            string ad = null;
            string nomina = null;

            List<string> anApAd = new List<string>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select an, ap, ad, nomina from interfaces.dbo.bitacora where importado = 1 and  id_nom ="+IdNum+" ", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                   
                    while (reader.Read())
                    {
                        an = reader[0].ToString().Trim();
                        ap = reader[1].ToString().Trim();
                        ad = reader[2].ToString().Trim();
                        nomina = reader[3].ToString().Trim();

                        anApAd.Add(an);
                        anApAd.Add(ap);
                        anApAd.Add(ad);
                        anApAd.Add(nomina);
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

        //obtiene el nombre que se muestra el modal al cargar los detalles de las delegaciones por nomina
        public static string ObtenerNombreModalDetalleNomina(int IdNomina) 
        {
            string NombreModal = null;
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("select nomina,'' 'EsAdicional', adicional 'NombreAdicional', coment, quincena, id_nom  from interfaces.dbo.bitacora where id_nom ="+IdNomina+"", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        if (string.IsNullOrWhiteSpace(reader[2].ToString().Trim()))
                        {
                            NombreModal = reader[0].ToString().Trim() + " -" + "- " + reader[3].ToString().Trim() + " _" + "_ " + reader[4].ToString().Trim();
                        }
                        else 
                        {
                            NombreModal = reader[0].ToString().Trim()+" -"+"- "+" ADICIONAL "+ " _" +"_ "+ reader[2].ToString().Trim() + " _" + "_ " + reader[3].ToString().Trim() + " _" + "_ " + reader[4].ToString().Trim();
                        }

                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerNombreModalDetalleNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo obtener los datos de la bitacora de la nomina "+IdNomina+" ";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }

            return NombreModal;
        }










        //Metodos para cheques Revicion y Foliacion
        /// <summary>
        /// Metodos para cheques Revicion y Foliacion el cual es para saber algunos detalles pasando un IdNomina 
        /// </summary>
        /// <param name="IdNomina"> es un entero</param>
        /// <returns>Este metodo trae como resultado quincena, nomina, coment, id_nom, an, en ese orden importado pasandole un IdNomina</returns>
        public static DatosBitacoraParaCheque ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(int IdNomina) 
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
                NuevaExcepcion.Metodo = "ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo leer u obtener los datos de la bitacora para la foliacion, verifique que existan datos para id_nom " + IdNomina+ "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }

            DatosNominaBitacora.Comentario = "Sin Datos";

            return DatosNominaBitacora;

        }




        /// <summary>
        /// Consulta el detalle de personal total de una nomina para saber el total de cheques por delegacion como se imprimen desde nomina foxito Ing.Gabriela
        /// Recibe como parametro una lista de consulta de totales y un boleano para saber si es general, Descentralizada o pertenece a otra nomina  
        /// </summary>
        /// <param name="ListaConsultas"></param>
        /// <param name=" EsNominaGeneralODesc"> true si es nomina 01 o 02 o false si no </param>
        /// <returns>Regresa una lista de Totales de Registros X Delegacion en la cual vienen cuantos cheques hay por delegacion "TotalRegistrosXDelegacionDTO"</returns>
        public static List<TotalRegistrosXDelegacionDTO> ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(List<string> ListaConsultas, bool EsNominaGeneralODesc)
        {
            List<TotalRegistrosXDelegacionDTO> TotalRegistros = new List<TotalRegistrosXDelegacionDTO>();


            foreach(string consulta in ListaConsultas)
            {

                try
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                        while (reader.Read())
                        {
                            //posicion 0 = sindicato -> 0 u 1
                            //posicion 1 = delegacion -> cadena
                            //posicion 2 = Total  -> numero de registros casteable a int
                            int a = Convert.ToInt32(reader[2].ToString().Trim());
                            if (Convert.ToInt32(reader[2].ToString().Trim()) > 0 ) 
                            {
                                TotalRegistrosXDelegacionDTO nuevoRegistro = new TotalRegistrosXDelegacionDTO();

                                if (EsNominaGeneralODesc)
                                {
                                    int castBoleano = Convert.ToInt32(reader[0].ToString().Trim());
                                    nuevoRegistro.Sindicato = Convert.ToBoolean(castBoleano);
                                }
                                nuevoRegistro.Delegacion    = reader[1].ToString().Trim();
                                nuevoRegistro.Total         = Convert.ToInt32( reader[2].ToString().Trim() );


                                TotalRegistros.Add(nuevoRegistro);
                            }



                        }
                    }


                }
                catch (Exception E)
                {
                    var transaccion = new Transaccion();

                    var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                    LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                    NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                    NuevaExcepcion.Metodo = "ObtenerDetalleNominaConsulta";
                    NuevaExcepcion.Usuario = null;
                    NuevaExcepcion.Excepcion = E.Message;
                    NuevaExcepcion.Comentario = "problema al extraer la data para la tabla del modal de pagoXFormas de pago al cargar una nomina";
                    NuevaExcepcion.Fecha = DateTime.Now;

                    repositorio.Agregar(NuevaExcepcion);

                }




            }

            return TotalRegistros;

        }













        /// <summary>
        /// pasa como parametro una consulta de una nomina y devuelve si ya fue foleada o no 
        /// </summary>
        /// <param name="ConsultaPreparada"></param>
        /// <returns>Devuelve False cuando no han sido foliados y true cuando ya fueron foleados</returns>
        public static bool ConsultaEstaFoliada(string ConsultaPreparada)
        {
            //nominaEstaFoliada
            bool consultaNominaEstaFoliada = true;

            List<string> listaNumcheFoliados = new List<string>();
                try
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ConsultaPreparada, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                        while (reader.Read())
                        {
                        /* var a = string.IsNullOrWhiteSpace("");
                         var b = string.IsNullOrWhiteSpace(null);
                         var c = string.IsNullOrWhiteSpace("01230");
                        */


                        //es true si el campo esta vacio o en blanco 
                        if (!string.IsNullOrWhiteSpace(reader[0].ToString().Trim()))
                            {
                                listaNumcheFoliados.Add(reader[0].ToString().Trim());
                            }



                        }
                    }


                }
                catch (Exception E)
                {
                    var transaccion = new Transaccion();

                    var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                    LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                    NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                    NuevaExcepcion.Metodo = "ConsultaEstaFoliada";
                    NuevaExcepcion.Usuario = null;
                    NuevaExcepcion.Excepcion = E.Message;
                    NuevaExcepcion.Comentario = "problema al ejecutar una consulta para saber si esta foliada o no para modal de pagoXFormas de pago al cargar una nomina";
                    NuevaExcepcion.Fecha = DateTime.Now;

                    repositorio.Agregar(NuevaExcepcion);

                }


            if (listaNumcheFoliados.Count() == 0)
            {
                consultaNominaEstaFoliada = false;
            }
              

            return consultaNominaEstaFoliada;

        }





        /// <summary>
        /// Obtiene data para la revicion de la nomina de formas de pago filtrada por delegacion 
        /// </summary>
        /// <param name="NumeroNomina"></param>
        /// <param name="AnSegunBitacora"></param>
        /// <param name="quincena"></param>
        /// <param name="NombresBanco"></param>
        /// <returns></returns>
        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosNominaRevicionFormasDePago(string ConsultaSql, string NumeroNomina, int NumeroChequeInicial, string NombreBanco ,bool Inhabilitado, int RangoInhabilitadoInicial, int RangoInhabilitadoFinal)
        {
            List<DatosReporteRevisionNominaDTO> ListaDatosReporteFoliacionPorNomina = new List<DatosReporteRevisionNominaDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ConsultaSql, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int TotalInhabilitados = (Convert.ToInt32(RangoInhabilitadoFinal) - Convert.ToInt32(RangoInhabilitadoInicial)) + 1;
                    int TotalInhabilitadosStatico = TotalInhabilitados;
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

                        if (Inhabilitado)
                        {
                            if (NumeroChequeInicial < RangoInhabilitadoInicial)
                            {
                                NuevoDatoReporte.Num_Che = Convert.ToString(NumeroChequeInicial++);
                            }
                            else
                            {
                                if (TotalInhabilitados > TotalInhabilitadosStatico)
                                {
                                    NuevoDatoReporte.Num_Che = Convert.ToString(++NumeroChequeInicial);
                                }
                                else 
                                {
                                    NumeroChequeInicial += TotalInhabilitados++;
                                    NuevoDatoReporte.Num_Che = Convert.ToString(NumeroChequeInicial);
                                }
                            }
                        }
                        else 
                        {
                          NuevoDatoReporte.Num_Che = Convert.ToString( NumeroChequeInicial++);
                        }


                       

                        NuevoDatoReporte.Liquido = reader[7].ToString().Trim();
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
                NuevaExcepcion.Metodo = "ObtenerDatosNominaRevicionFormasDePago";
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






        public static DatosCompletosBitacoraParaChequesDTO ObtenerDatosCompletosBitacoraPorIdNom(int IdNom)
        {

            DatosCompletosBitacoraParaChequesDTO DatosCompletosBitacora = new DatosCompletosBitacoraParaChequesDTO();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {

                    string consulta = "select id_nom, nomina, an, adicional, quincena,mes, referencia, CASE nomina  WHEN '08' THEN 'True' ELSE 'False' END 'EsPenA' ,coment  from interfaces.dbo.bitacora where id_nom = " +IdNom + " order by id_nom";
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        DatosCompletosBitacoraParaChequesDTO NuevoDetalle = new DatosCompletosBitacoraParaChequesDTO();

                        NuevoDetalle.Id_nom = Convert.ToInt32(reader[0].ToString().Trim());
                        NuevoDetalle.Nomina = reader[1].ToString().Trim();
                        NuevoDetalle.An = reader[2].ToString().Trim();
                        NuevoDetalle.Adicional = reader[3].ToString().Trim();
                        NuevoDetalle.Quincena = reader[4].ToString().Trim();
                        NuevoDetalle.Mes = Convert.ToInt32(reader[5].ToString().Trim());
                        NuevoDetalle.ReferenciaBitacora = reader[6].ToString().Trim();
                        NuevoDetalle.EsPenA = Convert.ToBoolean( reader[7].ToString().Trim() );
                        NuevoDetalle.Coment = reader[8].ToString().Trim();

                        return NuevoDetalle;
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosCompletosBitacoraPorQuincena";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

                DatosCompletosBitacoraParaChequesDTO ListaError = new DatosCompletosBitacoraParaChequesDTO();

                return ListaError;
            }


            return DatosCompletosBitacora;
        }




        public static List<DatosCompletosBitacoraParaChequesDTO> ObtenerDatosCompletosBitacoraPorQuincena(int Quincena) 
        {

            List<DatosCompletosBitacoraParaChequesDTO> ListaDatosCompletosBitacora = new List<DatosCompletosBitacoraParaChequesDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {

                    string consulta = "select id_nom, nomina, an, adicional, quincena,mes, referencia, CASE nomina  WHEN '08' THEN '1' ELSE '' END 'EsPenA' ,coment  from interfaces.dbo.bitacora where quincena = "+Quincena+" order by id_nom";
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

      
                    while (reader.Read())
                    {
                        DatosCompletosBitacoraParaChequesDTO NuevoDetalle = new DatosCompletosBitacoraParaChequesDTO();

                        NuevoDetalle.Id_nom = Convert.ToInt32( reader[0].ToString().Trim() );
                        NuevoDetalle.Nomina = reader[1].ToString().Trim();
                        NuevoDetalle.An = reader[2].ToString().Trim();
                        NuevoDetalle.Adicional = reader[3].ToString().Trim();
                        NuevoDetalle.Quincena = reader[4].ToString().Trim();
                        NuevoDetalle.Mes = Convert.ToInt32( reader[5].ToString().Trim() );
                        NuevoDetalle.ReferenciaBitacora = reader[6].ToString().Trim();
                        NuevoDetalle.EsPenA = Convert.ToBoolean( reader[7].ToString().Trim());
                        NuevoDetalle.Coment = reader[8].ToString().Trim();
                        
                        ListaDatosCompletosBitacora.Add(NuevoDetalle);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosCompletosBitacoraPorQuincena";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

                List<DatosCompletosBitacoraParaChequesDTO> ListaError = new List<DatosCompletosBitacoraParaChequesDTO>();

                return ListaError;
            }


            return ListaDatosCompletosBitacora;
        }




        public static List< NumRfcNombreLiquidoDTO> ObtenerNumRfcNombreLiquidoDeNomina(string An, bool EsPena)
        {

            //List< Dictionary<string, string> > registrosAfoliar = new List< Dictionary<string, string> >();
            List< NumRfcNombreLiquidoDTO > registrosAfoliar = new List<NumRfcNombreLiquidoDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {

                    string consulta = "select NUM, RFC, NOMBRE, LIQUIDO  from interfaces.dbo."+An+" where TARJETA <> '' or SERFIN <> '' or BANCOMER <> '' or BANORTE <> '' or HSBC <> '' ";

                    string nuevaConsulta = "";
                    if (EsPena)
                    {
                        nuevaConsulta = "select  NUM, RFC, NOMBRE, LIQUIDO, " +
                                           "CASE when TARJETA <> '' then 'BANAMEX PODER JUDICIAL' when SERFIN<>'' then 'SANTANDER'  when BANCOMER<>'' then 'BBVA' when BANORTE<>'' then 'BANORTE' when HSBC<>'' then 'HSBC' end as 'CUENTABANCARIA'," +
                                           "CASE when TARJETA <> '' then '8' when SERFIN<>'' then '2'  when BANCOMER<>'' then '4' when BANORTE<>'' then '3' when HSBC<>'' then '5' end as 'IdCuentaBancariaFoliacion'" +
                                           "from interfaces.dbo." + An + " where TARJETA <> '' or SERFIN<> '' or BANCOMER<> '' or BANORTE<> '' or HSBC<> ''";


                    }
                    else
                    {

                       nuevaConsulta = "select  NUM, RFC, NOMBRE, LIQUIDO, " +
                                               "CASE when TARJETA <> '' then 'BANAMEX' when SERFIN<>'' then 'SANTANDER'  when BANCOMER<>'' then 'BBVA' when BANORTE<>'' then 'BANORTE' when HSBC<>'' then 'HSBC' end as 'CUENTABANCARIA'," +
                                               "CASE when TARJETA <> '' then '1' when SERFIN<>'' then '2'  when BANCOMER<>'' then '4' when BANORTE<>'' then '3' when HSBC<>'' then '5' end as 'IdCuentaBancariaFoliacion'" +
                                               "from interfaces.dbo." + An + " where TARJETA <> '' or SERFIN<> '' or BANCOMER<> '' or BANORTE<> '' or HSBC<> ''";

                    }






                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(nuevaConsulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        NumRfcNombreLiquidoDTO nuevoRegistro = new NumRfcNombreLiquidoDTO();

                        nuevoRegistro.NumeroEmpleado = reader[0].ToString().Trim();
                        nuevoRegistro.Rfc = reader[1].ToString().Trim();
                        nuevoRegistro.Nombre = reader[2].ToString().Trim();
                        nuevoRegistro.Liquido = Convert.ToDecimal( reader[3].ToString().Trim());
                        nuevoRegistro.NombreBanco = reader[4].ToString().Trim();
                        nuevoRegistro.IdCuentaBancaria = reader[5].ToString().Trim();

                        registrosAfoliar.Add(nuevoRegistro);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerNumRfcNombreLiquidoDeNomina";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);


                List<NumRfcNombreLiquidoDTO> ListaError = new List<NumRfcNombreLiquidoDTO>();


                return ListaError;
            }


            return registrosAfoliar;
        }




        /// <summary>
        /// Actualiza un registro con el folio para un pagomatico  numeroEmpleado + los 3 datos de la quincena 
        /// </summary>
        /// <param name="Num"></param>
        /// <param name="Rfc"></param>
        /// <param name="An"></param>
        /// <param name="Folio"></param>
        /// <param name="NombreBanco"></param>
        /// <param name="Cuenta"></param>
        /// <param name="Observa"></param>
        public static int ActualizarBaseNominaEnSql(NumRfcNombreLiquidoDTO ActualizarRegistro, Tbl_CuentasBancarias cuentaDelTrabajador,  string An, string Folio,  string Observa )
        {
            int registrosActualizados = 0;

         
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionLocalInterfaces()))
                {
                    string consulta = "UPDATE interfaces.dbo."+An+" SET Num_che = '"+Folio+"', Banco_x = '"+cuentaDelTrabajador.NombreBanco+"', Cuenta_x = '"+cuentaDelTrabajador.Cuenta+"', Observa = '"+Observa+"' WHERE NUM = '"+ActualizarRegistro.NumeroEmpleado+"' and RFC = '"+ActualizarRegistro.Rfc+"' and LIQUIDO = '"+ActualizarRegistro.Liquido+"' and NOMBRE = '"+ActualizarRegistro.Nombre+"' ";
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);

                   registrosActualizados =  command.ExecuteNonQuery();
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ActualizarBaseNominaEnSql";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return registrosActualizados;
        }

    }
}
