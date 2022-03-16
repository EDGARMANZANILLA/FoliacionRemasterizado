using AutoMapper;
using DAP.Foliacion.Negocios;
using DAP.Plantilla.Models.CrearReferencia_CanceladosModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;

namespace DAP.Plantilla.Controllers
{
    public class CrearReferencia_CanceladosController : Controller
    {
        // GET: CrearReferencia_Cancelados
        public ActionResult Index()
        {
            //  List<CrearReferenciaDTO> listaEncontrada =  CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year);

            List<ReferenciaCanceladoModel> listaEncontrada = Mapper.Map<List<CrearReferenciaDTO>, List<ReferenciaCanceladoModel>>(CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year));

          

            return View(listaEncontrada);
        }










        [HttpPost]
        public ActionResult CrearReferenciaCancelado(int NuevoNumeroReferencia) 
        {
            bool bandera = false;
            int idDevuelto = CrearReferencia_CanceladosNegocios.CrearNuevaReferenciaCancelados(NuevoNumeroReferencia);

            if (idDevuelto > 0)
            {
                bandera = true;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult InactivarReferenciaCancelado(int Eliminar_IdReferenciaCancelacion)
        {
            int idDevuelto = CrearReferencia_CanceladosNegocios.InactivarReferenciaCancelados(Eliminar_IdReferenciaCancelacion);

            return Json(idDevuelto, JsonRequestBehavior.AllowGet);
        }





       

    }
}