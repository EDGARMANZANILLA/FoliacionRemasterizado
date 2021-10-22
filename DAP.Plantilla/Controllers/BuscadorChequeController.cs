using AutoMapper;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Foliacion.Negocios;
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

     





        [HttpPost]
        public ActionResult DetallesInformativosCheques(int IdRegistroBuscar)
        {

            try
            {


              //  int quincena = 2021;
            DetallesInformativosCheque nuevoDetalle = Mapper.Map< DetallesRegistroDTO , DetallesInformativosCheque>(BuscadorChequeNegocios.ObtenerDetallesIdRegistro(IdRegistroBuscar));
            //nuevoDetalle.Id_nom = 1;
            //nuevoDetalle.NumEmpleado = "015963";
            //nuevoDetalle.Quincena =  quincena;

            //nuevoDetalle.Folio = "bla bla bla";
            //nuevoDetalle.Liquido = 9388.36m;
            //nuevoDetalle.NombreBeneficiaro = "John doe";

            //nuevoDetalle.EstadoCheque = "Transito";
            //nuevoDetalle.ReferenciaBitacora = "2021E";
            //nuevoDetalle.EstadoCancelado = null;

            //nuevoDetalle.EsPenA = false;
            //nuevoDetalle.BancoPagador = "Santander";
            //nuevoDetalle.NombreBeneficiarioPenA = null;

            //nuevoDetalle.EsRefoliado = true;


           


            /***********************************************************************************/

            

                if (nuevoDetalle.Id_nom > 0)
                {
                    return PartialView(nuevoDetalle);
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




        public ActionResult ObtenerFiltradoDatos(int TipoDeBusqueda, string BuscarDato)
        {
            //TipoDeBusqueda
            // 1 => CHEQUE
            // 2 => NUMERO DE EMPLEADO
            // 3 => NOMBRE BENEFICIARIO

            //BuscadorChequeNegocios.BuscarDatoPorFiltro( TipoDeBusqueda, BuscarDato);

            // List<ProgramaDTO> listado = Mapper.Map<IEnumerable<Programas>, List<ProgramaDTO>>(ProgramasNegocios.ObtenerActivosDelAnio(DateTime.Now.Year));

            List<ElementosBuscador> elementosEncontrados2 = Mapper.Map<List<ResultadoObtenidoParaSelect2>,List<ElementosBuscador>>(Foliacion.Negocios.BuscadorChequeNegocios.BuscarDatoPorFiltro(TipoDeBusqueda, BuscarDato).ToList());
            //List <ElementosBuscador> elementosEncontrados = new List<ElementosBuscador>();



            //ElementosBuscador nuevoElementosEncontrado = new ElementosBuscador();
            //nuevoElementosEncontrado.id = 1;
            //nuevoElementosEncontrado.text = "Edgar";
            //elementosEncontrados.Add(nuevoElementosEncontrado);


            //ElementosBuscador nuevoElementosEncontrado2 = new ElementosBuscador();
            //nuevoElementosEncontrado2.id = 2;
            //nuevoElementosEncontrado2.text = "TILIN";
            //elementosEncontrados.Add(nuevoElementosEncontrado2);



            return Json(elementosEncontrados2, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Buscar(ElementosBuscador BuscarElemento /*int BuscarElemento_id, string BuscarElemento_text*/)
        {


            List<DetallesBusquedaModels> detallesRegistrosEncontrados = new List<DetallesBusquedaModels>(); 
            /****/

            if (BuscarElemento != null)
            {
                //tipoDeBusqueda
                // 1 => CHEQUE
                // 2 => NUMERO DE EMPLEADO
                // 3 => NOMBRE BENEFICIARIO
                switch (BuscarElemento.tipoBusqueda)
                {
                    case 1:
                        //su Id contiene el id de la tabla donde esta localizado el registro
                        detallesRegistrosEncontrados = Mapper.Map< List<DetallesBusqueda>, List<DetallesBusquedaModels>>(BuscadorChequeNegocios.ObtenerDetallesDeCheque(BuscarElemento.id));


                        break;
                    case 2:
                        //su id trae el numero de empleado que se esta buscando
                         detallesRegistrosEncontrados = Mapper.Map< List<DetallesBusqueda>, List<DetallesBusquedaModels>>( BuscadorChequeNegocios.ObtenerDetallesNumEmpleado(BuscarElemento.id));
                        break;
                    case 3:
                        //Su id trae el numero de empleado 
                        detallesRegistrosEncontrados = Mapper.Map<List<DetallesBusqueda>, List<DetallesBusquedaModels>>(BuscadorChequeNegocios.ObtenerDetallesNumEmpleado(BuscarElemento.id));
                        break;
                    default:
                        // code block
                        break;
                }

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
                RegistrosEncontrados = detallesRegistrosEncontrados,
                MensajeError = "No se encuentra el folio buscado"
            });
        }


      




    }
}