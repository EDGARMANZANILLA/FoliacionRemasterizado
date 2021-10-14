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




        public ActionResult ObtenerFiltradoDatos(int TipoDeBusqueda, string BuscarDato)
        {
            //TipoDeBusqueda
            // 1 => CHEQUE
            // 2 => NUMERO DE EMPLEADO
            // 3 => NOMBRE BENEFICIARIO

            switch (TipoDeBusqueda)
            {
                case 1:
                    // Bloque de codigo para buscar por CHEQUE
                    break;
                case 2:
                    // Bloque de codigo para buscar por NUMERO DE EMPLEADO 
                    break;
                case 3:
                    // Bloque de codigo para buscar por el nombre del BENEFICIARIO DEL CHEQUE
                    break;
                default:

                    // aun por verificar si el usuario no conoce ningun dato => funcionalidad a futuro
                    // code block
                    break;
            }


            List <ElementosBuscador> elementosEncontrados = new List<ElementosBuscador>();



            ElementosBuscador nuevoElementosEncontrado = new ElementosBuscador();
            nuevoElementosEncontrado.id = 1;
            nuevoElementosEncontrado.text = "Edgar";
            elementosEncontrados.Add(nuevoElementosEncontrado);


            ElementosBuscador nuevoElementosEncontrado2 = new ElementosBuscador();
            nuevoElementosEncontrado2.id = 2;
            nuevoElementosEncontrado2.text = "TILIN";
            elementosEncontrados.Add(nuevoElementosEncontrado2);



            return Json(elementosEncontrados, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Buscar(ElementosBuscador BuscarElemento /*int BuscarElemento_id, string BuscarElemento_text*/)
        {

            if (BuscarElemento != null)
            {

            }
            else 
            {

                return Json(new
                {
                    RespuestaServidor = 400,
                    MensajeError = "No se selecciono un elemento"
                });

            }



            return Json(new
            {
                RespuestaServidor = 201,
                MensajeError = "No se encuentra el folio buscado"
            });
        }


      




    }
}