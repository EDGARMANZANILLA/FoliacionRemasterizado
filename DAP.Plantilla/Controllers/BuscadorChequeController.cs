using DAP.Plantilla.Models.BuscardorChequeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DAP.Plantilla.Controllers
{
    public class BuscadorChequeController : Controller
    {
        // GET: BuscadorCheque
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult DetallesInformativosCheques(string BuscarNumFolio)
        {
            int quincena = 2021;
            DetallesInformativosCheque nuevoDetalle = new DetallesInformativosCheque();
            nuevoDetalle.Id_nom = 1;
            nuevoDetalle.NumEmpleado = "015963";
            nuevoDetalle.Quincena =  quincena;

            nuevoDetalle.Folio = BuscarNumFolio;
            nuevoDetalle.Liquido = 9388.36m;
            nuevoDetalle.NombreBeneficiaro = "John doe";

            nuevoDetalle.EstadoCheque = "Transito";
            nuevoDetalle.ReferenciaBitacora = "2021E";
            nuevoDetalle.EstadoCancelado = null;

            nuevoDetalle.EsPenA = false;
            nuevoDetalle.BancoPagador = "Santander";
            nuevoDetalle.NombreBeneficiarioPenA = null;

            nuevoDetalle.EsRefoliado = false;


            if (nuevoDetalle.Id_nom > 0)
            {
                return PartialView(nuevoDetalle);
            }

            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentra el folio buscado"
            });
        }









    }
}