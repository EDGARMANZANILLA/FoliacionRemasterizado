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

        /* FOLIAR PAGOMATICOS  */
        public static List<string> FolearPagomaticoPorNomina(int IdNom, string quincena, string Observa) 
        {
            List<string> erroresEnFoliacion = new List<string>();
       

            ///obtener el id de los bancos en consulta y luego agragarla dos campos mas para que ahi vengas en nombre del banco y la cuenta bancaria 
            ///


            //Consultar el Nombre del banco y la cuenta bancaria
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            List<Tbl_CuentasBancarias> resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).ToList();

            


            var datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom);
            
            var NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina =FoliarConsultasDBSinEntity.ObtenerNumRfcNombreLiquidoDeNomina(datosCompletosObtenidos.An, datosCompletosObtenidos.EsPenA);


            int registrosActualizados = 0;

            if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() > 0)
            {
                //key es el numero de empleado
                //value es el RFC 
                foreach (NumRfcNombreLiquidoDTO registroSeleccionado in NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina)
                {

                    Tbl_CuentasBancarias bancoYCuentaDelTrabajador = resultadoDatosBanco.Where(x => x.Id == Convert.ToInt32(registroSeleccionado.IdCuentaBancaria)).FirstOrDefault();
                    int registroAfectado = 0;


                    //NumRfcNombreLiquidoDTO, List<Tbl_CuentasBancarias> , string An, string Folio,  string Observa 
                    registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, registroSeleccionado.NumeroEmpleado + quincena, Observa);


                    if (registroAfectado >= 1)
                    {
                        registrosActualizados += registroAfectado;
                    }
                    else
                    {
                        if (erroresEnFoliacion.Count() == 0)
                        {
                            erroresEnFoliacion.Add("El numero de empleado: " + registroSeleccionado.NumeroEmpleado + " de la nomina " + IdNom + " no se pudo folear correctamente ");
                        }
                        else
                        {
                            erroresEnFoliacion.Add("Error en el empleado: " + registroSeleccionado.NumeroEmpleado);
                        }
                    }
                }
            }
            else 
            {
                erroresEnFoliacion.Add("Error: no existe ningun empleado con tarjeta para folear en esta nomina. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
                return erroresEnFoliacion;
            }




            if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() == registrosActualizados) 
            {
                return erroresEnFoliacion;
            }


            return erroresEnFoliacion;
        }



        public static List<string> FolearPagomaticoTodasLasNominas(string Quincena, string Observa)
        {
        

            List<string> erroresEnFoliacion = new List<string>();

            ///obtener el id de los bancos en consulta y luego agragarla dos campos mas para que ahi vengas en nombre del banco y la cuenta bancaria 
            ///


            //Consultar el Nombre del banco y la cuenta bancaria
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            List<Tbl_CuentasBancarias> resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).ToList();



            List<DatosRevicionTodasNominasDTO> DetallesNominasObtenidos = ObtenerTodasNominasXQuincena(Quincena);

            List<NumRfcNombreLiquidoDTO> NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina = null;
            int registrosActualizados = 0;

            foreach (DatosRevicionTodasNominasDTO nuevoRegitroObtenido in DetallesNominasObtenidos) 
            {
                int TresDigitosQuincena = Convert.ToInt32(Quincena.Substring(1, 3));

                var datosCompletosObtenidos = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(Convert.ToInt32( nuevoRegitroObtenido.Id_Nom));

                 NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina = FoliarConsultasDBSinEntity.ObtenerNumRfcNombreLiquidoDeNomina(datosCompletosObtenidos.An,  datosCompletosObtenidos.EsPenA );

                if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() > 0)
                {


                    //key es el numero de empleado
                    //value es el RFC 
                    foreach (NumRfcNombreLiquidoDTO registroSeleccionado in NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina)
                    {

                        Tbl_CuentasBancarias bancoYCuentaDelTrabajador = resultadoDatosBanco.Where(x => x.Id == Convert.ToInt32(registroSeleccionado.IdCuentaBancaria)).FirstOrDefault();
                        int registroAfectado = 0;


                        //NumRfcNombreLiquidoDTO, List<Tbl_CuentasBancarias> , string An, string Folio,  string Observa 
                        registroAfectado = FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(registroSeleccionado, bancoYCuentaDelTrabajador, datosCompletosObtenidos.An, registroSeleccionado.NumeroEmpleado+TresDigitosQuincena, Observa);


                        if (registroAfectado >= 1)
                        {
                            registrosActualizados += registroAfectado;
                        }
                        else
                        {
                     
                                erroresEnFoliacion.Add("El numero de empleado: " + registroSeleccionado.NumeroEmpleado + " de la nomina " + nuevoRegitroObtenido.Id_Nom + " no se pudo folear correctamente "+ "/n");
                    
                        }
                    }
                }
                else
                {
                    
                    erroresEnFoliacion.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : "+nuevoRegitroObtenido.Id_Nom+". Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
                     
                }



            }



            if (NumRfcNombreLiquidoNombreBancoIdCuentaObtenidosDeNomina.Count() == registrosActualizados)
            {
                return erroresEnFoliacion;
            }


            return erroresEnFoliacion;
        }


    }
}
