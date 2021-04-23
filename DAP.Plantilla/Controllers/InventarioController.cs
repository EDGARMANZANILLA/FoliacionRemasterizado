using DAP.Plantilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Negocios;


namespace DAP.Foliacion.Plantilla.Controllers
{
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {

            List<InventarioModel> BancosMostrar = new List<InventarioModel>();

            var InventariosActivos = Negocios.InventarioNegocios.ObtenerInventarioActivo();  
            foreach (var inventarioBanco in InventariosActivos)
            {
                InventarioModel NuevoBanco = new InventarioModel();

                NuevoBanco.Id = inventarioBanco.Id;
                NuevoBanco.NombreBanco = inventarioBanco.Tbl_CuentasBancarias.NombreBanco;
                NuevoBanco.FormasDisponibles = inventarioBanco.FormasDisponibles;
                NuevoBanco.UltimoFolioInventario = inventarioBanco.UltimoFolioInventario;
                NuevoBanco.UltimoFolioQuincena = inventarioBanco.UltimoFolioQuincena;
                NuevoBanco.FormasQuincena1 = inventarioBanco.FormasQuincena1;
                NuevoBanco.FormasQuincena2 = inventarioBanco.FormasQuincena2;
                NuevoBanco.EstimadoMeses = inventarioBanco.EstimadoMeses;

                BancosMostrar.Add(NuevoBanco);

             }


            return View(BancosMostrar);
        }


        public ActionResult Agregar(string NombreBanco)
        {

            ViewBag.NombreBancoSeleccionado = NombreBanco;

            return View();
        }


        public ActionResult DetalleBanco(string NombreBanco)
        {

            ViewBag.NombreBancoSeleccionado = NombreBanco;

            return View();
        }





        public ActionResult Ajustar()
        {
            return View();
        }


        public ActionResult Solicitar()
        {
            return View();
        }

        
        public ActionResult Inhabilitados()
        {


            return View();
        }




        //metodos por post

        [HttpPost]
        public JsonResult GuardarInventarioAgregado(List<AgregarInventarioModel> listaDeContenedores,string NumOrden, string banco)
        {
            bool bandera = false;

            var contenedores = listaDeContenedores;
            try
            {
                int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

                foreach (AgregarInventarioModel nuevoContenedor in listaDeContenedores) 
                {
                    bandera = Negocios.InventarioNegocios.GuardarInventarioContenedores(idBanco, NumOrden, nuevoContenedor.IteradorDeContenedores, nuevoContenedor.FInicial, nuevoContenedor.FFinal, nuevoContenedor.TotalFormas);
                }


              }
            catch (Exception e)
            {
                bandera = false;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }



    }
}