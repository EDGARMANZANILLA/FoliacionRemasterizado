using DAP.Foliacion.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades;

namespace DAP.Foliacion.Negocios
{
    public class FoliarNegocios
    { 

        public static Dictionary<int, string> ObtenerNominasXNumeroQuincena(string NumeroQuincena) 
        {
            List<NombreNominasDTO> nombresNomina = FoliarConsultasDBSinEntity.ObtenerNombreNominas(NumeroQuincena);

            Dictionary<int, string> nombresListosNomina = new Dictionary<int, string>();

            int i = 0;
            foreach (NombreNominasDTO NuevaNomina in nombresNomina)
            {
                i += 01;
                if (NuevaNomina.Adicional == "")
                {        
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]  [ " + NuevaNomina.Nomina + " ] " + " -" + "-" + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }
                else 
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]  [ " + NuevaNomina.Nomina + " ] " + " -" + "-" + "- " + "ADICIONAL" +" -" + "-" + "- " + NuevaNomina.Adicional + " -" + "- " + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }


            }

            return nombresListosNomina;
        }

        public static Dictionary<int, string> ObtenerBancosParaPagomatico() 
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true);

            Dictionary<int, string> bancosMostrar= new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados) 
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " "+cuentaPagomatico.NombreBanco+" "+" - "+" [ "+cuentaPagomatico.Cuenta+" ] ");
            }

            return bancosMostrar;
        }

        /// <summary>
        /// Obtiene los bancos con lo que se pueden 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> ObtenerBancoParaFormasPago()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);

            Dictionary<int, string> bancosMostrar = new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados)
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " " + cuentaPagomatico.NombreBanco + " " + " - " + " [ " + cuentaPagomatico.Cuenta + " ] ");
            }

            return bancosMostrar;
        }






        public static string ObtenerBancoPorID(int Id)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            Tbl_CuentasBancarias resultado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);

            return  resultado.NombreBanco+" - [ "+resultado.Cuenta +" ] "; 
        }




        /*Obtener Detalle (Id, Nombre, cuenta, IdInventaria)De banco para Formas de Cheques*/

        public static IEnumerable<Tbl_CuentasBancarias> ObtenerDetalleBancoFormasDePago() 
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

           return repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);
        }














        /// <summary>
        /// Devuelve el an de como se encuentra en bitacora de una nomina especifica para revisarla o foliarla
        /// </summary>
        /// <param name="IdNum">numero de la nomina que corresponde la seleccion del usuario</param>
        /// <returns></returns>
        public static List<string> ObtenerAnApAdNominaBitacoraPorIdNumConexion(int IdNum) 
        {
          return  FoliarConsultasDBSinEntity.ObtenerAnApAdNominaBitacoraPorIdNumConexion(IdNum);
        
        }



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaRevicion( string NumeroNomina, string An, int Quincena) 
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true ).Select(y => y.Id ).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas) 
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }


         return  FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPagomatico(NumeroNomina, An, Quincena, NombresBanco);
  
        }

        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaPENALRevicion(string NumeroNomina ,string An, int Quincena)
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).Select(y => y.Id).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas)
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }

            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPenalPagomatico(NumeroNomina, An, Quincena, NombresBanco);

            return DatosNomina;
        }


        public static string ObtenerRutaCOmpletaArchivoIdNomina(int IdNomina) 
        {
            return FoliarConsultasDBSinEntity.ObtenerRutaIdNomina(IdNomina);
        }


      
        public static string ObtenerNumeroNominaXIdNumBitacora(int IdNum)
        {
           return FoliarConsultasDBSinEntity.ObtenerNumeroNominaXIdNum(IdNum);
        }



        /// <summary>
        /// Obtine una lista de las nominas que pueden ser Foliadas para que se foleen todas de un solo jalon
        /// recibe como parametro el numero de quincena ejem 2112
        /// </summary>
        /// <param name="Quincena"></param>
        /// <returns></returns>
        public static List<DatosRevicionTodasNominasDTO> ObtenerTodasNominasXQuincena(string NumeroQuincena) {

            return FoliarConsultasDBSinEntity.ObtenerListaDTOTodasNominasXquincena(NumeroQuincena);
        }


        public static DatosBitacoraParaCheque ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(int IdNomina) 
        {
            return FoliarConsultasDBSinEntity.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(IdNomina);
        }


        //Metodos para foliar formas de pagos (cheques)
        public static List<ResumenNominaDTO> ObtenerDetallesNominaChequesParaModal(int IdNomina ) 
        {

            List<ResumenNominaDTO> listaResumenNomina = new List<ResumenNominaDTO>();


           DatosBitacoraParaCheque datosEncontrados = ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(IdNomina);

            //verifica que no haya ocurrido un problema al traer los datos
            if (datosEncontrados.Comentario != "Sin Datos")
            {
                string consulta = null;
                bool foliado = true;


                //verifica que la nomina sea general o desentralizado ya que son las dos unicas que se imprimen en diferentes archivos (Confianza y sindicalizados C-G?2115.tx y S-G?2115.txt)
                if (datosEncontrados.Nomina == "01" || datosEncontrados.Nomina == "02")
                {
                    //Obtener consulta para pintar los totales en el modal 
                    ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(datosEncontrados.An);
                    List<string> listaConsultas = nuevaConsulta.ObtenerConsultasTotalesSindicato();


                    if (listaConsultas.Count() > 0)
                    {
                        //se le envia un true por perteneser a una nomina general o descentralizada
                        List<TotalRegistrosXDelegacionDTO> resultadoTotalRegistros = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaConsultas, true);

                        if (resultadoTotalRegistros.Count() > 0)
                        {
                            foreach (TotalRegistrosXDelegacionDTO registroSeleccionado in resultadoTotalRegistros)
                            {
                                //registroSeleccionado.Delegacion


                                //string deleg = null;


                                switch (registroSeleccionado.Delegacion)
                                {
                                    case "Campeche y Mas":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizado 
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza degacion 00 e inlcuyen ('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16');
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Champoton":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion 03;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Escarcega - Candelaria":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicato
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion 04;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Calkini":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //sindizalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion  05;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Hecelchakan":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion  06;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Hopelchen":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizados
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion 07;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                }


                                ResumenNominaDTO nuevoResumen = new ResumenNominaDTO();
                                nuevoResumen.Delegacion = registroSeleccionado.Delegacion;


                                if (registroSeleccionado.Sindicato)
                                {
                                    nuevoResumen.Sindicalizado = Convert.ToString(registroSeleccionado.Total);
                                }
                                else
                                {
                                    nuevoResumen.Confianza = Convert.ToString( registroSeleccionado.Total);
                                }
                               // nuevoResumen.Otros = 0;

                                nuevoResumen.Foliado = foliado;
                               // nuevoResumen.Total = registroSeleccionado.Total;


                                listaResumenNomina.Add(nuevoResumen);

                            }


                        }
                    }





                } else if (datosEncontrados.Nomina == "08") 
                {
                    //son para las nominas que son de pension alimenticia "08"
                    //Obtener la consulta de los totales para mostrarlos en la tabla del modal de pagoXFormasDePago
                    ConsultasSQLOtrasNominasConCheques consultasPensionAlimenticia = new ConsultasSQLOtrasNominasConCheques();
                    List<string> NuevaPension = consultasPensionAlimenticia.ObtenerConsultaTotalesPencionAlimenticia(datosEncontrados.An);


                    List<TotalRegistrosXDelegacionDTO> resultadoTotalRegistros = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina( NuevaPension, false);

                    if (resultadoTotalRegistros.Count() > 0)
                    {
                        foreach (TotalRegistrosXDelegacionDTO registroSeleccionado in resultadoTotalRegistros)
                        {
                            //registroSeleccionado.Delegacion


                            //string deleg = null;


                            switch (registroSeleccionado.Delegacion)
                            {
                                case "Campeche y Otros":

                                    //Confianza degacion 00 e inlcuyen ('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16');
                                    consulta = "select NUM_CHE from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                                case "Champoton":

                                    //Confianza delegacion 03;
                                    consulta = "select * from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03  order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                                case "Escarcega - Candelaria":

                                    //Confianza delegacion 04;
                                    consulta = "select * from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                                case "Calkini":

                                    //Confianza delegacion  05;
                                    consulta = "select * from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                                case "Hecelchakan":

                                    //Confianza delegacion  06;
                                    consulta = "select * from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06  order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                                case "Hopelchen":

                                    //Confianza delegacion 07;
                                    consulta = "select * from interfaces.dbo."+datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07  order by JUZGADO, NOMBRE";
                                    foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);

                                    break;

                            }


                            ResumenNominaDTO nuevoResumen = new ResumenNominaDTO();



                            nuevoResumen.Delegacion = registroSeleccionado.Delegacion;

                            //nuevoResumen.Sindicalizado = ;
                            //nuevoResumen.Confianza = "";
                            nuevoResumen.Otros = Convert.ToString(registroSeleccionado.Total);

                            nuevoResumen.Foliado = foliado;
                            //  nuevoResumen.Total = registroSeleccionado.Total;


                            listaResumenNomina.Add(nuevoResumen);

                        }


                    }

                }
                else
                {
                    //entra dentro de esta categoria para cualquier otra nomina que no sea la general o la descentralizada osea ni 01 u 02
                    //Consultar totales para la table del modal de formas de pago 
                    ConsultasSQLOtrasNominasConCheques nuevaConsultaOtrasNominas = new ConsultasSQLOtrasNominasConCheques();
                    List<string> listaConsultasObtenidas = nuevaConsultaOtrasNominas.ObtenerConsultasTotalesOtrasNominas(datosEncontrados.An);


                    List<TotalRegistrosXDelegacionDTO> resultadoTotalRegistros = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaConsultasObtenidas, false);

                    if (resultadoTotalRegistros.Count() > 0)
                    {
                        foreach (TotalRegistrosXDelegacionDTO registroSeleccionado in resultadoTotalRegistros)
                        {
                            //registroSeleccionado.Delegacion


                            //string deleg = null;


                            switch (registroSeleccionado.Delegacion)
                            {
                                case "Campeche y Mas":

                                        //Confianza degacion 00 e inlcuyen ('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16');
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                  
                                    break;

                                case "Champoton":

                                        //Confianza delegacion 03;
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                   
                                    break;

                                case "Escarcega - Candelaria":

                                        //Confianza delegacion 04;
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                    
                                    break;

                                case "Calkini":

                                        //Confianza delegacion  05;
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                    
                                    break;

                                case "Hecelchakan":

                                        //Confianza delegacion  06;
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                    
                                    break;

                                case "Hopelchen":

                                        //Confianza delegacion 07;
                                        consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An + " where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                        foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                    
                                    break;

                            }


                            ResumenNominaDTO nuevoResumen = new ResumenNominaDTO();

                           

                            nuevoResumen.Delegacion = registroSeleccionado.Delegacion;

                            //nuevoResumen.Sindicalizado = ;
                            //nuevoResumen.Confianza = "";
                            nuevoResumen.Otros = Convert.ToString( registroSeleccionado.Total );

                            nuevoResumen.Foliado = foliado;
                            //  nuevoResumen.Total = registroSeleccionado.Total;


                            listaResumenNomina.Add(nuevoResumen);

                        }


                    }

                }


            }



            return listaResumenNomina;
        }

        public static string ObtenerNombreModalPorIDNomina(int IdNomina) 
        {
           return FoliarConsultasDBSinEntity.ObtenerNombreModalDetalleNomina(IdNomina);
        }




        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosRevicionPorDelegacionFormasPago(string Consulta, string NumeroNomina, int NumeroChequeInicial, string NombreBanco, bool Inhabilitado, int RangoInhabilitadoInicial, int RangoInhabilitadoFinal) 
        {
            return FoliarConsultasDBSinEntity.ObtenerDatosNominaRevicionFormasDePago(Consulta, NumeroNomina, NumeroChequeInicial, NombreBanco, Inhabilitado, RangoInhabilitadoInicial, RangoInhabilitadoFinal);
        }



        /*OBTENER DELEGACION*/
        public static string ObtenerDelegacionPorId(int Delegacion) 
        {
            string nombreDelegacion = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    nombreDelegacion = "Campeche y otros municipios";
                    break;
                case 3:
                    /*Champoton 03 */
                    nombreDelegacion = "Champoton";
                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    nombreDelegacion = "Escarcega y Candelaria";
                    break;
                case 5:
                    /*Calkini 5 */
                    nombreDelegacion = "Calkini";
                    break;
                case 6:
                    /*Hecelchakan 6 */
                    nombreDelegacion = "Hecelchakan";
                    break;
                case 7:
                    /*Hopelchen 7 */
                    nombreDelegacion = "Hopelchen";
                    break;

            }
            return nombreDelegacion;

        }





        /* FOLIAR    */

        //********************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
                                                                       /*** FOLEAR NOMINAS CON PAGOMATICOS   ***/
       
        public static List<AlertasAlFolearPagomaticosDTO> FolearPagomaticoPorNomina(int IdNom, string quincena, string Observa) 
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();

            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();


            ///obtener el id de los bancos en consulta y luego agragarla dos campos mas para que ahi vengas en nombre del banco y la cuenta bancaria 
            //Consultar el Nombre del banco y la cuenta bancaria
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            List<Tbl_CuentasBancarias> resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).ToList();


            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);



            var datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom);
            var NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina =FoliarConsultasDBSinEntity.ObtenerNumRfcNombreLiquidoDeNominaPAGOMATICO(datosCompletosObtenidos.An, datosCompletosObtenidos.EsPenA);

            //Pregunta si ya fue foleada una vez o si no hay ningun registro de que se haya foliado 
            bool EstaFoliada = FoliarConsultasDBSinEntity.ExiteRegistroDeFolicionNominaEnDBFoliacion(Convert.ToInt32(datosCompletosObtenidos.Quincena), datosCompletosObtenidos.Id_nom, NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count(), 2 /*Por ser pagomatico*/);


            int registrosActualizados = 0;
            int registrosInsertadosOActualizados = 0;

            if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() > 0)
            {
                //key es el numero de empleado
                //value es el RFC 
                foreach (NumRfcNombreLiquidoDTO registroSeleccionado in NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina)
                {
                    Tbl_Pagos nuevoPago = new Tbl_Pagos();

                    Tbl_CuentasBancarias bancoYCuentaDelTrabajador = resultadoDatosBanco.Where(x => x.Id == Convert.ToInt32(registroSeleccionado.IdCuentaBancaria)).FirstOrDefault();
                    int registroAfectado = 0;


                    int folio = (Convert.ToInt32( registroSeleccionado.NumeroEmpleado)+ Convert.ToInt32( quincena));

                    if (datosCompletosObtenidos.Nomina == "08")
                    {
                        ///Crear un update para pension alimenticia
                        registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaPenAEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, folio, Observa);
                    }
                    else
                    {   //NumRfcNombreLiquidoDTO, List<Tbl_CuentasBancarias> , string An, string Folio,  string Observa 
                        registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, folio, Observa);
                    }




                    //Si hay un registro afectado significa que se actualizo en la base de datos y por ende hay que guardarlo en la DB de foliacion para tener un registro de 
                    //de lo que se ha foliado, esto servira para el buscador de cheques.
                    //Si es la primera vez que se folea se hace un Update
                    //si es 2 o mas veces que se folea hay que hacer un update 
                    if (registroAfectado >= 1)
                    {
                        registrosActualizados += registroAfectado;
                    }




                    if (EstaFoliada && registroAfectado >= 1)
                    {
                        int quinenaAbuscar = Convert.ToInt32(datosCompletosObtenidos.Quincena);

                        Tbl_Pagos pagoAmodificar = new Tbl_Pagos();

                        if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                        {
                            pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quinenaAbuscar && x.NumEmpleado == registroSeleccionado.NumeroEmpleado && x.ImporteLiquido == registroSeleccionado.Liquido && x.NumBeneficiario == registroSeleccionado.NumBeneficiario);
                        }
                        else 
                        {
                            pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quinenaAbuscar && x.NumEmpleado == registroSeleccionado.NumeroEmpleado && x.ImporteLiquido == registroSeleccionado.Liquido );
                        }



                        //si entra es por que esta foliada y se hara solo un update
                        pagoAmodificar.Id_nom = datosCompletosObtenidos.Id_nom;
                        pagoAmodificar.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                        pagoAmodificar.An = datosCompletosObtenidos.An;
                        pagoAmodificar.Adicional = datosCompletosObtenidos.Adicional;
                        pagoAmodificar.Mes = datosCompletosObtenidos.Mes;
                        pagoAmodificar.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                        pagoAmodificar.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                        pagoAmodificar.RfcEmpleado = registroSeleccionado.Rfc;
                        pagoAmodificar.NumEmpleado = registroSeleccionado.NumeroEmpleado;

                        if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                        {
                            pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(registroSeleccionado.NumeroEmpleado); //conectarse a funcion y buscar nombre 
                            pagoAmodificar.BeneficiarioPenA = registroSeleccionado.Nombre;
                            pagoAmodificar.NumBeneficiario = registroSeleccionado.NumBeneficiario;
                        }
                        else
                        {
                            pagoAmodificar.NombreEmpleado = registroSeleccionado.Nombre;
                            pagoAmodificar.BeneficiarioPenA = null;
                            pagoAmodificar.NumBeneficiario = null;
                        }


                        pagoAmodificar.EsPenA = datosCompletosObtenidos.EsPenA;

                        //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                        pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = bancoYCuentaDelTrabajador.Id;
                        pagoAmodificar.FolioCheque = folio;
                        pagoAmodificar.ImporteLiquido = registroSeleccionado.Liquido;
                        pagoAmodificar.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                        //1 = Transito, 2= Pagado
                        //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                        pagoAmodificar.IdCat_EstadoPago_Pagos = 1;


                        string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || " + datosCompletosObtenidos.Nomina + " || " + datosCompletosObtenidos.Quincena + " || " + registroSeleccionado.NumeroEmpleado + " || " + registroSeleccionado.Liquido + " || " + folio + " || " + registroSeleccionado.NumBeneficiario;
                        EncriptarCadena encriptar = new EncriptarCadena();

                        pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);


                        // nuevoPago.IdReferenciaCancelados_Pagos =;
                        // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                        //Prueba para ver que todo cambio
                       // pagoAmodificar.EsRefoliado = true;
                        pagoAmodificar.Activo = true;

                        Tbl_Pagos pagoModificado = repositorioTblPago.Modificar(pagoAmodificar);

                        if (!string.IsNullOrEmpty(Convert.ToString( pagoModificado.FolioCheque ))) 
                        {
                            registrosInsertadosOActualizados++;
                        }
                        

                    }
                    else 
                    {
                        //si entra es por que no esta foliada y se haran inserts 
                        nuevoPago.Id_nom = datosCompletosObtenidos.Id_nom;
                        nuevoPago.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                        nuevoPago.An = datosCompletosObtenidos.An;
                        nuevoPago.Adicional = datosCompletosObtenidos.Adicional;
                        nuevoPago.Mes = datosCompletosObtenidos.Mes;
                        nuevoPago.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                        nuevoPago.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                        nuevoPago.RfcEmpleado = registroSeleccionado.Rfc;
                        nuevoPago.NumEmpleado = registroSeleccionado.NumeroEmpleado;

                        if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                        {
                            nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(registroSeleccionado.NumeroEmpleado); //conectarse a funcion y buscar nombre 
                            nuevoPago.BeneficiarioPenA = registroSeleccionado.Nombre;
                            nuevoPago.NumBeneficiario = registroSeleccionado.NumBeneficiario;
                        }
                        else
                        {
                            nuevoPago.NombreEmpleado = registroSeleccionado.Nombre;
                            nuevoPago.BeneficiarioPenA = null;
                            nuevoPago.NumBeneficiario = null;
                        }

                        nuevoPago.EsPenA = datosCompletosObtenidos.EsPenA;
                        //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = bancoYCuentaDelTrabajador.Id;
                        nuevoPago.FolioCheque = folio;
                        nuevoPago.ImporteLiquido = registroSeleccionado.Liquido;
                        nuevoPago.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                        //1 = Transito, 2= Pagado
                        //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                        nuevoPago.IdCat_EstadoPago_Pagos = 1;


                        string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || "+datosCompletosObtenidos.Nomina+" || "+datosCompletosObtenidos.Quincena+" || "+registroSeleccionado.NumeroEmpleado+" || "+registroSeleccionado.Liquido+" || "+folio +" || "+registroSeleccionado.NumBeneficiario;
                        EncriptarCadena encriptar = new  EncriptarCadena();
                        
                        nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                        // nuevoPago.IdReferenciaCancelados_Pagos =;
                        // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                        // nuevoPago.EsRefoliado =;
                        nuevoPago.Activo = true;

                        Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar(nuevoPago);


                        if (!string.IsNullOrEmpty(Convert.ToString( pagoInsertado.FolioCheque)))
                        {
                            registrosInsertadosOActualizados++;
                        }

                    }




                }



            }
            else if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() == 0)
            {
                
                nuevaAlerta.IdAtencion = 1;

                nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                nuevaAlerta.Detalle = "SIN PAGOMATICOS";
                nuevaAlerta.Solucion = "F/FP";
                nuevaAlerta.Id_Nom = Convert.ToString( datosCompletosObtenidos.Id_nom);
                nuevaAlerta.RegistrosFoliados = registrosActualizados;

                registrosActualizados = 0;
                Advertencias.Add(nuevaAlerta);
               // return nuevaAlerta;
            }



            //si se cumple todos los registros se actualizaron correctamente 
            if (registrosActualizados == NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() && registrosActualizados > 0 && registrosInsertadosOActualizados == registrosActualizados)
            {
               
                nuevaAlerta.IdAtencion = 0;

                nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                nuevaAlerta.Detalle = "";
                nuevaAlerta.Solucion = "";
                nuevaAlerta.Id_Nom = Convert.ToString( datosCompletosObtenidos.Id_nom);
                nuevaAlerta.RegistrosFoliados = registrosActualizados;
               
                registrosActualizados = 0;

                Advertencias.Add(nuevaAlerta);
                //return nuevaAlerta;

            }
            else if (registrosActualizados != NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() && registrosActualizados > 0)
            {
                //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 

              
                nuevaAlerta.IdAtencion = 2;

                nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
                nuevaAlerta.Solucion = "IFNN";
                nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos.Id_nom);
                nuevaAlerta.RegistrosFoliados = registrosActualizados;
                
                registrosActualizados = 0;

                Advertencias.Add(nuevaAlerta);
                //return nuevaAlerta;
            }





            return Advertencias;
        }



        public static List<AlertasAlFolearPagomaticosDTO> FolearPagomaticoTodasLasNominas(string Quincena, string Observa)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias  = new List<AlertasAlFolearPagomaticosDTO>();

           // AlertasAlFolearPagomaticosDTO nuevoError = new AlertasAlFolearPagomaticosDTO();


          //  List<string> erroresEnFoliacion = new List<string>();

            ///obtener el id de los bancos en consulta y luego agragarla dos campos mas para que ahi vengas en nombre del banco y la cuenta bancaria 
            ///


            //Consultar el Nombre del banco y la cuenta bancaria
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            List<Tbl_CuentasBancarias> resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).ToList();

            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);





            List<DatosRevicionTodasNominasDTO> DetallesNominasObtenidos = ObtenerTodasNominasXQuincena(Quincena);
            List<NumRfcNombreLiquidoDTO> NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina = null;
            int registrosActualizados = 0;

            foreach (DatosRevicionTodasNominasDTO nuevoRegitroObtenido in DetallesNominasObtenidos) 
            {
                //Sirve para pruebas y no tener que folear las nominas aqui descritas
                //if (nuevoRegitroObtenido.Nomina.Equals("01") || nuevoRegitroObtenido.Nomina.Equals("05") || nuevoRegitroObtenido.Nomina.Equals("02") || nuevoRegitroObtenido.Nomina.Equals("08") || nuevoRegitroObtenido.Nomina.Equals("07"))
                //{
                
                
                //}
                //else { 



                    int TresDigitosQuincena = Convert.ToInt32(Quincena.Substring(1, 3));

                    var datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(Convert.ToInt32(nuevoRegitroObtenido.Id_Nom));

                    NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina = FoliarConsultasDBSinEntity.ObtenerNumRfcNombreLiquidoDeNominaPAGOMATICO(datosCompletosObtenidos.An, datosCompletosObtenidos.EsPenA);


                    //Pregunta si ya fue foleada una vez o si no hay ningun registro de que se haya foliado 
                    bool EstaFoliada = FoliarConsultasDBSinEntity.ExiteRegistroDeFolicionNominaEnDBFoliacion(Convert.ToInt32(datosCompletosObtenidos.Quincena), datosCompletosObtenidos.Id_nom, NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count(), 2 /*Por ser pagomatico*/);

                    int registrosInsertadosOActualizados = 0;
                    //Se usa para devolverle al cliente una lista de lo que ocurrio durante el proceso 
                    AlertasAlFolearPagomaticosDTO nuevAdvertencia = new AlertasAlFolearPagomaticosDTO();

                    if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() > 0)
                    {

                        //key es el numero de empleado
                        //value es el RFC 
                        foreach (NumRfcNombreLiquidoDTO registroSeleccionado in NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina)
                        {
                            Tbl_Pagos pagoAmodificar = new Tbl_Pagos();
                            Tbl_Pagos nuevoPago = new Tbl_Pagos();

                            Tbl_CuentasBancarias bancoYCuentaDelTrabajador = resultadoDatosBanco.Where(x => x.Id == Convert.ToInt32(registroSeleccionado.IdCuentaBancaria)).FirstOrDefault();
                            int registroAfectado = 0;
                            int folio =  Convert.ToInt32(registroSeleccionado.NumeroEmpleado)+TresDigitosQuincena;


                            int numChe = Convert.ToInt32(registroSeleccionado.NumeroEmpleado)+TresDigitosQuincena;
                            if (datosCompletosObtenidos.Nomina == "08")
                            {
                                ///Crear un update para pension alimenticia
                                registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaPenAEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, numChe, Observa);
                            }
                            else
                            {   //NumRfcNombreLiquidoDTO, List<Tbl_CuentasBancarias> , string An, string Folio,  string Observa 
                                registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, numChe, Observa);
                            }




                            if (registroAfectado >= 1)
                            {
                                registrosActualizados += registroAfectado;
                            }


                            if (EstaFoliada && registroAfectado >= 1)
                            {
                                int quinenaAbuscar = Convert.ToInt32(datosCompletosObtenidos.Quincena);

                                if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                                {
                                    pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quinenaAbuscar && x.NumEmpleado == registroSeleccionado.NumeroEmpleado && x.ImporteLiquido == registroSeleccionado.Liquido && x.NumBeneficiario == registroSeleccionado.NumBeneficiario);
                                }
                                else
                                {
                                    pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quinenaAbuscar && x.NumEmpleado == registroSeleccionado.NumeroEmpleado && x.ImporteLiquido == registroSeleccionado.Liquido);
                                }



                                //si entra es por que esta foliada y se hara solo un update
                                pagoAmodificar.Id_nom = datosCompletosObtenidos.Id_nom;
                                pagoAmodificar.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                                pagoAmodificar.An = datosCompletosObtenidos.An;
                                pagoAmodificar.Adicional = datosCompletosObtenidos.Adicional;
                                pagoAmodificar.Mes = datosCompletosObtenidos.Mes;
                                pagoAmodificar.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                                pagoAmodificar.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                                pagoAmodificar.RfcEmpleado = registroSeleccionado.Rfc;
                                pagoAmodificar.NumEmpleado = registroSeleccionado.NumeroEmpleado;

                                if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                                {
                                    pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(registroSeleccionado.NumeroEmpleado); //conectarse a funcion y buscar nombre 
                                    pagoAmodificar.BeneficiarioPenA = registroSeleccionado.Nombre;
                                    pagoAmodificar.NumBeneficiario = registroSeleccionado.NumBeneficiario;
                                }
                                else
                                {
                                    pagoAmodificar.NombreEmpleado = registroSeleccionado.Nombre;
                                    pagoAmodificar.BeneficiarioPenA = null;
                                    pagoAmodificar.NumBeneficiario = null;
                                }


                                pagoAmodificar.EsPenA = datosCompletosObtenidos.EsPenA;

                                //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                                pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = bancoYCuentaDelTrabajador.Id;
                                pagoAmodificar.FolioCheque = folio;
                                pagoAmodificar.ImporteLiquido = registroSeleccionado.Liquido;
                                pagoAmodificar.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                                //1 = Transito, 2= Pagado
                                //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                                pagoAmodificar.IdCat_EstadoPago_Pagos = 1;


                                string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || " + datosCompletosObtenidos.Nomina + " || " + datosCompletosObtenidos.Quincena + " || " + registroSeleccionado.NumeroEmpleado + " || " + registroSeleccionado.Liquido + " || " + folio + " || " + registroSeleccionado.NumBeneficiario;
                                EncriptarCadena encriptar = new EncriptarCadena();

                                pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);


                                // nuevoPago.IdReferenciaCancelados_Pagos =;
                                // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                                //Prueba para ver que todo cambio
                               // pagoAmodificar.EsRefoliado = true;
                                pagoAmodificar.Activo = true;

                                Tbl_Pagos pagoModificado = repositorioTblPago.Modificar(pagoAmodificar);

                                if (!string.IsNullOrEmpty(Convert.ToString( pagoModificado.FolioCheque)))
                                {
                                    registrosInsertadosOActualizados++;
                                }


                            }
                            else
                            {
                                //si entra es por que no esta foliada y se haran inserts 
                                nuevoPago.Id_nom = datosCompletosObtenidos.Id_nom;
                                nuevoPago.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                                nuevoPago.An = datosCompletosObtenidos.An;
                                nuevoPago.Adicional = datosCompletosObtenidos.Adicional;
                                nuevoPago.Mes = datosCompletosObtenidos.Mes;
                                nuevoPago.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                                nuevoPago.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                                nuevoPago.RfcEmpleado = registroSeleccionado.Rfc;
                                nuevoPago.NumEmpleado = registroSeleccionado.NumeroEmpleado;

                                if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                                {
                                    nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(registroSeleccionado.NumeroEmpleado); //conectarse a funcion y buscar nombre 
                                    nuevoPago.BeneficiarioPenA = registroSeleccionado.Nombre;
                                    nuevoPago.NumBeneficiario = registroSeleccionado.NumBeneficiario;
                                }
                                else
                                {
                                    nuevoPago.NombreEmpleado = registroSeleccionado.Nombre;
                                    nuevoPago.BeneficiarioPenA = null;
                                    nuevoPago.NumBeneficiario = null;
                                }

                                nuevoPago.EsPenA = datosCompletosObtenidos.EsPenA;
                                //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                                nuevoPago.IdTbl_CuentaBancaria_BancoPagador = bancoYCuentaDelTrabajador.Id;
                                nuevoPago.FolioCheque = folio;
                                nuevoPago.ImporteLiquido = registroSeleccionado.Liquido;
                                nuevoPago.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                                //1 = Transito, 2= Pagado
                                //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                                nuevoPago.IdCat_EstadoPago_Pagos = 1;


                                string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || " + datosCompletosObtenidos.Nomina + " || " + datosCompletosObtenidos.Quincena + " || " + registroSeleccionado.NumeroEmpleado + " || " + registroSeleccionado.Liquido + " || " + folio + " || " + registroSeleccionado.NumBeneficiario;
                                EncriptarCadena encriptar = new EncriptarCadena();

                                nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                                // nuevoPago.IdReferenciaCancelados_Pagos =;
                                // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                                // nuevoPago.EsRefoliado =;
                                nuevoPago.Activo = true;

                                Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar(nuevoPago);


                                if (!string.IsNullOrEmpty(Convert.ToString( pagoInsertado.FolioCheque)))
                                {
                                    registrosInsertadosOActualizados++;
                                }

                            }




                        }
                    }
                    else if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() == 0)
                    {
                        nuevAdvertencia.IdAtencion = 1;
                        
                        nuevAdvertencia.NumeroNomina = datosCompletosObtenidos.Nomina;
                        nuevAdvertencia.NombreNomina = datosCompletosObtenidos.Coment;
                        nuevAdvertencia.Detalle = "SIN PAGOMATICOS";
                        nuevAdvertencia.Solucion = "F/FP";
                        nuevAdvertencia.Id_Nom = nuevoRegitroObtenido.Id_Nom ;
                        nuevAdvertencia.RegistrosFoliados = registrosActualizados;
                        Advertencias.Add(nuevAdvertencia);
                        registrosActualizados = 0;

                    }



                    if (registrosActualizados == NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() && registrosActualizados > 0)
                    {
                        nuevAdvertencia.IdAtencion = 0;

                        nuevAdvertencia.NumeroNomina = datosCompletosObtenidos.Nomina;
                        nuevAdvertencia.NombreNomina = datosCompletosObtenidos.Coment;
                        nuevAdvertencia.Detalle = "";
                        nuevAdvertencia.Solucion = "";
                        nuevAdvertencia.Id_Nom = nuevoRegitroObtenido.Id_Nom ;
                        nuevAdvertencia.RegistrosFoliados = registrosActualizados;
                        Advertencias.Add(nuevAdvertencia);
                        registrosActualizados = 0;

                    }
                    else if (registrosActualizados != NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() && registrosActualizados > 0)
                    {
                        nuevAdvertencia.IdAtencion = 2;

                        nuevAdvertencia.NumeroNomina = datosCompletosObtenidos.Nomina;
                        nuevAdvertencia.NombreNomina = datosCompletosObtenidos.Coment;
                        nuevAdvertencia.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
                        nuevAdvertencia.Solucion = "IFNN";
                        nuevAdvertencia.Id_Nom = nuevoRegitroObtenido.Id_Nom ;
                        nuevAdvertencia.RegistrosFoliados = registrosActualizados;                        
                        Advertencias.Add(nuevAdvertencia);
                        registrosActualizados = 0;

                    }

                //final de las pruebas para no tener que folear las nominas arriba descritas
                // }

            }




            return Advertencias;
        }










        //********************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
                                                                    /*** FOLEAR CHEQUES  (FORMAS DE PAGO)  ***/
        public static List<AlertasAlFolearPagomaticosDTO> FoliarChequesPorNomina(FoliarFormasPagoDTO NuevaNominaFoliar , string Observa)
        {

            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();


            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            Tbl_CuentasBancarias bancoEncontrado = repositorio.Obtener(x => x.Id == NuevaNominaFoliar.IdBancoPagador && x.Activo == true);





            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);




            var datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(NuevaNominaFoliar.IdNomina);
            bool EstaFoliada = false;
            string consultaPersonal = "";
            int numeroRegistrosActualizados = 0;
            int registrosInsertadosOActualizados = 0; 
            // 1 = le pertenece a las nominas general y descentralizada
            // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 
            if (NuevaNominaFoliar.GrupoNomina == 1)
            {
                //Obtener la consulta a la que corresponde la delegacion para la nomina general y descentralizada
                ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(datosCompletosObtenidos.An);
                consultaPersonal = nuevaConsulta.ObtenerConsultaSindicatoFormasDePagoGeneralYDesc(NuevaNominaFoliar.Delegacion, NuevaNominaFoliar.Sindicato);
                //ObtenerResumenDatosFormasDePagoFoliar(string ConsultaSql, int NumeroChequeInicial, string NombreBanco, bool Inhabilitado, int RangoInhabilitadoInicial, int RangoInhabilitadoFinal)
                List<ResumenPersonalAFoliarPorChequesDTO> resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos.EsPenA, Observa ,consultaPersonal, bancoEncontrado,  NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado,Convert.ToInt32( NuevaNominaFoliar.RangoInhabilitadoInicial) , Convert.ToInt32( NuevaNominaFoliar.RangoInhabilitadoFinal ) );

               // List<NumRfcNombreLiquidoDTO> DetallePersonalConChequeObtenido = FoliarConsultasDBSinEntity.ObtenerDetalleDeEmpleadoEnNominaCheques(consultaPersonal, datosCompletosObtenidos.EsPenA);


            }
            else if (NuevaNominaFoliar.GrupoNomina == 2)
            {
                //para las nominas que no son pension
                ConsultasSQLOtrasNominasConCheques crearConsultaNominasSinSindicalizados = new ConsultasSQLOtrasNominasConCheques();

                //Resultado del personal que se foliara 
                List<ResumenPersonalAFoliarPorChequesDTO> resumenPersonalFoliar = new List<ResumenPersonalAFoliarPorChequesDTO>();




                if ( datosCompletosObtenidos.Nomina.Equals("08") )
                {
                    //para las nominas que si son pension 
                    consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticiaFoliar(NuevaNominaFoliar.Delegacion, datosCompletosObtenidos.An);
                    resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos.EsPenA, Observa, consultaPersonal, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));

                    //List<NumRfcNombreLiquidoDTO> DetallePersonalConChequeObtenido = FoliarConsultasDBSinEntity.ObtenerDetalleDeEmpleadoEnNominaCheques(consultaPersonal, datosCompletosObtenidos.EsPenA);

                }
                else 
                {
                    consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerConsultaConOrdenamientoFormasDePagoFoliar(NuevaNominaFoliar.Delegacion, datosCompletosObtenidos.An);
                    resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosObtenidos.EsPenA, Observa, consultaPersonal, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));

                    // List<NumRfcNombreLiquidoDTO> DetallePersonalConChequeObtenido = FoliarConsultasDBSinEntity.ObtenerDetalleDeEmpleadoEnNominaCheques(consultaPersonal, datosCompletosObtenidos.EsPenA);

                }


                
           
                if (resumenPersonalFoliar.Count() >= 1) 
                {
                    foreach (ResumenPersonalAFoliarPorChequesDTO nuevaPersona in resumenPersonalFoliar) 
                    {
                        Tbl_Pagos nuevoPago = new Tbl_Pagos();


                        int registroAfectado = 0;
                        registroAfectado =  FoliarConsultasDBSinEntity.ActualizarBaseNominaParaCheques(nuevaPersona, datosCompletosObtenidos.An, datosCompletosObtenidos.EsPenA);



                        if (registroAfectado >= 1)
                        {
                            numeroRegistrosActualizados += registroAfectado;
                        }

                        //DatosCompletosBitacoraParaChequesDTO resumenNomina,  int TipoPago, ResumenPersonalAFoliarPorChequesDTO PersonalAverificar 
                        EstaFoliada = FoliarConsultasDBSinEntity.ExiteRegistroPersonaEnDBFoliacion( datosCompletosObtenidos, 1 /*Por ser cheques*/, nuevaPersona );
                        ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
                        if (EstaFoliada && registroAfectado >= 1)
                        {
                            Tbl_Pagos pagoAmodificar = new Tbl_Pagos();

                            int quincena = Convert.ToInt32( datosCompletosObtenidos.Quincena );

                            if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                            {
                                pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quincena && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
                            }
                            else
                            {
                                pagoAmodificar = repositorioTblPago.Obtener(x => x.Id_nom == datosCompletosObtenidos.Id_nom && x.Quincena == quincena && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
                            }



                            //si entra es por que esta foliada y se hara solo un update
                            pagoAmodificar.Id_nom = datosCompletosObtenidos.Id_nom;
                            pagoAmodificar.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                            pagoAmodificar.An = datosCompletosObtenidos.An;
                            pagoAmodificar.Adicional = datosCompletosObtenidos.Adicional;
                            pagoAmodificar.Mes = datosCompletosObtenidos.Mes;
                            pagoAmodificar.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                            pagoAmodificar.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                            pagoAmodificar.RfcEmpleado = nuevaPersona.RFC ;
                            pagoAmodificar.NumEmpleado = nuevaPersona.NumEmpleado;

                            if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                            {
                                pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.NumEmpleado); //conectarse a funcion y buscar nombre 
                                pagoAmodificar.BeneficiarioPenA = nuevaPersona.Nombre;
                                pagoAmodificar.NumBeneficiario = nuevaPersona.NumBeneficiario;
                            }
                            else
                            {
                                pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;
                                pagoAmodificar.BeneficiarioPenA = null;
                                pagoAmodificar.NumBeneficiario = null;
                            }


                            pagoAmodificar.EsPenA = datosCompletosObtenidos.EsPenA;

                            //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                            pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
                            pagoAmodificar.FolioCheque = nuevaPersona.NumChe;
                            pagoAmodificar.ImporteLiquido = nuevaPersona.Liquido;
                            pagoAmodificar.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 

                            //1 = Transito, 2= Pagado
                            //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                            pagoAmodificar.IdCat_EstadoPago_Pagos = 1;


                            string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || " + datosCompletosObtenidos.Nomina + " || " + datosCompletosObtenidos.Quincena + " || " + nuevaPersona.NumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.NumBeneficiario;
                            EncriptarCadena encriptar = new EncriptarCadena();

                            pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);


                            // nuevoPago.IdReferenciaCancelados_Pagos =;
                            // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                            //Prueba para ver que todo cambio
                            // pagoAmodificar.EsRefoliado = true;
                            pagoAmodificar.Activo = true;

                            Tbl_Pagos pagoModificado = repositorioTblPago.Modificar(pagoAmodificar);

                            if (!string.IsNullOrEmpty( Convert.ToString( pagoModificado.FolioCheque )))
                            {
                                registrosInsertadosOActualizados++;
                            }


                        }
                        else
                        {
                            //si entra es por que no esta foliada y se haran inserts 
                            nuevoPago.Id_nom = datosCompletosObtenidos.Id_nom;
                            nuevoPago.Nomina = Convert.ToInt32(datosCompletosObtenidos.Nomina);
                            nuevoPago.An = datosCompletosObtenidos.An;
                            nuevoPago.Adicional = datosCompletosObtenidos.Adicional;
                            nuevoPago.Mes = datosCompletosObtenidos.Mes;
                            nuevoPago.Quincena = Convert.ToInt32(datosCompletosObtenidos.Quincena);
                            nuevoPago.ReferenciaBitacora = datosCompletosObtenidos.ReferenciaBitacora;
                            nuevoPago.RfcEmpleado = nuevaPersona.RFC;
                            nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;

                            if (datosCompletosObtenidos.EsPenA && datosCompletosObtenidos.Nomina.Equals("08"))
                            {
                                nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.NumEmpleado); //conectarse a funcion y buscar nombre 
                                nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
                                nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;
                            }
                            else
                            {
                                nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
                                nuevoPago.BeneficiarioPenA = null;
                                nuevoPago.NumBeneficiario = null;
                            }

                            nuevoPago.EsPenA = datosCompletosObtenidos.EsPenA;
                            //nuevoPago.IdTbl_InventarioDetalle = 0 ; //No tiene un detalle de inventario ya que no es un cheque
                            nuevoPago.IdTbl_CuentaBancaria_BancoPagador = bancoEncontrado.Id;
                            nuevoPago.FolioCheque = nuevaPersona.NumChe;
                            nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
                            nuevoPago.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 

                            //1 = Transito, 2= Pagado
                            //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado
                            nuevoPago.IdCat_EstadoPago_Pagos = 1;


                            string cadenaDeIntegridad = datosCompletosObtenidos.Id_nom + " || " + datosCompletosObtenidos.Nomina + " || " + datosCompletosObtenidos.Quincena + " || " + nuevaPersona.NumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe+ " || " + nuevaPersona.NumBeneficiario;
                            EncriptarCadena encriptar = new EncriptarCadena();

                            nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                            // nuevoPago.IdReferenciaCancelados_Pagos =;
                            // nuevoPago.IdCat_EstadoCancelados_Pagos =;
                            // nuevoPago.EsRefoliado =;
                            nuevoPago.Activo = true;

                            Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar(nuevoPago);


                            if (!string.IsNullOrEmpty( Convert.ToString( pagoInsertado.FolioCheque )))
                            {
                                registrosInsertadosOActualizados++;
                            }

                        }



















                    }






                }
                else if (resumenPersonalFoliar.Count() == 0)
                {

                    nuevaAlerta.IdAtencion = 1;

                    nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                    nuevaAlerta.Detalle = "SIN PAGOMATICOS";
                    nuevaAlerta.Solucion = "F/FP";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos.Id_nom);
                    nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados;
                    nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

                    numeroRegistrosActualizados = 0;
                    Advertencias.Add(nuevaAlerta);
                    // return nuevaAlerta;
                }



                //si se cumple todos los registros se actualizaron correctamente 
                if (numeroRegistrosActualizados == resumenPersonalFoliar.Count() && numeroRegistrosActualizados > 0 && registrosInsertadosOActualizados == numeroRegistrosActualizados)
                {

                    nuevaAlerta.IdAtencion = 0;

                    nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                    nuevaAlerta.Detalle = "";
                    nuevaAlerta.Solucion = "";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos.Id_nom);
                    nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados;
                    nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

                    numeroRegistrosActualizados = 0;

                    Advertencias.Add(nuevaAlerta);
                    //return nuevaAlerta;

                }
                else if (numeroRegistrosActualizados != resumenPersonalFoliar.Count() && numeroRegistrosActualizados > 0)
                {
                    //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 


                    nuevaAlerta.IdAtencion = 2;

                    nuevaAlerta.NumeroNomina = datosCompletosObtenidos.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosObtenidos.Coment;
                    nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
                    nuevaAlerta.Solucion = "IFNN";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosObtenidos.Id_nom);
                    nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados;
                    nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

                    numeroRegistrosActualizados = 0;

                    Advertencias.Add(nuevaAlerta);
                    //return nuevaAlerta;
                }







            }


            return Advertencias;
        }



    }
}
