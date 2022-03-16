using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Entidades.DTO.FinalizarReferencia_CanceladosDTO;
using DAP.Foliacion.Negocios;

namespace DAP.Plantilla.Controllers
{
    public class FinalizarReferencia_CanceladosController : Controller
    {
        // GET: FinalizarReferencia_Cancelados

        public ActionResult Index_FinalizarReferencia()
        {

            Dictionary<int, string> referenciasActivasFiltradas = FinalizarReferencia_CanceladosNegocios.ObtenerReferenciasActivas(DateTime.Now.Year);


            return View(referenciasActivasFiltradas);
        }



        [HttpPost]
        public ActionResult ObtnerDetallesDentroReferencia(int IdReferencia)
        {

            List<DetallePagoDTO> detallesEnReferencia = FinalizarReferencia_CanceladosNegocios.ObtenerDetalleDePagosDentroReferencia(IdReferencia);

            if(detallesEnReferencia.Count > 0)
            {
                return Json(new
                {
                    Bandera = true,
                   
                    ListaDatos = detallesEnReferencia
                });

            }

            return Json(new  {Bandera = true });
        }



        public ActionResult RemoverNumeroReferenciaCancelacionDeUnIdRegistro(int IdRegistroRemoverReferencia)
        {
            //Mensajes de ERRORES
            // 1 => /No es un cheque sino una dispercion (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 2 => /No Existe la referencia (APLICA PARA AGREGAR, ACTUALIZAR O REVOCAR UN CHEQUE DE LA REFERENCIA DE CANCELACION)
            // 3 => No se puede cambiar un cheque a la misma referencia
            // 4 => La referencia no puede ser removida porque aun no tiene una (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)

            //Mensajes Exitosos
            // 6 => /Se agrego a una referencia exitosamente
            // 7 => /Se Cambio de referencia con Exito
            // 8 => La referencias fue removida con exito de la referencia de cancelacion (APLiCA SOLO PARA QUITAR UN PAGO DE UNA REFERENCIA)

            string mensaje = "";
            string solucion = "";
            int errorRecibido = 0;

            errorRecibido = BuscadorChequeNegocios.RevocarCheque_ReferenciaCancelado(IdRegistroRemoverReferencia);


            switch (errorRecibido)
            {
                //Mensajes de error
                case 1:
                    mensaje = " No se puede agregar una dispercion a una referencia ";
                    solucion = "Solicite una suspencion de la forma de pago";
                    break;
               
                case 4:
                    mensaje = "No se puede revocar una forma de pago de una referencia, si nunca se a estado en una ";
                    solucion = "Asegurese que la forma de pago se encuentre cargada en una referencia";
                    break;

                case 8:
                    mensaje = "La referencia fue removida exitosamente "; 
                    break;


                default:
                    mensaje = "La peticion no fue procesada exitodamente";
                    solucion = "Reintente de nuevo mas tarde";
                    break;
            }

            return Json(new
            {
                NumeroMensaje = errorRecibido,
                Mensaje = mensaje,
                Solucion = solucion
            });

        }







    }
}