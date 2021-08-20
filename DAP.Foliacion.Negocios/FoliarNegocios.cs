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



        /// <summary>
        /// Devuelve el an de como se encuentra en bitacora de una nomina especifica para revisarla o foliarla
        /// </summary>
        /// <param name="IdNum">numero de la nomina que corresponde la seleccion del usuario</param>
        /// <returns></returns>
        public static List<string> ObtenerAnBitacoraIdNum(int IdNum) 
        {
          return  FoliarConsultasDBSinEntity.ObtenerAnBitacoraPorIdNumConexion(IdNum);
        
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


            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPagomatico(NumeroNomina, An, Quincena, NombresBanco);
  
            return DatosNomina;
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


        //Metodos para foliar formas de pagos (cheques)
        public static List<ResumenNominaDTO> ObtenerDetallesNominaParaCheques(int IdNomina) 
        {

            List<ResumenNominaDTO> listaResumenNomina = new List<ResumenNominaDTO>();


           DatosBitacoraParaCheque datosEncontrados =  FoliarConsultasDBSinEntity.ObtenerAnBitacoraParaCheques(IdNomina);

            //verifica que no haya ocurrido un problema al traer los datos
            if (datosEncontrados.Comentario != "Sin Datos")
            {
                //verifica que la nomina sea general o desentralizado ya que son las dos unicas que se imprimen en diferentes archivos (Confianza y sindicalizados C-G?2115.tx y S-G?2115.txt)
                if (datosEncontrados.Nomina == "01" || datosEncontrados.Nomina == "02")
                {
                    ConsultasSQLSindicatoGeneralYDesc nuevaConsulta  = new ConsultasSQLSindicatoGeneralYDesc(datosEncontrados.An);


                    List<string> listaConsultas = nuevaConsulta.ObtenerConsultasSindicato();

                    if (listaConsultas.Count() > 0) 
                    {
                        List<TotalRegistrosDelegacionXSindicatoDTO> resultadoTotalRegistros = FoliarConsultasDBSinEntity.ObtenerDetalleNominaConsultaGeneralYDesc(listaConsultas);

                        if (resultadoTotalRegistros.Count() > 0) 
                        {
                            foreach (TotalRegistrosDelegacionXSindicatoDTO registroSeleccionado in resultadoTotalRegistros) 
                            {
                                //registroSeleccionado.Delegacion


                                //string deleg = null;
                                string consulta = null;
                                bool foliado = true;

                                switch (registroSeleccionado.Delegacion)
                                {
                                    case "Campeche y Mas":

                                        if (registroSeleccionado.Sindicato )
                                        {
                                            //Sindicalizado 
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza degacion 00 e inlcuyen ('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16');
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Champoton":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion 03;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Escarcega - Candelaria":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicato
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion 04;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Calkini":

                                        if (registroSeleccionado.Sindicato) 
                                        {
                                            //sindizalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion  05;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Hecelchakan":

                                        if (registroSeleccionado.Sindicato)
                                        {
                                            //Sindicalizado
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        else
                                        {
                                            //Confianza delegacion  06;
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                                            foliado = FoliarConsultasDBSinEntity.ConsultaEstaFoliada(consulta);
                                        }
                                        break;

                                    case "Hopelchen":

                                        if (registroSeleccionado.Sindicato) 
                                        {
                                            //Sindicalizados
                                            consulta = "select NUM_CHE from interfaces.dbo." + datosEncontrados.An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2') , DELEG,  SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
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
                                    nuevoResumen.Sindicalizado = registroSeleccionado.Total;
                                } else 
                                {
                                    nuevoResumen.Confianza = registroSeleccionado.Total;
                                }
                                nuevoResumen.Otros = 0;

                                nuevoResumen.Foliado = foliado;
                                nuevoResumen.Total = registroSeleccionado.Total;


                                listaResumenNomina.Add(nuevoResumen);

                            }
                  
                        
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




        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosRevicionPorDelegacionFormasPago(string Consulta, string NumeroNomina, int NumeroChequeInicial, string NombreBanco) 
        {
            return FoliarConsultasDBSinEntity.ObtenerDatosNominaRevicionFormasDePago(Consulta, NumeroNomina, NumeroChequeInicial, NombreBanco);
        }




    }
}
