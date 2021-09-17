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
            return Json(FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles }).ToList() , JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerNombreNominas(string NumeroQuincena)
        {

            var contenedoresEncontrados = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);


            //Se crea la variable de session cada vez que inicia el proceso de foliar



            return Json(contenedoresEncontrados, JsonRequestBehavior.AllowGet);
        }



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





        #region Metodos para la Foliacion por medio de Formas de pago
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

            //obtiene los detalles de una nomina en especifico filtrado por el Id_Nom de bitacora
            var detalleIdNomina = FoliarNegocios.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(NuevaRevicion.IdNomina);
            
            string NombreBanco = FoliarNegocios.ObtenerBancoPorID(NuevaRevicion.IdBancoPagador);

            string ultimoFolioUsar = "";

            string rutaAlmacenamiento = "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionNominaFormasDePago" + NuevaRevicion.IdNomina + ".pdf";


            if (NuevaRevicion.GrupoNomina == 1)
            {
               
                //Obtener la consulta a la que corresponde la delegacion 
                ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(detalleIdNomina.An);
                string consulta = nuevaConsulta.ObtenerConsultaSindicatoFormasDePago(NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);

           
                //obtiene los datos como quedarian posiblemente al momento de folear 
                List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consulta, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));



                //Crear reporte 
                DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                //Pasa el nombre de la ruta
                dtsRevicionFolios.Ruta.AddRutaRow( "RUTA "+FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), " LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper() );


                //cargar datos al reporte 
               
                foreach (var dato in datosRevicionObtenidos)
                {

                    ultimoFolioUsar = dato.Num_Che;
                    dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
                }


                // Materializa el reporte en un pdf que pone en una carpeta 
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

                rd.SetDataSource(dtsRevicionFolios);

                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);



            

            } else if (NuevaRevicion.GrupoNomina == 2)
            {
               
                string consultaOtrasNominas="";

                //verifica que si la nomina a verificar es pension "08" entonces selecciona una consulta deacuerdo a la nomina seleccionada
                if (detalleIdNomina.Nomina != "08")
                {
                    ConsultasSQLOtrasNominasConCheques NuevaConsulta = new ConsultasSQLOtrasNominasConCheques();
                    consultaOtrasNominas = NuevaConsulta.ObtenerConsultaConOrdenamientoFormasDePago(NuevaRevicion.Delegacion, detalleIdNomina.An);
                }
                else 
                {
                    ConsultasSQLOtrasNominasConCheques NuevaConsultaPension= new ConsultasSQLOtrasNominasConCheques();
                    consultaOtrasNominas = NuevaConsultaPension.ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticia(NuevaRevicion.Delegacion , detalleIdNomina.An);
                }

                //Si no esta vacia procede obtener los datos y y rellena el pdf  
                if (!string.IsNullOrWhiteSpace(consultaOtrasNominas)) 
                {
                    List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consultaOtrasNominas, detalleIdNomina.Nomina, Convert.ToInt32(NuevaRevicion.RangoInicial), NombreBanco, NuevaRevicion.Inhabilitado, Convert.ToInt32(NuevaRevicion.RangoInhabilitadoInicial), Convert.ToInt32(NuevaRevicion.RangoInhabilitadoFinal));


                    //Crear reporte 
                    DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                    //Pasa el nombre de la ruta que es parte del encabezado del reporte
                    dtsRevicionFolios.Ruta.AddRutaRow("RUTA" + FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina), "LA DELEGACION SELECCIONADA ES : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper());
                    
                    //cargar datos al reporte 
                    foreach (var dato in datosRevicionObtenidos)
                    {   
                        ultimoFolioUsar = dato.Num_Che;
                        dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
                    }


                    // Materializa el reporte en un pdf que pone en una carpeta 
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

                    rd.SetDataSource(dtsRevicionFolios);

                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);
                }
            

            }







            if (System.IO.File.Exists(rutaAlmacenamiento) && ultimoFolioUsar != "")
            {

                return Json(new
                {
                    RespuestaServidor = "201",
                    Delegacion = "VISTA PREVIA DE LA DELGACION : " + FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper(),
                    UltimoFolioUsado = ultimoFolioUsar,
                    FoliosTotal = (Convert.ToInt32(ultimoFolioUsar) - NuevaRevicion.RangoInicial) + 1, 
                    DatosExtras = FoliarNegocios.ObtenerDetalleBancoFormasDePago().Select(y => new { y.NombreBanco, y.Cuenta, y.Tbl_Inventario.FormasDisponibles }).ToList()
                });

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    Delegacion = "ERROR AL CARGAR LA DELGACION : " + (FoliarNegocios.ObtenerDelegacionPorId(NuevaRevicion.Delegacion).ToUpper()),
                    UltimoFolioUsado = "Error no se puede simular la Foliacion",
                    FoliosTotal = 0,
                    Error = "No coincide la delegacion con el sindicato"
                });
                //respuestaServer = "500";

            }

          //  return Json("404", JsonRequestBehavior.AllowGet);
        }

        #endregion











        //Guardar NuevaQuincena en la Tbl_historicoQuincenasRegistradas
        public ActionResult RegistrarNuevaQuincena(int NuevaQuincesa) 
        {


            return Json(NuevaQuincesa, JsonRequestBehavior.AllowGet);

        }





        //********************************************************//
        //********************************************************//
        // Metodos para folear //
        public ActionResult FoliarPorIdNominaPagomatico(int IdNomina, string NumeroQuincena /*, int  IdBanco*/) 
        {
            string Observa = "TARJETA";
            Convert.ToInt32(NumeroQuincena.Substring(1, 3));
            List<string> errores =FoliarNegocios.FolearPagomaticoPorNomina(IdNomina, NumeroQuincena.Substring(1, 3), Observa);

            if (errores.Count() == 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }


            return Json(errores, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FoliarTodasNominas(string NumeroQuincena)
        {
            string Observa = "TARJETA";
            //List<string> errores = FoliarNegocios.FolearPagomaticoTodasLasNominas(NumeroQuincena, Observa);

            List<string> errores = new List<string>();

            errores.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : 326. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
            errores.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : 326. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
            errores.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : 326. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
            errores.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : 326. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
            errores.Add("Error: no existe ningun empleado con tarjeta para folear en la nomina : 326. Asegurese que de existe por lo menos un empleado con tarjeta o elija 'FOLEAR POR FORMAS DE PAGO'");
            

            if (errores.Count() == 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }


            return Json(errores, JsonRequestBehavior.AllowGet);
        }


    }


}