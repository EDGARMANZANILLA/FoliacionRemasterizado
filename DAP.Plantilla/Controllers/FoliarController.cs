﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Negocios;
using DAP.Plantilla.Reportes.Datasets;
using DAP.Plantilla.Models;
using DAP.Foliacion.Entidades;
using DAP.Plantilla.Models.FoliacionModels;
using System.Security.Principal;
using AutoMapper;

namespace DAP.Plantilla.Controllers
{
    public class FoliarController : Controller
    {
        //VISTAS
        public ActionResult Index()
        {
            ViewBag.UltimaQuincenaEncontrada = FoliarNegocios.ObtenerUltimaQuincenaFoliada();

            return View();
        }

        public ActionResult FoliarXPagomatico(string NumeroQuincena)
        {
            try
            {
              //  Session.Remove("NumeroQuincena");
              //  Session["NumeroQuincena"] = Convert.ToInt32(NumeroQuincena.Substring(1, 3));

                int anio = Convert.ToInt32( DateTime.Now.Year.ToString().Substring(0, 2) + NumeroQuincena.Substring(0, 2));

                ViewBag.NumeroQuincena = NumeroQuincena;
                Dictionary<int, string> ListaNombresQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena, anio );
                
                if (ListaNombresQuincena.Count() > 0)
                {
                    return PartialView(ListaNombresQuincena);
                }

            }
            catch (Exception E)
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    MensajeError = "Ocurrio un error verifique que la quincena sea correcta"
                });


            }

            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentran cargadas las nominas de la quincena seleccionada"
            });
        }

        public ActionResult FoliarXFormasPago(string Quincena)
        {
            try
            {

                //Session.Remove("NumeroQuincena");
                //Session["NumeroQuincena"] = Convert.ToInt32(Quincena.Substring(1, 3));

                int anio = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));

                ViewBag.NumeroQuincena = Quincena;
                Dictionary<int, string> ListaNombresNominaQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(Quincena, anio);
                ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancoParaFormasPago();

                if (ListaNombresNominaQuincena.Count() > 0)
                {
                    return PartialView(ListaNombresNominaQuincena);
                }
            }
            catch (Exception E)
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    MensajeError = "Ocurrio un error verifique que la quincena sea correcta"
                });


            }
       


            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentran cargadas las nominas de la quincena seleccionada"
            });
        }







        [HttpPost]
        //Actualiza la tabla de cuantos cheques por banco quedan disponibles cuando carga la vista de Foliar index 
        public ActionResult ActualizarTablaResumenBanco(string Dato) 
        {
            return Json(FoliarNegocios.ObtenerDetalleBancoFormasDePago(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerNombreNominas(string NumeroQuincena)
        {
            int anio = ObtenerAnioDeQuincena(NumeroQuincena);

            var contenedoresEncontrados = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena, anio);


            //Se crea la variable de session cada vez que inicia el proceso de foliar



            return Json(contenedoresEncontrados, JsonRequestBehavior.AllowGet);
        }







        /****************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************/
        /***********************************************************            Index         ***************************************************************************************************/
        /***************************************************************************************************************************************************************************************/
        /***************************************************************************************************************************************************************************************/
        //Aun en validacion si deberia imprimirse o no el documento - por el momento no se ocupa
        // esto generaba un Reporte para saber las nominas que se tenian que foliar con formas de pago (cheques) pero aun no se encontro una ayuda real asi que actualmente no se usa como tal
        public FileResult GenerarReporteParaFoliar(string Quincena)
        {

             var datosReporteObtenido = FoliarNegocios.ObtenerEmpleadosXNominaParaReporteFoliacion(Quincena);

      


            DAP.Plantilla.Reportes.Datasets.ReporteInicialNominasParaFoliacion dtsReporteInicialParaFoliacion = new  DAP.Plantilla.Reportes.Datasets.ReporteInicialNominasParaFoliacion();

            if (datosReporteObtenido.Count() > 0)
            {
                //Cargar el numero del memo
                dtsReporteInicialParaFoliacion.Quincena.AddQuincenaRow(Quincena);


                //cargar datos al dataset para el reporte
                foreach (var reporteDelegacion in datosReporteObtenido)
                {

                    dtsReporteInicialParaFoliacion.RegitrosNominaDelegacion.AddRegitrosNominaDelegacionRow(reporteDelegacion.Nomina, reporteDelegacion.Id_nom, reporteDelegacion.Coment, reporteDelegacion.Adicional, reporteDelegacion.RutaNomina, Convert.ToString( reporteDelegacion.Confianza), Convert.ToString( reporteDelegacion.Sindicalizado), Convert.ToString( reporteDelegacion.Otros), reporteDelegacion.Delegacion);
                }

            }


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/NominasParaFoliacion.rpt"));

            rd.SetDataSource(dtsReporteInicialParaFoliacion);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ReporteParaFoliacion"+"_"+Quincena+".pdf");
        }












        #region Metodos para la Revicion de la Foliacion por medio de Formas de pago
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //************************************  ****    METODOS PARA FOLIAR DELEGACION DE NOMINA PARA FORMAS DE PAGO (QUECHE)      **************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        public ActionResult FoliarNominaFormasDePago(DatosParaFoliarChequesModel NuevaFoliacionDatos)
        {
            


            return Json("archivoBase64", JsonRequestBehavior.AllowGet);
        }



        //*************************************************************************************************************************************************************************************************************************************************//
        //*************************************************************************************************************************************************************************************************************************************************//
        //****************************   OBTIENE UN RESUMEN POR DELEGACION DE UNA NOMINA ELEGIDA PARA VISUALIZAR ES UN MODAL CUANTOS REGISTROS HAY POR DELEGACION PARA FORLIAR POR CHEQUERA (CHEQUE)      **************************************************//
        //************************************************************************************************************************************************************************************************************************************************//
        //************************************************************************************************************************************************************************************************************************************************//
        public ActionResult ObtenerResumenxDelegacionNominaCheques(int IdNomina , string Quincena )
        {

            int anioInterface = ObtenerAnioDeQuincena(Quincena);
            //ObtenerDetalleNominaParaCheques
            // var resumenDatosTablaModal = FoliarNegocios.ObtenerDetallesNominaChequesParaModal(IdNomina).OrderBy(X => X.Delegacion);
            var resumenDatosTablaModal = FoliarNegocios.ObtenerResumenDelegacionesNominaCheques( IdNomina, anioInterface).OrderBy(X => X.IdDelegacion);

            //string NombreModal = FoliarNegocios.ObtenerNombreModalPorIDNomina(IdNomina);

         

            //return Json(new
            //{
            //    TablaModal = resumenDatosTablaModal
            //});

            return Json(resumenDatosTablaModal, JsonRequestBehavior.AllowGet);
        }

        //***************************************************************************************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************************************************************************************//
        //***********************        GENERA REPORTE EN PDF DE CHEQUES DONDE SE VISUALIZA CADA EMPLEADO COMO SE ENCUENTRA EN SQL PARA VERIFICAR SI ESTA BIEN FOLIADO O NO DE ACUERDO A LOS FOLIOS Y LA CHEQUERA QUE SE UTILIZO    *************************//
        //***************************************************************************************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************************************************************************************//
        public ActionResult RevisarReportePDFChequeIdNominaPorDelegacion(GenerarReportePorDelegacionChequeModels GenerarReporteDelegacion) 
        {
            int anioInterface = ObtenerAnioDeQuincena(GenerarReporteDelegacion.Quincena);


            var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(GenerarReporteDelegacion.IdNomina, anioInterface);

            List<ResumenRevicionNominaPDFModel> ResumenRevicionNominaPDF = new List<ResumenRevicionNominaPDFModel>();

            if (GenerarReporteDelegacion.GrupoFoliacion == 0)
            {
                //El grupo de foliacion {0} de los cheques pertenece a las nominas GENERAL Y DESCENTRALIZADOS

                bool EsSindicalizado;
                if (GenerarReporteDelegacion.Sindicato > 0 && GenerarReporteDelegacion.Confianza == 0)
                {
                    EsSindicalizado = true;
                    ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, EsSindicalizado, GenerarReporteDelegacion.IdDelegacion));
                              
                }
                else if (GenerarReporteDelegacion.Sindicato == 0 && GenerarReporteDelegacion.Confianza > 0) 
                {
                    EsSindicalizado = false;
                    ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, EsSindicalizado, GenerarReporteDelegacion.IdDelegacion));

                }


            }
            else 
            {
                ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionOtrasNominas_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, GenerarReporteDelegacion.IdDelegacion ));
            }



            string archivoBase64;
            //    using (new Foliacion.Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            //   {

            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow(datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina, " ");
            // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


            foreach (var resultado in ResumenRevicionNominaPDF)
            {
                //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(resultado.Contador, resultado.Partida, resultado.NombreEmpleado, resultado.Delegacion, resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta, resultado.CadenaNumEmpleado, resultado.Nomina);
            }

            string pathPdf = @"C:\Reporte\FoliacionRevicionPDF";

            if (!Directory.Exists(pathPdf))
            {
                Directory.CreateDirectory(pathPdf);
            }




            string pathCompleto = pathPdf + "\\" + "RevicionNomina"+datosCompletosNomina.Id_nom+".pdf";
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));
            rd.SetDataSource(dtsRevicionFolios);
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathCompleto);



            byte[] archivo = ObtenerBytes(pathCompleto);


            archivoBase64 = Convert.ToBase64String(archivo);



            if (System.IO.File.Exists(pathCompleto))
            {
                System.IO.File.Delete(pathCompleto);
            }

            //}



            return Json(archivoBase64, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RevisarReportePDFChequeIdNomina(int IdNomina , string Quincena)
        {
            int anioInterface = ObtenerAnioDeQuincena(Quincena);


            var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, anioInterface);

            List<ResumenRevicionNominaPDFModel> ResumenRevicionNominaPDF = new List<ResumenRevicionNominaPDFModel>();

            

            if (datosCompletosNomina.Nomina == "01" || datosCompletosNomina.Nomina == "02")
            {
                ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>( FoliarNegocios.ObtenerDatosPersonalesNominaGENEDESCE_ReporteCheque( IdNomina, anioInterface));
            }
            else 
            {
                ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(  FoliarNegocios.ObtenerDatosPersonalesOtrasNomina_ReporteCheque(IdNomina, anioInterface) );
            }





            string archivoBase64;
            //    using (new Foliacion.Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            //   {

            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow(datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina, " ");
            // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


            foreach (var resultado in ResumenRevicionNominaPDF)
            {
                //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(resultado.Contador, resultado.Partida, resultado.NombreEmpleado, resultado.Delegacion, resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta, resultado.CadenaNumEmpleado, resultado.Nomina);
            }

            string pathPdf = @"C:\Reporte\FoliacionRevicionPDF";

            if (!Directory.Exists(pathPdf))
            {
                Directory.CreateDirectory(pathPdf);
            }




            string pathCompleto = pathPdf + "\\" + "RevicionNomina" + datosCompletosNomina.Id_nom + ".pdf";
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));
            rd.SetDataSource(dtsRevicionFolios);
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathCompleto);



            byte[] archivo = ObtenerBytes(pathCompleto);


            archivoBase64 = Convert.ToBase64String(archivo);



            if (System.IO.File.Exists(pathCompleto))
            {
                System.IO.File.Delete(pathCompleto);
            }

            //}



            return Json(archivoBase64, JsonRequestBehavior.AllowGet);
        }


      



        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***********************************************************MEtodo para ver reporte de nomina cheque **********************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//


        //public ActionResult RevisarNominaFormaPago(RevicionFormasPagoModel NuevaRevicion)

        //{   //el grupo de nomina pertenece a los que se folean por el campo sindizato
        //    // 1 = le pertenece a las nominas general y descentralizada
        //    // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 



        //    string ultimoFolioUsar = "";
        //    int iteradorPersonasFoliadas = 0;

        //    string rutaAlmacenamiento = "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionNominaFormasDePago" + NuevaRevicion.IdNomina + ".pdf";


        //    //obtiene los detalles de una nomina en especifico filtrado por el Id_Nom de bitacora
        //    var detalleIdNomina = FoliarNegocios.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(NuevaRevicion.IdNomina);

        //    if (detalleIdNomina.Nomina == "01" && NuevaRevicion.Sindicato == false && NuevaRevicion.Confianza == false || detalleIdNomina.Nomina == "02" && NuevaRevicion.Sindicato == false && NuevaRevicion.Confianza == false)
        //    {
        //        //Verifica que si la nomina es General o Descentralizada el usuario haya escogido un tipo de foliacion por SINDICATO o CONFIANZA
        //        return Json(new
        //        {
        //            RespuestaServidor = 99,
        //            Error = "¿Desea foliar sindicalizados o de confianza?",
        //            Solucion = "Asegurese de seleccionar un item de sindicato o confianza en el modal Detalles de nomina"

        //        });
        //    }
        //    else
        //    {
        //        if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial == 0 && NuevaRevicion.RangoInhabilitadoFinal == 0 || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial != 0 && NuevaRevicion.RangoInhabilitadoFinal == 0 || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial == 0 && NuevaRevicion.RangoInhabilitadoFinal != 0)
        //        {
        //            //Verifica que ambos campos de inhabilitados esten llenos  
        //            return Json(new
        //            {
        //                RespuestaServidor = 99,
        //                Error = "Rangos de Inhabilitados no llenados correctamente",
        //                Solucion = "Asegurese de que los rangos Inabilitados esten llenados ( Tanto el inicial como final)"

        //            });
        //        }
        //        else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial > NuevaRevicion.RangoInhabilitadoFinal)
        //        {
        //            //verififica que el rango inhabilitado inicial no sea mayor que el final
        //            return Json(new
        //            {
        //                RespuestaServidor = 99,
        //                Error = "'Revise sus numeros de folios de inhabilitacion'",
        //                Solucion = "El folio inicial no puede ser mas grande que el final"

        //            });


        //        }
        //        else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoInicial || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoFinal)
        //        {
        //            return Json(new
        //            {
        //                RespuestaServidor = 99,
        //                Error = "El rango inicial de folicion es menor que algun rango inhabilitado ",
        //                Solucion = " 'Revise sus numeros de folios inhabilitados' "
        //            });
        //        }
        //        else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial >= NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial >= NuevaRevicion.RangoInhabilitadoFinal /*|| NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoFinal  */)
        //        {
        //            //NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoFinal  &&  NuevaRevicion.RangoInhabilitadoInicial <= NuevaRevicion.RangoInhabilitadoFinal


        //            //para que este bien los folios de inhabilitacion el (( RANGOINICIAL < RANGOINHABILITADOINICIAL YY RANGOINHABILITADOFINAL > RANGOINHABILITADOINICIAL))
        //            return Json(new
        //            {
        //                RespuestaServidor = 99,
        //                Error = "El folio inicial no concuerdan con los folios a inhabilitar ",
        //                Solucion = " 'Revise sus numeros de folios inhabilitados' "
        //            });

        //        }
        //        else
        //        {


        //            string NombreBanco = FoliarNegocios.ObtenerBancoPorID(NuevaRevicion.IdBancoPagador);

        //            //Grupo 1
        //            if (detalleIdNomina.Nomina == "01" || detalleIdNomina.Nomina == "02")
        //            {

        //                //Obtener la consulta a la que corresponde la delegacion 
        //               // ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(detalleIdNomina.An);
        //                //string consulta = nuevaConsulta.ObtenerConsultaSindicatoFormasDePago(NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);
        //                string consulta = ConsultasSQLSindicatoGeneralYDesc.ObtenerConsultaSindicatoFormasDePago(detalleIdNomina.An, NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);


        //                //obtiene los datos como quedarian posiblemente al momento de folear 
        //                List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consulta, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));

        //                ///
        //                //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
        //                var chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaRevicion.IdBancoPagador, NuevaRevicion.RangoInicial, datosRevicionObtenidos.Count(), NuevaRevicion.Inhabilitado, NuevaRevicion.RangoInhabilitadoInicial, NuevaRevicion.RangoInhabilitadoFinal);

        //                var foliosNoDisponibles =  chequesVerificadosFoliar.Where( y => y.Incidencia != "").ToList();


        //                //Si todos los folios no tienen incidencias el proceso continua su rumbo 
        //                if (foliosNoDisponibles.Count == 0)
        //                {

        //                    //Crear reporte 
        //                    DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

        //                    //Pasa el nombre de la ruta
        //                    dtsRevicionFolios.Ruta.AddRutaRow("RUTA " + FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), " LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper());


        //                    //cargar datos al reporte 
        //                    foreach (var dato in datosRevicionObtenidos)
        //                    {
        //                        iteradorPersonasFoliadas++;
        //                        ultimoFolioUsar = dato.Num_Che;
        //                        dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
        //                    }


        //                    // Materializa el reporte en un pdf que pone en una carpeta 
        //                    ReportDocument rd = new ReportDocument();
        //                    rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

        //                    rd.SetDataSource(dtsRevicionFolios);

        //                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);



        //                }
        //                else 
        //                {
        //                    //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
        //                    return Json(new
        //                    {
        //                        RespuestaServidor = 98,
        //                        FoliosConIncidencias = foliosNoDisponibles
        //                    }) ;

        //                }

        //            }
        //            else /*if (NuevaRevicion.GrupoNomina == 2)*/
        //            {
        //                //Grupo2
        //                //Funciona para cualquier otra nomina que no se folea por sindicato y confianza 

        //                string consultaOtrasNominas = "";

        //                //verifica que si la nomina a verificar es pension "08" entonces selecciona una consulta deacuerdo a la nomina seleccionada
        //                if (detalleIdNomina.Nomina != "08")
        //                {
        //                    ConsultasSQLOtrasNominasConCheques NuevaConsulta = new ConsultasSQLOtrasNominasConCheques();
        //                    consultaOtrasNominas = NuevaConsulta.ObtenerConsultaConOrdenamientoFormasDePago(NuevaRevicion.Delegacion, detalleIdNomina.An);
        //                }
        //                else
        //                {
        //                    ConsultasSQLOtrasNominasConCheques NuevaConsultaPension = new ConsultasSQLOtrasNominasConCheques();
        //                    consultaOtrasNominas = NuevaConsultaPension.ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticia(NuevaRevicion.Delegacion, detalleIdNomina.An);
        //                }

        //                //Si no esta vacia procede obtener los datos y y rellena el pdf  
        //                if (!string.IsNullOrWhiteSpace(consultaOtrasNominas))
        //                {
        //                    List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consultaOtrasNominas, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));

        //                    //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
        //                    var chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaRevicion.IdBancoPagador, NuevaRevicion.RangoInicial, datosRevicionObtenidos.Count(), NuevaRevicion.Inhabilitado, NuevaRevicion.RangoInhabilitadoInicial, NuevaRevicion.RangoInhabilitadoFinal);

        //                    var foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();




        //                    //Si todos los folios no tienen incidencias el proceso continua su rumbo 
        //                    if (foliosNoDisponibles.Count == 0 && chequesVerificadosFoliar.Count > 0)
        //                    {

        //                        //Crear reporte 
        //                        DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

        //                        //Pasa el nombre de la ruta que es parte del encabezado del reporte
        //                        dtsRevicionFolios.Ruta.AddRutaRow("RUTA" + FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), "LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper());

        //                        //cargar datos al reporte 
        //                        foreach (var dato in datosRevicionObtenidos)
        //                        {
        //                            iteradorPersonasFoliadas++;
        //                            ultimoFolioUsar = dato.Num_Che;
        //                            dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
        //                        }


        //                        // Materializa el reporte en un pdf que pone en una carpeta 
        //                        ReportDocument rd = new ReportDocument();
        //                        rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

        //                        rd.SetDataSource(dtsRevicionFolios);

        //                        rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);
        //                    }
        //                    else if (chequesVerificadosFoliar.Count == 0)
        //                    {
        //                        //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
        //                        return Json(new
        //                        {
        //                            RespuestaServidor = 97,
        //                            Error = "No existe el folio inicial ingresado para la foliacion"
        //                        });
        //                    }
        //                    else 
        //                    {
        //                        //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
        //                        return Json(new
        //                        {
        //                            RespuestaServidor = 98,
        //                            FoliosConIncidencias = foliosNoDisponibles
        //                        });
        //                    }
        //                }


        //            }

        //        }


        //    }












        //    if (System.IO.File.Exists(rutaAlmacenamiento) && ultimoFolioUsar != "")
        //    {

        //        return Json(new
        //        {
        //            RespuestaServidor = 201,
        //            Delegacion = "VISTA PREVIA DE LA DELGACION : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper(),
        //            UltimoFolioUsado = ultimoFolioUsar,
        //            FoliosTotal = iteradorPersonasFoliadas,
        //            DatosExtras = FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.FormasDisponiblesInventario }).ToList()
        //        });

        //        // respuestaServer = "201";
        //    }
        //    else
        //    {
        //        return Json(new
        //        {
        //            RespuestaServidor = 500,
        //            Delegacion = "ERROR AL CARGAR LA DELGACION : " + (FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper()),
        //            UltimoFolioUsado = "Error no se puede simular la Foliacion",
        //            FoliosTotal = 0,
        //            Error = "No coincide la delegacion con el sindicato"
        //        });
        //    }

        //  //  return Json("404", JsonRequestBehavior.AllowGet);
        //}

        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//


        #endregion











        ////Guardar NuevaQuincena en la Tbl_historicoQuincenasRegistradas
        //public ActionResult RegistrarNuevaQuincena(int NuevaQuincesa) 
        //{
        //    return Json(NuevaQuincesa, JsonRequestBehavior.AllowGet);
        //}




        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //************************************      Metodos para FOLIAR nominas con PAGOMATICOS por nomina o todas las nominas      *************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//

        public async System.Threading.Tasks.Task<ActionResult> FoliarPorIdNominaPagomatico (int IdNomina, string NumeroQuincena) 
        {
            List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.FolearPagomaticoPorNomina(IdNomina, ObtenerAnioDeQuincena(NumeroQuincena), NumeroQuincena);

            var DBFAbierta = errores.Where(x => x.IdAtencion == 4).Select( x => new { x.Id_Nom, x.Detalle, x.Solucion } ).ToList();

            if (DBFAbierta.Count() > 0)
            {
                return Json(new
                {
                    bandera = false,
                    DBFAbierta = DBFAbierta
                });
            }
            else 
            {
                return Json(new
                {
                    bandera = true,
                    resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                });
            }
        }

        //Aun no esta en uso
        public async System.Threading.Tasks.Task<ActionResult> FoliarTodasNominas(string NumeroQuincena)
        {
            DateTime now = DateTime.Now;
            string Observa = "TARJETA";
            List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada ==  0).ToList();

            //Poner en segundo plano esto ya que se folean todas las nominas que contienen pagomatico
            List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.FolearPagomaticoTodasLasNominas( detallesTodasNominas);
            // List<AlertasAlFolearPagomaticosDTO> errores =  FoliarNegocios.FolearPagomaticoTodasLasNominas(NumeroQuincena, Observa).OrderBy(x => x.Id_Nom).ToList();

            DateTime fin = DateTime.Now;

            TimeSpan tarda = fin - now; 
            return Json( errores, JsonRequestBehavior.AllowGet);
        }


        //******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
        //************************************          VERFIFICAR LAS NOMINAS SI YA ESTAN FOLIADAS O NO  (VERIFICA en SQL)    ****************************//
        //******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
        /*******  Pinta tabla con el detalle de una nomina para saber si esta foliada y su detalle para pagomaticos  ******/
        public ActionResult EstaFoliadaIdNominaPagomatico(int IdNom, string NumeroQuincena) 
        {
            List<AlertaDeNominasFoliadasPagomatico> resultadoAlertas = FoliarNegocios.EstaFoliadaNominaSeleccionadaPagoMatico(IdNom, ObtenerAnioDeQuincena(NumeroQuincena) ).ToList();

           // return Json(resultadoAlertas, JsonRequestBehavior.AllowGet);
            if (resultadoAlertas.Count() > 0)
            {
                return Json(new
                {
                    RespuestaServidor = "201",
                    DetalleTabla = resultadoAlertas.OrderBy(x => x.Id_Nom)
                }) ;

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    Error = "ERROR AL CARGAR LOS DETALLES DE TODAS LAS NOMINAS, 'INTENTE DE NUEVO' "
                });
                //respuestaServer = "500";

            }
        }

        /*****************************************************************************************************************************************************************************/
                                 /****** pinta una tabla con el detalle de todas las nominas para saber si estan foliadas y sus detalles ******/
        public ActionResult EstanFoliadasTodasNominaPagomatico(string NumeroQuincena)
        {
            List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada != 2 ).ToList();


            if (detallesTodasNominas.Count() > 0)
            {
                return Json(new
                {
                    RespuestaServidor = "201",
                    DetalleTabla = detallesTodasNominas
                });

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    Error = "ERROR AL CARGAR LOS DETALLES DE TODAS LAS NOMINAS, 'INTENTE DE NUEVO' " 
                });
                //respuestaServer = "500";

            }

        }


        //*********************************************************************************************************************************************************************************************************//
        //*********************************************************************************************************************************************************************************************************//
        //************************************          GENERA REPORTE EN PDF DONDE SE VISUALIZA EL CADA EMPLEADO COMO SE ENCUENTRA EN SQL PARA VERIFICAR SI ESTA BIEN FOLIADO O NO    ****************************//
        //********************************************************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************************************************//

        public ActionResult RevisarReportePDFPagomaticoPorIdNomina(int IdNomina, string Quincena)
        {
            string archivoBase64 = "no entre";
           
                    int anio = ObtenerAnioDeQuincena(Quincena);

                    var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, anio);

                    var resumenRevicionNominaReportePDF = FoliarNegocios.ObtenerDatosPersonalesNominaReportePagomatico(datosCompletosNomina.An, anio, datosCompletosNomina.Nomina);



                    DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                    dtsRevicionFolios.Ruta.AddRutaRow(datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina, " ");
                    // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


                    foreach (var resultado in resumenRevicionNominaReportePDF)
                    {
                        //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                        dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(resultado.Contador, resultado.Partida, resultado.NombreEmpleado, resultado.Delegacion, resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta, resultado.CadenaNumEmpleado, resultado.Nomina);
                    }

                    string pathPdf = "C:\\Reporte\\FoliacionRevicionPDF";

                    if (!Directory.Exists(pathPdf))
                    {
                        Directory.CreateDirectory(pathPdf);
                    }


                   // string a = Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt");
                    

                    string pathCompleto = pathPdf + "\\" + "RevicionNomina" + IdNomina + ".pdf";

                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reportes/Crystal"), "RevicionFoliacionNomina.rpt"));
                    rd.SetDataSource(dtsRevicionFolios);
                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Reporte\\FoliacionRevicionPDF\\"+"RevicionNomina"+IdNomina+".pdf");



                   byte[] archivo = ObtenerBytes("C:\\Reporte\\FoliacionRevicionPDF\\" + "RevicionNomina"+IdNomina+".pdf");


                   //// if (archivo != null) 
                   archivoBase64 = Convert.ToBase64String(archivo);



                    if (System.IO.File.Exists(pathCompleto))
                    {
                        System.IO.File.Delete(pathCompleto);
                    }
                    rd.Close();
                    rd.Dispose();
                    dtsRevicionFolios.Dispose();
     

        
            return Json(archivoBase64, JsonRequestBehavior.AllowGet);
        }

        public byte[] ObtenerBytes(string path) => System.IO.File.ReadAllBytes(path);








        public int ObtenerAnioDeQuincena(string Quincena)
        {
            return Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));
        }











        //*********************************************************************************************************************************************************************************************************//
        //*********************************************************************************************************************************************************************************************************//
        //****************************************************         Metodos para FOLIAR nominas con cheques(Formas de pagos)    ********************************************************************************//
        //********************************************************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************************************************//

        public async System.Threading.Tasks.Task<ActionResult> FoliarNominaFormaPago(DatosAFoliarNominaConChequeraModel NuevaFoliacionNomina)
        {
            int totalDeRegistrosAFoliar = 0;
            if (NuevaFoliacionNomina.IdGrupoFoliacion == 0)
            {
                if (NuevaFoliacionNomina.Confianza > 0 && NuevaFoliacionNomina.Sindicato == 0)
                {
                    //son de confianza
                    totalDeRegistrosAFoliar = NuevaFoliacionNomina.Confianza;
                }
                else if (NuevaFoliacionNomina.Confianza == 0 && NuevaFoliacionNomina.Sindicato > 0)
                {
                    //Son sindicalizados
                    totalDeRegistrosAFoliar = NuevaFoliacionNomina.Sindicato;
                }
            } else if (NuevaFoliacionNomina.IdGrupoFoliacion == 1) 
            {
                totalDeRegistrosAFoliar = NuevaFoliacionNomina.Otros;
            }


            List<FoliosAFoliarInventario> chequesVerificadosFoliar = FoliarNegocios.verificarDisponibilidadFoliosEnInventarioDetalle(NuevaFoliacionNomina.IdBancoPagador, NuevaFoliacionNomina.RangoInicial, totalDeRegistrosAFoliar, NuevaFoliacionNomina.Inhabilitado, NuevaFoliacionNomina.RangoInhabilitadoInicial, NuevaFoliacionNomina.RangoInhabilitadoFinal).ToList();

            List<FoliosAFoliarInventario> foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();

            if (foliosNoDisponibles.Count() > 0)
            {
                return Json(new
                {
                    resultServer = 0, 
                    FoliosConIncidencias = foliosNoDisponibles
                });

            }
            else 
            {

                FoliarFormasPagoDTO foliarNomina = new FoliarFormasPagoDTO();

                foliarNomina.IdNomina = NuevaFoliacionNomina.IdNomina;
                foliarNomina.IdDelegacion = NuevaFoliacionNomina.IdDelegacion;
                foliarNomina.Sindicato = NuevaFoliacionNomina.Sindicato;
                foliarNomina.Confianza = NuevaFoliacionNomina.Confianza;
                foliarNomina.IdBancoPagador = NuevaFoliacionNomina.IdBancoPagador;
                foliarNomina.RangoInicial = NuevaFoliacionNomina.RangoInicial;

                // por si el usuario habilita la casilla inhabilitados aqui se rescatan
                foliarNomina.Inhabilitado = NuevaFoliacionNomina.Inhabilitado;
                foliarNomina.RangoInhabilitadoInicial = NuevaFoliacionNomina.RangoInhabilitadoInicial;
                foliarNomina.RangoInhabilitadoFinal = NuevaFoliacionNomina.RangoInhabilitadoFinal;


                //propiedad usada para saber a que grupo de nomina corresponde
                // 1 = le pertenece a las nominas general y descentralizada
                // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza
                foliarNomina.IdGrupoFoliacion = NuevaFoliacionNomina.IdGrupoFoliacion;
                foliarNomina.AnioInterfaz = ObtenerAnioDeQuincena(NuevaFoliacionNomina.Quincena);

                string Observa = "CHEQUE";
                List<AlertasAlFolearPagomaticosDTO> resultadoFoliacion = await FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa, chequesVerificadosFoliar);

                var DbfAbierta = resultadoFoliacion.Where(x => x.IdAtencion == 4).Select(x => new { x.Id_Nom, x.Detalle, x.Solucion }).ToList();


                if (DbfAbierta.Count() > 0) 
                {
                        return Json(new
                        {
                            resultServer = 5,
                            DbfAbierta = DbfAbierta
                        });
                }


                return Json(new
                {
                    resultServer = 1,
                    resultadoFoliacion = resultadoFoliacion
                });

            }






        }
    }


}