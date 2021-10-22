using System;
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

namespace DAP.Plantilla.Controllers
{
    public class FoliarController : Controller
    {
        // GET: Foliar
        public ActionResult Index()
        {
            //  ObtenerNombreNominas("2112");
            //var detallesBancos = FoliarNegocios.ObtenerDetalleBancoFormasDePago();

            /*
            var detallesBancoFiltrado = FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles }).ToList();

            List<DetallesBancoInventario> nuevaLista = new List<DetallesBancoInventario>();

            foreach (var detalle in detallesBancoFiltrado) 
            {
                DetallesBancoInventario nuevoDetalle = new DetallesBancoInventario();
                nuevoDetalle.NombreBanco = detalle.NombreBanco;
                nuevoDetalle.Cuenta = detalle.Cuenta;
                nuevoDetalle.FormasDisponibles = detalle.FormasDisponibles;

                nuevaLista.Add(nuevoDetalle);
            }

            ViewBag.DetallesBanco = nuevaLista;
            */


            ViewBag.UltimaQuincenaEncontrada = FoliarNegocios.ObtenerUltimaQuincenaFoliada();

            return View();
        }

        public ActionResult FoliarXPagomatico(string NumeroQuincena)
        {
            try
            {
                Session.Remove("NumeroQuincena");
                Session["NumeroQuincena"] = Convert.ToInt32(NumeroQuincena.Substring(1, 3));
                ViewBag.NumeroQuincena = NumeroQuincena;
                Dictionary<int, string> ListaNombresQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);
                // ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancosParaPagomatico(); 

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



        public ActionResult FoliarXFormasPago(string NumeroQuincena)
        {
            try
            {

                Session.Remove("NumeroQuincena");
                Session["NumeroQuincena"] = Convert.ToInt32(NumeroQuincena.Substring(1, 3));
                ViewBag.NumeroQuincena = NumeroQuincena;
                Dictionary<int, string> ListaNombresNominaQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);
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
            return Json(FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles, y.Tbl_Inventario.UltimoFolioUtilizado }).ToList() , JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerNombreNominas(string NumeroQuincena)
        {

            var contenedoresEncontrados = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);


            //Se crea la variable de session cada vez que inicia el proceso de foliar



            return Json(contenedoresEncontrados, JsonRequestBehavior.AllowGet);
        }







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





        //******************************************************************************************************************************************************************//
        //******************************************************************************************************************************************************************//
        // Metodos de revision para folear nominas por formas de PAGOMATICO  por nomina o todas las nominas//
        public ActionResult RevisarTodasNominas(string NumeroQuincena)
        {
            //Seleccionar una lista con las nominas disponibles de la quincena
            var nominasObtenidadRevicion = FoliarNegocios.ObtenerTodasNominasXQuincena(NumeroQuincena);

            //Se crea una lista para contener todas las nominas
            List<List<DatosReporteRevisionNominaDTO>> datosNominaObtenidos = new List<List<DatosReporteRevisionNominaDTO>>();

            //string nombreBanco = null;
            foreach (var nuevaNomina in nominasObtenidadRevicion)
            {

                //verifica que en la posicion 1 de ap y 2 de ad vengan vacios ya que si estan vacios quiere decir que es una nomina de pencion alimenticia
                if (nuevaNomina.Ap != "" && nuevaNomina.Ad != "")
                {
                    //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                    // nombreBanco = FoliarNegocios.ObtenerBancoPorID(2);
                    datosNominaObtenidos.Add(FoliarNegocios.ObtenerDatosFoliadosPorNominaRevicion(nuevaNomina.Nomina, nuevaNomina.An, Convert.ToInt32(NumeroQuincena.Substring(1, 3))));
                }
                else
                {
                    //entra a este apartado al saber que ap = "" y ad = "" lo que quiere decir que se trata de una nomina de pension alimenticia
                    //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                    //nombreBanco = FoliarNegocios.ObtenerBancoPorID(1);
                    datosNominaObtenidos.Add(FoliarNegocios.ObtenerDatosFoliadosPorNominaPENALRevicion(nuevaNomina.Nomina, nuevaNomina.An, Convert.ToInt32(NumeroQuincena.Substring(1, 3))));
                }

            }



            //GENERACION DEL PDF GENERAL PARA QUE PUEDA VISUALIZAR EL USUARIO EN UN MODAL 
            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionGeneralFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();


            int i = 0;

            foreach (var NominaObtenida in datosNominaObtenidos) {

                foreach (var recorrerNomina in NominaObtenida)
                {
                    i++;

                    dtsRevicionGeneralFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(i), recorrerNomina.Partida, recorrerNomina.Nombre, recorrerNomina.Deleg, recorrerNomina.Num_Che, recorrerNomina.Liquido, recorrerNomina.CuentaBancaria, recorrerNomina.Num, recorrerNomina.Nom);

                }
            }


            dtsRevicionGeneralFolios.Ruta.AddRutaRow("Contiene " + i + " registros de dispercion de todas las nominas de la quincena No. " + NumeroQuincena, " REVICION DE TODAS LAS NOMINAS ");



            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

            rd.SetDataSource(dtsRevicionGeneralFolios);


            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionGeneralNominas" + NumeroQuincena + ".pdf");

            return Json("TODO LISTO", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RevisarPorIdNomina(int IdNomina)
        {
            //Buscar el campo an de la bitacora al que pertenece  el IdNomina
            //Crear un metodo que reciba el Idnomina en el negocio que obtenga los datos que los actualize en la lista(no en SQL)
            //regresarlo al controlador y pasarlo a un pdf en cristal report para mostrarselo al usuario 

            //si contiene mas de un elemento se le puede aplicar la llave ya que si contiene el compo ESP_NOM lanomina
            //si solo contiene un valor y los otros estan vacios esa nomina corresponde a una Pencion Alimenticia
            List<string> anApAdEncontrados = FoliarNegocios.ObtenerAnApAdNominaBitacoraPorIdNumConexion(IdNomina);

            string an = anApAdEncontrados[0];
            string ap = anApAdEncontrados[1];
            string ad = anApAdEncontrados[2];


            //se obtiene el numero de a quincena por session ya que esta dentro de otro metodo de este controlador pero aqui se restada ese dato
            int quincena = (int)Session["NumeroQuincena"];


            //se solicita el numero de la nomina para pasarsela al cristal ya que este campo ayuda a saber a que nomina pertenece
            string numeroNomina = FoliarNegocios.ObtenerNumeroNominaXIdNumBitacora(IdNomina);

            // string nombreBanco = null;

            //se crea un contenedor para guardar los datos y enviarselos a cristal y el cliente u usuario vea como quedara la nomina al terminar de foliarla
            List<DatosReporteRevisionNominaDTO> datosNominaObtenidos = new List<DatosReporteRevisionNominaDTO>();

            //verifica que en la posicion 1 de ap y 2 de ad vengan vacios ya que si estan vacios quiere decir que es una nomina de pencion alimenticia
            if (anApAdEncontrados[1] != "" && anApAdEncontrados[2] != "")
            {
                //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                //nombreBanco = FoliarNegocios.ObtenerBancoPorID(2);
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaRevicion(numeroNomina, anApAdEncontrados[0], quincena);
            }
            else
            {
                //entra a este apartado al saber que ap = "" y ad = "" lo que quiere decir que se trata de una nomina de pension alimenticia
                //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                // nombreBanco = FoliarNegocios.ObtenerBancoPorID(1);
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaPENALRevicion(numeroNomina, anApAdEncontrados[0], quincena);
            }


            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


            foreach (var resultado in datosNominaObtenidos)
            {
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
            }


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

            rd.SetDataSource(dtsRevicionFolios);


            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionNomina" + IdNomina + ".pdf");

            return Json("TODO LISTO", JsonRequestBehavior.AllowGet);
        }





         //*****************************************************************************************************************************************************************//
        //*****************************************************************************************************************************************************************//
                                            // METODOS DE REVISION para folear nominas con cheques (Formas de pagos) //
        #region Metodos para la Revicion de la Foliacion por medio de Formas de pago
        public ActionResult ObtenerDetalleNominaPorIdNominaParaModal(int IdNomina)
        {
            //ObtenerDetalleNominaParaCheques

            var resumenDatosTablaModal = FoliarNegocios.ObtenerDetallesNominaChequesParaModal(IdNomina).OrderBy(X => X.Delegacion);

            string NombreModal = FoliarNegocios.ObtenerNombreModalPorIDNomina(IdNomina);

            var NumeroNomina = FoliarNegocios.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(IdNomina);

            bool bandera= false;

            if (NumeroNomina.Nomina.Equals("01") || NumeroNomina.Nomina.Equals("02"))
            {
                bandera = true;
            } 

            return Json(new
            {
                TablaModal = resumenDatosTablaModal,
                NombreDetalladoNomina = NombreModal,
                NominaEsGenODesc = bandera

            });

           // return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RevisarNominaFormaPago(RevicionFormasPagoModel NuevaRevicion)
       
        {   //el grupo de nomina pertenece a los que se folean por el campo sindizato
            // 1 = le pertenece a las nominas general y descentralizada
            // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 


           
            string ultimoFolioUsar = "";
            int iteradorPersonasFoliadas = 0;

            string rutaAlmacenamiento = "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionNominaFormasDePago" + NuevaRevicion.IdNomina + ".pdf";


            //obtiene los detalles de una nomina en especifico filtrado por el Id_Nom de bitacora
            var detalleIdNomina = FoliarNegocios.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(NuevaRevicion.IdNomina);

            if (detalleIdNomina.Nomina == "01" && NuevaRevicion.Sindicato == false && NuevaRevicion.Confianza == false || detalleIdNomina.Nomina == "02" && NuevaRevicion.Sindicato == false && NuevaRevicion.Confianza == false)
            {
                //Verifica que si la nomina es General o Descentralizada el usuario haya escogido un tipo de foliacion por SINDICATO o CONFIANZA
                return Json(new
                {
                    RespuestaServidor = 99,
                    Error = "¿Desea foliar sindicalizados o de confianza?",
                    Solucion = "Asegurese de seleccionar un item de sindicato o confianza en el modal Detalles de nomina"

                });
            }
            else
            {
                if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial == 0 && NuevaRevicion.RangoInhabilitadoFinal == 0 || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial != 0 && NuevaRevicion.RangoInhabilitadoFinal == 0 || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial == 0 && NuevaRevicion.RangoInhabilitadoFinal != 0)
                {
                    //Verifica que ambos campos de inhabilitados esten llenos  
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "Rangos de Inhabilitados no llenados correctamente",
                        Solucion = "Asegurese de que los rangos Inabilitados esten llenados ( Tanto el inicial como final)"

                    });
                }
                else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInhabilitadoInicial > NuevaRevicion.RangoInhabilitadoFinal)
                {
                    //verififica que el rango inhabilitado inicial no sea mayor que el final
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "'Revise sus numeros de folios de inhabilitacion'",
                        Solucion = "El folio inicial no puede ser mas grande que el final"

                    });


                }
                else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoInicial || NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoFinal)
                {
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "El rango inicial de folicion es menor que algun rango inhabilitado ",
                        Solucion = " 'Revise sus numeros de folios inhabilitados' "
                    });
                }
                else if (NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial >= NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial >= NuevaRevicion.RangoInhabilitadoFinal /*|| NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial > NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoFinal  */)
                {
                    //NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoFinal  &&  NuevaRevicion.RangoInhabilitadoInicial <= NuevaRevicion.RangoInhabilitadoFinal


                    //para que este bien los folios de inhabilitacion el (( RANGOINICIAL < RANGOINHABILITADOINICIAL YY RANGOINHABILITADOFINAL > RANGOINHABILITADOINICIAL))
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "El folio inicial no concuerdan con los folios a inhabilitar ",
                        Solucion = " 'Revise sus numeros de folios inhabilitados' "
                    });

                }
                else
                {


                    string NombreBanco = FoliarNegocios.ObtenerBancoPorID(NuevaRevicion.IdBancoPagador);

                    //Grupo 1
                    if (detalleIdNomina.Nomina == "01" || detalleIdNomina.Nomina == "02")
                    {

                        //Obtener la consulta a la que corresponde la delegacion 
                       // ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(detalleIdNomina.An);
                        //string consulta = nuevaConsulta.ObtenerConsultaSindicatoFormasDePago(NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);
                        string consulta = ConsultasSQLSindicatoGeneralYDesc.ObtenerConsultaSindicatoFormasDePago(detalleIdNomina.An, NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);


                        //obtiene los datos como quedarian posiblemente al momento de folear 
                        List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consulta, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));

                        ///
                        //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
                        var chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaRevicion.IdBancoPagador, NuevaRevicion.RangoInicial, datosRevicionObtenidos.Count(), NuevaRevicion.Inhabilitado, NuevaRevicion.RangoInhabilitadoInicial, NuevaRevicion.RangoInhabilitadoFinal);

                        var foliosNoDisponibles =  chequesVerificadosFoliar.Where( y => y.Incidencia != "").ToList();


                        //Si todos los folios no tienen incidencias el proceso continua su rumbo 
                        if (foliosNoDisponibles.Count == 0)
                        {

                            //Crear reporte 
                            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                            //Pasa el nombre de la ruta
                            dtsRevicionFolios.Ruta.AddRutaRow("RUTA " + FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), " LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper());


                            //cargar datos al reporte 
                            foreach (var dato in datosRevicionObtenidos)
                            {
                                iteradorPersonasFoliadas++;
                                ultimoFolioUsar = dato.Num_Che;
                                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
                            }


                            // Materializa el reporte en un pdf que pone en una carpeta 
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

                            rd.SetDataSource(dtsRevicionFolios);

                            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);



                        }
                        else 
                        {
                            //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                            return Json(new
                            {
                                RespuestaServidor = 98,
                                FoliosConIncidencias = foliosNoDisponibles
                            }) ;

                        }

                    }
                    else /*if (NuevaRevicion.GrupoNomina == 2)*/
                    {
                        //Grupo2
                        //Funciona para cualquier otra nomina que no se folea por sindicato y confianza 

                        string consultaOtrasNominas = "";

                        //verifica que si la nomina a verificar es pension "08" entonces selecciona una consulta deacuerdo a la nomina seleccionada
                        if (detalleIdNomina.Nomina != "08")
                        {
                            ConsultasSQLOtrasNominasConCheques NuevaConsulta = new ConsultasSQLOtrasNominasConCheques();
                            consultaOtrasNominas = NuevaConsulta.ObtenerConsultaConOrdenamientoFormasDePago(NuevaRevicion.Delegacion, detalleIdNomina.An);
                        }
                        else
                        {
                            ConsultasSQLOtrasNominasConCheques NuevaConsultaPension = new ConsultasSQLOtrasNominasConCheques();
                            consultaOtrasNominas = NuevaConsultaPension.ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticia(NuevaRevicion.Delegacion, detalleIdNomina.An);
                        }

                        //Si no esta vacia procede obtener los datos y y rellena el pdf  
                        if (!string.IsNullOrWhiteSpace(consultaOtrasNominas))
                        {
                            List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consultaOtrasNominas, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));

                            //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
                            var chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaRevicion.IdBancoPagador, NuevaRevicion.RangoInicial, datosRevicionObtenidos.Count(), NuevaRevicion.Inhabilitado, NuevaRevicion.RangoInhabilitadoInicial, NuevaRevicion.RangoInhabilitadoFinal);

                            var foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();




                            //Si todos los folios no tienen incidencias el proceso continua su rumbo 
                            if (foliosNoDisponibles.Count == 0 && chequesVerificadosFoliar.Count > 0)
                            {

                                //Crear reporte 
                                DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                                //Pasa el nombre de la ruta que es parte del encabezado del reporte
                                dtsRevicionFolios.Ruta.AddRutaRow("RUTA" + FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), "LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper());

                                //cargar datos al reporte 
                                foreach (var dato in datosRevicionObtenidos)
                                {
                                    iteradorPersonasFoliadas++;
                                    ultimoFolioUsar = dato.Num_Che;
                                    dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
                                }


                                // Materializa el reporte en un pdf que pone en una carpeta 
                                ReportDocument rd = new ReportDocument();
                                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

                                rd.SetDataSource(dtsRevicionFolios);

                                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);
                            }
                            else if (chequesVerificadosFoliar.Count == 0)
                            {
                                //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                                return Json(new
                                {
                                    RespuestaServidor = 97,
                                    Error = "No existe el folio inicial ingresado para la foliacion"
                                });
                            }
                            else 
                            {
                                //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                                return Json(new
                                {
                                    RespuestaServidor = 98,
                                    FoliosConIncidencias = foliosNoDisponibles
                                });
                            }
                        }


                    }

                }


            }












            if (System.IO.File.Exists(rutaAlmacenamiento) && ultimoFolioUsar != "")
            {

                return Json(new
                {
                    RespuestaServidor = 201,
                    Delegacion = "VISTA PREVIA DE LA DELGACION : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper(),
                    UltimoFolioUsado = ultimoFolioUsar,
                    FoliosTotal = iteradorPersonasFoliadas,
                    DatosExtras = FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles }).ToList()
                });

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = 500,
                    Delegacion = "ERROR AL CARGAR LA DELGACION : " + (FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper()),
                    UltimoFolioUsado = "Error no se puede simular la Foliacion",
                    FoliosTotal = 0,
                    Error = "No coincide la delegacion con el sindicato"
                });
            }

          //  return Json("404", JsonRequestBehavior.AllowGet);
        }

        #endregion











        //Guardar NuevaQuincena en la Tbl_historicoQuincenasRegistradas
        public ActionResult RegistrarNuevaQuincena(int NuevaQuincesa) 
        {
            return Json(NuevaQuincesa, JsonRequestBehavior.AllowGet);
        }




        //*************************************************************************************************************************************************************//
        //************************************************************************************************************************************************************//
        //***********************************************************************************************************************************************************//
                                            // Metodos para FOLIAR nominas con PAGOMATICOS por nomina o todas las nominas  //

        public ActionResult FoliarPorIdNominaPagomatico(int IdNomina, string NumeroQuincena /*, int  IdBanco*/) 
        {
            string Observa = "TARJETA";

            List<AlertasAlFolearPagomaticosDTO> errores = FoliarNegocios.FolearPagomaticoPorNomina(IdNomina, NumeroQuincena.Substring(1, 3), Observa).ToList() ;


            return Json(errores, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FoliarTodasNominas(string NumeroQuincena)
        {
            string Observa = "TARJETA";

            List<AlertasAlFolearPagomaticosDTO> errores =  FoliarNegocios.FolearPagomaticoTodasLasNominas(NumeroQuincena, Observa).OrderBy(x => x.Id_Nom).ToList();

            return Json( errores, JsonRequestBehavior.AllowGet);
        }


        /*************************************************************************************************************************************************************/
                               /*******  Pinta tabla con el detalle de una nomina para saber si esta foliada y su detalle para pagomaticos  ******/
        public ActionResult EstaFoliadaIdNominaPagomatico(int IdNom) 
        {
            List<AlertaDeNominasFoliadasPagomatico> resultadoAlertas = FoliarNegocios.EstaFoliadaNominaSeleccionadaPagoMatico(IdNom).ToList();

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

        /*************************************************************************************************************************************************************/
                                 /****** pinta una tabla con el detalle de todas las nominas para saber si estan foliadas y sus detalles ******/
        public ActionResult EstanFoliadasTodasNominaPagomatico(string NumeroQuincena)
        {
            List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).ToList();


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



        //**************************************************************************************************************************************************************//
        //*************************************************************************************************************************************************************//
        //************************************************************************************************************************************************************//
        // Metodos para FOLIAR nominas con cheques (Formas de pagos) //
        public ActionResult FoliarNominaFormaPago(RevicionFormasPagoModel NuevaFoliacionNomina)
        {  
                List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();

            //el grupo de nomina pertenece a los que se folean por el campo sindizato
            // 1 = le pertenece a las nominas general y descentralizada
            // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 


            //obtiene los detalles de una nomina en especifico filtrado por el Id_Nom de bitacora
            var detalleIdNomina = FoliarNegocios.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(NuevaFoliacionNomina.IdNomina);

            if (detalleIdNomina.Nomina == "01" && NuevaFoliacionNomina.Sindicato == false && NuevaFoliacionNomina.Confianza == false || detalleIdNomina.Nomina == "02" && NuevaFoliacionNomina.Sindicato == false && NuevaFoliacionNomina.Confianza == false)
            {
                //Verifica que si la nomina es General o Descentralizada el usuario haya escogido un tipo de foliacion por SINDICATO o CONFIANZA
                return Json(new
                {
                    RespuestaServidor = 99,
                    Error = "¿Desea foliar sindicalizados o de confianza?",
                    Solucion = "Asegurese de seleccionar un item de sindicato o confianza en el modal Detalles de nomina"

                });
            }
            else
            {


                if (NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInhabilitadoInicial == 0 && NuevaFoliacionNomina.RangoInhabilitadoFinal == 0 || NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInhabilitadoInicial != 0 && NuevaFoliacionNomina.RangoInhabilitadoFinal == 0 || NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInhabilitadoInicial == 0 && NuevaFoliacionNomina.RangoInhabilitadoFinal != 0)
                {
                    //Verifica que ambos campos de inhabilitados esten llenos  
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "Rangos de Inhabilitados no llenados correctamente",
                        Solucion = "Asegurese de que los rangos Inabilitados esten llenados ( Tanto el inicial como final)"

                    });
                }
                else if (NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInhabilitadoInicial > NuevaFoliacionNomina.RangoInhabilitadoFinal)
                {
                    //verififica que el rango inhabilitado inicial no sea mayor que el final
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "'Revise sus numeros de folios de inhabilitacion'",
                        Solucion = "El folio inicial no puede ser mas grande que el final"

                    });


                }
                else if (NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial > NuevaFoliacionNomina.RangoInhabilitadoInicial || NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial > NuevaFoliacionNomina.RangoInhabilitadoFinal)
                {
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "El rango inicial de folicion es menor que algun rango inhabilitado ",
                        Solucion = " 'Revise sus numeros de folios inhabilitados' "
                    });
                }
                else if (NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial >= NuevaFoliacionNomina.RangoInhabilitadoInicial && NuevaFoliacionNomina.RangoInicial >= NuevaFoliacionNomina.RangoInhabilitadoFinal /*|| NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial > NuevaFoliacionNomina.RangoInhabilitadoInicial && NuevaFoliacionNomina.RangoInicial < NuevaFoliacionNomina.RangoInhabilitadoFinal*/)
                {
                    //NuevaRevicion.Inhabilitado && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoInicial && NuevaRevicion.RangoInicial < NuevaRevicion.RangoInhabilitadoFinal  &&  NuevaRevicion.RangoInhabilitadoInicial <= NuevaRevicion.RangoInhabilitadoFinal
                    // (NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial > NuevaFoliacionNomina.RangoInhabilitadoInicial || NuevaFoliacionNomina.Inhabilitado && NuevaFoliacionNomina.RangoInicial > NuevaFoliacionNomina.RangoInhabilitadoFinal)


                    //para que este bien los folios de inhabilitacion el (( RANGOINICIAL < RANGOINHABILITADOINICIAL YY RANGOINHABILITADOFINAL > RANGOINHABILITADOINICIAL))
                    return Json(new
                    {
                        RespuestaServidor = 99,
                        Error = "El folio inicial no concuerdan con los folios a inhabilitar ",
                        Solucion = " 'Revise sus numeros de folios inhabilitados' "
                    });

                }
                else
                {


                    FoliarFormasPagoDTO foliarNomina = new FoliarFormasPagoDTO();

                    foliarNomina.IdNomina = NuevaFoliacionNomina.IdNomina;
                    foliarNomina.Delegacion = NuevaFoliacionNomina.Delegacion;
                     foliarNomina.Sindicato = NuevaFoliacionNomina.Sindicato;
                    foliarNomina.Confianza = NuevaFoliacionNomina.Confianza;
                    foliarNomina.IdBancoPagador = NuevaFoliacionNomina.IdBancoPagador;
                    foliarNomina.RangoInicial = NuevaFoliacionNomina.RangoInicial;

                    ///por si el usuario habilita la casilla inhabilitados aqui se rescatan  
                    foliarNomina.Inhabilitado = NuevaFoliacionNomina.Inhabilitado;
                    foliarNomina.RangoInhabilitadoInicial = NuevaFoliacionNomina.RangoInhabilitadoInicial;
                    foliarNomina.RangoInhabilitadoFinal = NuevaFoliacionNomina.RangoInhabilitadoFinal;


                    // propiedad usada para saber a que grupo de nomina corresponde 
                    // 1 = le pertenece a las nominas general y descentralizada
                    // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 
                    foliarNomina.GrupoNomina = NuevaFoliacionNomina.GrupoNomina;

                    string Observa = "CHEQUE";









                    //HACER QUE LA FUNCION DE AQUI ABAJO FUNCIONE 
                    // Y 
                    // CREAR UNA TABLA DENTRO DE UN MODAL PARA MOSTRAR LOS FOLIOS DE CHEQUES QUE TIENEN PROBLEMAS 
                    string consultaPersonal = "";
                    int TotalDeRegistrosAFoliar = 0;

                 var datosCompletosObtenidos = FoliarNegocios.ObtenerRegistroNominaPorId(NuevaFoliacionNomina.IdNomina);

                    if (datosCompletosObtenidos.Nomina == "01" || datosCompletosObtenidos.Nomina == "02")
                    {
                        //Obtener la consulta a la que corresponde la delegacion para la nomina general y descentralizada
                       // ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(datosCompletosObtenidos.An);    
                       // consultaPersonal = nuevaConsulta.ObtenerNumeroDeRegistroFormasDePagoGeneralYDesc(NuevaFoliacionNomina.Delegacion, NuevaFoliacionNomina.Sindicato);
                        consultaPersonal = ConsultasSQLSindicatoGeneralYDesc.ObtenerNumeroDeRegistroFormasDePagoGeneralYDesc(datosCompletosObtenidos.An, NuevaFoliacionNomina.Delegacion, NuevaFoliacionNomina.Sindicato);
                         TotalDeRegistrosAFoliar = FoliarNegocios.ObtenerNumeroDeRegistrosDeConsulta(consultaPersonal);

                        //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
                        List<FoliosAFoliarInventario> chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaFoliacionNomina.IdBancoPagador, NuevaFoliacionNomina.RangoInicial, TotalDeRegistrosAFoliar, NuevaFoliacionNomina.Inhabilitado, NuevaFoliacionNomina.RangoInhabilitadoInicial, NuevaFoliacionNomina.RangoInhabilitadoFinal).ToList();

                        List<FoliosAFoliarInventario> foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();



                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        //** comentar este coddigo y descomentar el de abajo para saber la disponibilidad de cheques **//
                        Advertencias = FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa/*, chequesVerificadosFoliar*/);


                        // INICICIO PARA VERIFICAR FOLIOS ------DESCOMENTAR PARA SU FUNCIONAMIENDO
                        /*

                        //Si todos los folios no tienen incidencias el proceso continua su rumbo 
                        if (foliosNoDisponibles.Count == 0 && chequesVerificadosFoliar.Count > 0)
                        {
                            Advertencias = FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa, chequesVerificadosFoliar);
                        }
                        else if (chequesVerificadosFoliar.Count == 0)
                        {
                            //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                            return Json(new
                            {
                                RespuestaServidor = 97,
                                Error = "No existe el folio inicial ingresado para la foliacion"
                            }) ;
                        }
                        else
                        {
                            //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                            return Json(new
                            {
                                RespuestaServidor = 98,
                                FoliosConIncidencias = foliosNoDisponibles
                            });
                        }
                        */
                        // INICICIO PARA VERIFICAR FOLIOS ------DESCOMENTAR PARA SU FUNCIONAMIENDO
                      



                    }
                    else 
                    {

                        /**********************************************************************************************************************************/
                        /**********************************************************************************************************************************/
                        //El grupo corresponde TODAS LAS NOMINA CON EXCEPCION DEL GRUPO 1 

                        //para las nominas que no son pension
                        ConsultasSQLOtrasNominasConCheques crearConsultaNominasSinSindicalizados = new ConsultasSQLOtrasNominasConCheques();

                        //OBTIENE UNA CONSULTA DEPENDIENDO DEL TIPO DE NOMINA 
                        if (datosCompletosObtenidos.Nomina.Equals("08"))
                        {
                            //para las nominas que si son pension 
                            consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerTotalRegistrosPensionAlimenticiaFoliar(NuevaFoliacionNomina.Delegacion, datosCompletosObtenidos.An);
                            TotalDeRegistrosAFoliar = FoliarNegocios.ObtenerNumeroDeRegistrosDeConsulta(consultaPersonal);
                        }
                        else
                        {
                            consultaPersonal = crearConsultaNominasSinSindicalizados.ObtenerTotalRegistrosDePagoFoliarOtrasNominas(NuevaFoliacionNomina.Delegacion, datosCompletosObtenidos.An);
                            TotalDeRegistrosAFoliar = FoliarNegocios.ObtenerNumeroDeRegistrosDeConsulta(consultaPersonal);
                        }

                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        /***************************************************************************************************/
                        //** comentar este coddigo y descomentar el de abajo para saber la disponibilidad de cheques **//
                         Advertencias = FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa/*, chequesVerificadosFoliar*/);


                        // INICICIO PARA VERIFICAR FOLIOS ------DESCOMENTAR PARA SU FUNCIONAMIENDO
                        /*
                        //VERIFICA QUE LOS FOLIOS A USAR ESTEN DISPONIBLES EN EL INVENTARIO 
                        List<FoliosAFoliarInventario> chequesVerificadosFoliar = FoliarNegocios.verificarFoliosEnInventarioDetalle(NuevaFoliacionNomina.IdBancoPagador, NuevaFoliacionNomina.RangoInicial, TotalDeRegistrosAFoliar, NuevaFoliacionNomina.Inhabilitado, NuevaFoliacionNomina.RangoInhabilitadoInicial, NuevaFoliacionNomina.RangoInhabilitadoFinal);

                        List<FoliosAFoliarInventario> foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();






                        //Si todos los folios no tienen incidencias el proceso continua su rumbo 
                        if (foliosNoDisponibles.Count == 0 && chequesVerificadosFoliar.Count > 0)
                        {
                            Advertencias = FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa, chequesVerificadosFoliar);
                        }
                        else if (chequesVerificadosFoliar.Count == 0)
                        {
                            //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                            return Json(new
                            {
                                RespuestaServidor = 97,
                                Error = "No existe el folio inicial ingresado para la foliacion"
                            });
                        }
                        else
                        {
                            //retorna la lista de folios que no se pueden utilizar por que tienen una incidencia
                            return Json(new
                            {
                                RespuestaServidor = 98,
                                FoliosConIncidencias = foliosNoDisponibles
                            });
                        }

                        */
                        // FIN ARA VERIFICAR FOLIOS ------DESCOMENTAR PARA SU FUNCIONAMIENDO






                    }




                }

            }








            if (Advertencias.Count() > 0)
            {
                int UltimoFolioUsado = Advertencias.Select(x => x.UltimoFolioUsado).Max();
                string registrosActualizados = Advertencias.Select(x => x.RegistrosFoliados).SingleOrDefault().ToString(); 
                return Json(new
                {
                    RespuestaServidor = 201,
                    Delegacion = "VISTA PREVIA DE LA DELGACION : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaFoliacionNomina.Delegacion).ToUpper(),
                    UltimoFolioUsado = UltimoFolioUsado,
                    RegistrosTotalesActualizados = registrosActualizados,
                    Advertencia = Advertencias,
                    DatosExtras = FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles }).ToList()
                }) ;

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = 500,
                    Delegacion = "ERROR AL CARGAR LA DELGACION : " + (FoliarNegocios.ObtenerDelegacionPorId(NuevaFoliacionNomina.Delegacion).ToUpper()),
                    UltimoFolioUsado = "Error no se puede Foliar",
                    RegistrosTotalesActualizados = 0,
                    Advertencia = Advertencias,
                    Error = "No coincide la delegacion con el sindicato"
                });
                //respuestaServer = "500";

            }


        }





    }


}