using DAP.Plantilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DAP.Plantilla.Controllers
{
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {

            List<InventarioModel> BancosMostrar = new List<InventarioModel>();

            // var InventariosActivos = Negocios.FoliacionNegocios.ObtenerInventarioActivo();

            // foreach (var inventarioBanco in InventariosActivos)
            // {
            //InventarioModel NuevoBanco = new InventarioModel();
            //NuevoBanco.Id = inventarioBanco.Id;
            //NuevoBanco.NombreBanco = inventarioBanco.Tbl_CuentasBancarias.NombreBanco;
            //NuevoBanco.FormasDisponibles = inventarioBanco.FormasDisponibles;
            //NuevoBanco.UltimoFolioInventario = inventarioBanco.UltimoFolioInventario;
            //NuevoBanco.UltimoFolioQuincena = inventarioBanco.UltimoFolioQuincena;
            //NuevoBanco.FormasQuincena1 = inventarioBanco.FormasQuincena1;
            //NuevoBanco.FormasQuincena2 = inventarioBanco.FormasQuincena2;
            //NuevoBanco.EstimadoMeses = inventarioBanco.EstimadoMeses;

            //BancosMostrar.Add(NuevoBanco);

            // }

            InventarioModel NuevoBanco = new InventarioModel();
            NuevoBanco.Id = 1;
            NuevoBanco.NombreBanco = "banco";
            NuevoBanco.UltimoFolioInventario = 150;
            NuevoBanco.UltimoFolioQuincena = 10;
            NuevoBanco.FormasQuincena1 = 12;
            NuevoBanco.FormasQuincena2 = 11;
            NuevoBanco.EstimadoMeses = 5;

            BancosMostrar.Add(NuevoBanco);




            return View(BancosMostrar);
        }


        public ActionResult Ajustar()
        {
            return View();
        }


        public ActionResult Solicitar()
        {
            return View();
        }

        public ActionResult nuevoParcial() 
        {

            List<InventarioModel> BancosMostrar = new List<InventarioModel>();



            InventarioModel NuevoBanco = new InventarioModel();
            NuevoBanco.Id = 1;
            NuevoBanco.NombreBanco = "banco";
            NuevoBanco.UltimoFolioInventario = 150;
            NuevoBanco.UltimoFolioQuincena = 10;
            NuevoBanco.FormasQuincena1 = 12;
            NuevoBanco.FormasQuincena2 = 11;
            NuevoBanco.EstimadoMeses = 5;

            BancosMostrar.Add(NuevoBanco);




            return PartialView(BancosMostrar);
           
        }


    }
}