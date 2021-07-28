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



        public ActionResult RevisarTodasNominas(string NumeroQuincena, int CuentaBancaria)
        {

           

          return Json("hoalñ", JsonRequestBehavior.AllowGet);
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



            int quincena = (int)Session["NumeroQuincena"];



            string nombreBanco = null;

            //se crea un contenedor para guardar los datos y enviarselos a cristal y el cliente u usuario vea como quedara la nomina al terminar de foliarla
            List<DatosReporteRevisionNominaDTO> datosNominaObtenidos = new List<DatosReporteRevisionNominaDTO>();

            //verifica que en la posicion 1 de ap y 2 de ad vengan vacios ya que si estan vacios quiere decir que es una nomina de pencion alimenticia
            if (anApAdEncontrados[1] != "" && anApAdEncontrados[2] != "")
            {
                //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                 nombreBanco = FoliarNegocios.ObtenerBancoPorID(2);
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaRevicion(anApAdEncontrados[0], quincena, nombreBanco);
            }
            else 
            {
                //entra a este apartado al saber que ap = "" y ad = "" lo que quiere decir que se trata de una nomina de pension alimenticia
                //Se envia el id de banco 2 por que todos los pagomaticos son con santander. si se quiere cambiar de banco sustituir el id que se envia
                 nombreBanco = FoliarNegocios.ObtenerBancoPorID(1);
                datosNominaObtenidos = FoliarNegocios.ObtenerDatosFoliadosPorNominaPENALRevicion(anApAdEncontrados[0], quincena, nombreBanco);
            }
         

            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow( FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina) );


            foreach (var resultado in datosNominaObtenidos)
            {
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num);
            }


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt"));

            rd.SetDataSource(dtsRevicionFolios);

         
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Users\\Israel\\source\\repos\\EDGARMANZANILLA\\FoliacionRemasterizado\\DAP.Plantilla\\Reportes\\ReportesPDFSTemporales\\"+"RevicionNomina"+IdNomina+".pdf");

            return Json("TODO LISTO", JsonRequestBehavior.AllowGet);
        }






    }


}