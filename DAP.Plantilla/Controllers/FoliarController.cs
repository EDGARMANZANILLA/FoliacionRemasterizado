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

namespace DAP.Plantilla.Controllers
{
    public class FoliarController : Controller
    {
        // GET: Foliar
        public ActionResult Index()
        {
          //  ObtenerNombreNominas("2112");
            return View();
        }

        public ActionResult FoliarXPagomatico(string NumeroQuincena)
        {
            Session.Remove("NumeroQuincena");
            Session["NumeroQuincena"] = Convert.ToInt32( NumeroQuincena.Substring(1,3));
            ViewBag.NumeroQuincena = NumeroQuincena;
            Dictionary<int, string> ListaNombresQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);
           // ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancosParaPagomatico(); 

            return PartialView(ListaNombresQuincena);
        }

        public ActionResult FoliarXFormasPago(string NumeroQuincena)
        {
            Session.Remove("NumeroQuincena");
            Session["NumeroQuincena"] = Convert.ToInt32(NumeroQuincena.Substring(1, 3));
            ViewBag.NumeroQuincena = NumeroQuincena;
            Dictionary<int, string> ListaNombresNominaQuincena = FoliarNegocios.ObtenerNominasXNumeroQuincena(NumeroQuincena);
            ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancoParaFormasPago(); 


            return PartialView(ListaNombresNominaQuincena);
        }




        [HttpPost]
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
                    datosNominaObtenidos.Add( FoliarNegocios.ObtenerDatosFoliadosPorNominaRevicion(nuevaNomina.Nomina, nuevaNomina.An, Convert.ToInt32(NumeroQuincena.Substring(1, 3))));
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


            dtsRevicionGeneralFolios.Ruta.AddRutaRow("Contiene "+i+" registros de dispercion de todas las nominas de la quincena No. " + NumeroQuincena);



            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

            rd.SetDataSource(dtsRevicionGeneralFolios);


            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionGeneralNominas"+NumeroQuincena+".pdf");

            return Json("TODO LISTO", JsonRequestBehavior.AllowGet);
        }


        public ActionResult RevisarPorIdNomina(int IdNomina )
        {
            //Buscar el campo an de la bitacora al que pertenece  el IdNomina
            //Crear un metodo que reciba el Idnomina en el negocio que obtenga los datos que los actualize en la lista(no en SQL)
            //regresarlo al controlador y pasarlo a un pdf en cristal report para mostrarselo al usuario 

            //si contiene mas de un elemento se le puede aplicar la llave ya que si contiene el compo ESP_NOM lanomina
            //si solo contiene un valor y los otros estan vacios esa nomina corresponde a una Pencion Alimenticia
            List<string> anApAdEncontrados = FoliarNegocios.ObtenerAnBitacoraIdNum(IdNomina);

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
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaRevicion(numeroNomina , anApAdEncontrados[0], quincena);
            }
            else 
            {
                //entra a este apartado al saber que ap = "" y ad = "" lo que quiere decir que se trata de una nomina de pension alimenticia
                //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
               // nombreBanco = FoliarNegocios.ObtenerBancoPorID(1);
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaPENALRevicion(numeroNomina ,anApAdEncontrados[0], quincena);
            }
         

            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow( FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina) );


            foreach (var resultado in datosNominaObtenidos)
            {
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
            }


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

            rd.SetDataSource(dtsRevicionFolios);

         
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\"+"RevicionNomina"+IdNomina+".pdf");

            return Json("TODO LISTO", JsonRequestBehavior.AllowGet);
        }





        #region Metodos para la Foliacion por medio de Formas de pago
        public ActionResult ObtenerDetalleNominaPorIdNominaParaModal(int IdNomina)
        {
            //ObtenerDetalleNominaParaCheques

            var resumenDatosTablaModal = FoliarNegocios.ObtenerDetallesNominaParaCheques(IdNomina).OrderBy( X => X.Delegacion);

            string NombreModal = FoliarNegocios.ObtenerNombreModalPorIDNomina(IdNomina);


            return Json(new
            {
                TablaModal = resumenDatosTablaModal,
                NombreDetalladoNomina = NombreModal
            });

           // return Json(resultado, JsonRequestBehavior.AllowGet);
        }





        
        public ActionResult RevisarNominaFormaPago(RevicionFormasPagoModel NuevaRevicion)
        {   //el grupo de nomina pertenece a los que se folean por el campo sindizato
            // 1 = le pertenece a las nominas general y descentralizada
            // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 

           

            if (NuevaRevicion.GrupoNomina == 1) 
            {
                //En la posicion 1 viene el AN como se encuentra en bitacora 
                List<string> An = FoliarNegocios.ObtenerAnBitacoraIdNum(NuevaRevicion.IdNomina);



                ConsultasSQLSindicatoGeneralYDesc nuevaConsulta = new ConsultasSQLSindicatoGeneralYDesc(An[0]);
                string consulta = nuevaConsulta.ObtenerConsultaSindicaroFormasDePago(NuevaRevicion.Delegacion, NuevaRevicion.Sindicato);

                string NombreBanco =FoliarNegocios.ObtenerBancoPorID(NuevaRevicion.IdBancoPagador);

                //An[3] viene el numero de la nomina correcpondiente a alpha
                List<DatosReporteRevisionNominaDTO> datosRevicionObtenidos = FoliarNegocios.ObtenerDatosRevicionPorDelegacionFormasPago(consulta, An[3], Convert.ToInt32(NuevaRevicion.RangoInicial) , NombreBanco );



                //Crear reporte 
                DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

                dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(NuevaRevicion.IdNomina));

                string ultimoFolioUsar = "";
                foreach (var dato in datosRevicionObtenidos)
                {

                    ultimoFolioUsar = dato.Num_Che;
                    dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(dato.Id), dato.Partida, dato.Nombre, dato.Deleg, dato.Num_Che, dato.Liquido, dato.CuentaBancaria, dato.Num, dato.Nom);
                }


                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

                rd.SetDataSource(dtsRevicionFolios);
                
                string rutaAlmacenamiento = "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\" + "RevicionNominaFormasDePago" + NuevaRevicion.IdNomina + ".pdf";

                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, rutaAlmacenamiento);


                
                if (System.IO.File.Exists(rutaAlmacenamiento) && ultimoFolioUsar != "")
                {
                   
                    return Json(new
                    {
                        RespuestaServidor = "201",
                        UltimoFolioUsado = ultimoFolioUsar,
                        FoliosTotal = (Convert.ToInt32( ultimoFolioUsar) - NuevaRevicion.RangoInicial) + 1
                    });

                     // respuestaServer = "201";
                }
                else 
                {
                    return Json(new
                    {
                        RespuestaServidor = "500",
                        UltimoFolioUsado = "Error no se puede simular la Foliacion",
                        FoliosTotal = 0,
                        Error = "No coincide la delegacion con el sindicato"
                    });
                    //respuestaServer = "500";
                    
                }
               
            }
            
            return Json("404", JsonRequestBehavior.AllowGet);
        }

        #endregion



    }


}