using DAP.Plantilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Negocios;
using DAP.Foliacion.Entidades.DTO;


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

              
                NuevoBanco.NombreBanco = inventarioBanco.NombreBanco;
                NuevoBanco.Cuenta = inventarioBanco.Cuenta;
                NuevoBanco.FormasDisponibles = inventarioBanco.Tbl_Inventario.FormasDisponibles;
                NuevoBanco.UltimoFolioInventario = inventarioBanco.Tbl_Inventario.UltimoFolioInventario;
                NuevoBanco.UltimoFolioUtilizado = inventarioBanco.Tbl_Inventario.UltimoFolioUtilizado;
                NuevoBanco.FormasUsadasQuincena1 = inventarioBanco.Tbl_Inventario.FormasUsadasQuincena1;
                NuevoBanco.FormasUsadasQuincena2 = inventarioBanco.Tbl_Inventario.FormasUsadasQuincena2;
                NuevoBanco.EstimadoMeses = inventarioBanco.Tbl_Inventario.EstimadoMeses;

                BancosMostrar.Add(NuevoBanco);

             }


            return View(BancosMostrar);
        }


        public ActionResult Agregar(string NombreBanco)
        {

            ViewBag.NombreBancoSeleccionado = NombreBanco;

            return View();
        }



        public ActionResult Incidencias(string NombreBanco) 
        {
            ViewBag.NombreBancoSeleccionado = NombreBanco;

            return View();
        }


        public ActionResult DetalleBanco(string NombreBanco)
        {

            ViewBag.NombreBancoSeleccionado = NombreBanco;

            ViewBag.NumeroCuentaBanco = Negocios.InventarioNegocios.ObtenerCuentaBancariaPorNombreBanco(NombreBanco);

             int IdInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorNombreBanco(NombreBanco);
            ViewBag.IdInventario = IdInventario;


           var detallesObtenidos = Negocios.InventarioNegocios.obtenerDetallesTabla(IdInventario).ToList();

            List<DetalleBancoModel> listaDetalle = new List<DetalleBancoModel>();

            foreach (var detalle in detallesObtenidos)
            {
                DetalleBancoModel nuevoDetalle = new DetalleBancoModel();

                nuevoDetalle.NumeroOrden = detalle.Tbl_InventarioContenedores.NumOrden;
                nuevoDetalle.NumeroContenedor = detalle.Tbl_InventarioContenedores.NumContenedor;
                nuevoDetalle.NumeroFolio = detalle.NumFolio;

                if (detalle.IdIncidencia != null)
                    nuevoDetalle.Incidencia = detalle.Tbl_InventarioTipoIncidencia.Descrip_Incidencia;

                if(detalle.IdEmpleado != null)
                nuevoDetalle.NombreEmpleado = detalle.Tbl_InventarioAsignacionPersonal.NombrePersonal;

                if (detalle.FechaIncidencia != null)
                    nuevoDetalle.FechaIncidencia = detalle.FechaIncidencia.ToString();

                listaDetalle.Add(nuevoDetalle);
            }


            return View(listaDetalle);
        }


        public ActionResult Ajustar(string NombreBanco)
        {

            ViewBag.NombreBanco = NombreBanco;


           // int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);

            string numeroCuenta = Negocios.InventarioNegocios.ObtenerCuentaBancariaPorNombreBanco(NombreBanco);

            int idInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorNombreBanco(NombreBanco);

            ViewBag.NumeroCuentaBanco = numeroCuenta;
            ViewBag.IdInventario = idInventario;

            ViewBag.OrdenesEncontradas = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(idInventario);


            ViewBag.ListaNombrePersonal = Negocios.InventarioNegocios.ObtenerPersonalActivo();

            


            return View();
        }


        public ActionResult Solicitar()
        {
            //pasar nombre de bancos activos
            List<string> nombresBancos = new List<string>();

            var bancosActivos = Negocios.InventarioNegocios.ObtenerBancosConChequera();
            int numeroBancos = 0;
            foreach (var banco in bancosActivos)
            {
                numeroBancos += 1;
                nombresBancos.Add(banco.NombreBanco);
            }

            ViewBag.NumeroBancos = numeroBancos;
            ViewBag.ListaNombreBancos = nombresBancos;

            return View();
        }

        
        public ActionResult Inhabilitados()
        {
            //pasar nombre de bancos activos
            //List<string> nombresBancos = new List<string>();

            //var bancosActivos = Negocios.InventarioNegocios.ObtenerBancos();

            //foreach (var banco in bancosActivos)
            //{
            //    string nuevoBanco;

            //    int idNuevoBanco;

            //    idNuevoBanco = banco.IdCuentaBancaria;

            //    string Nuevobanco = Negocios.InventarioNegocios.ObtenerNombreBancoXId(idNuevoBanco);


            //    nombresBancos.Add(Nuevobanco);


            //}


            //ViewBag.ListaNombreBancos = nombresBancos;

            return View();
        }

        public ActionResult Inhabilitar(string NombreBanco)
        {
            ViewBag.NombreBanco = NombreBanco;


            //int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);

            ViewBag.NumeroCuentaBanco = Negocios.InventarioNegocios.ObtenerCuentaBancariaPorNombreBanco(NombreBanco);

            ViewBag.IdInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorNombreBanco(NombreBanco);

           


            ViewBag.OrdenesEncontradas = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(ViewBag.IdInventario);


            return View();
        }


      


        //metodos por post
        [HttpPost]

        //Metodo Para Agregar contenedores 
        //Revisado
        public JsonResult GuardarInventarioAgregado(List<AgregarInventarioModel> listaDeContenedores,string NumOrden, string banco)
        {
            bool bandera = false;

           // var contenedores = listaDeContenedores;
            try
            {
                //int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

                //nuevo
                int idInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorNombreBanco(banco);

                foreach (AgregarInventarioModel nuevoContenedor in listaDeContenedores) 
                {
                    bandera = Negocios.InventarioNegocios.GuardarInventarioContenedores(idInventario, NumOrden, nuevoContenedor.IteradorDeContenedores, nuevoContenedor.FInicial, nuevoContenedor.FFinal, nuevoContenedor.TotalFormas);
                }


              }
            catch (Exception e)
            {
                bandera = false;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }



        //Metodo Para crear tablas de Info de la tabla Inventariocontenedores
        public ActionResult CrearTablaInhabilitadosOAsignacion(string NombreBanco)
        {
            //int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);

            //int IdInventarioBanco = Negocios.InventarioNegocios.ObtenerIdInventarioPorNombreBanco(NombreBanco);

            var contenedoresEncontrados = Negocios.InventarioNegocios.ObtenerInfoContendoresPorBanco(NombreBanco);


          


            return Json(contenedoresEncontrados, JsonRequestBehavior.AllowGet);
        }


        //Metodo para Obtener Numeros de orden de la tabla InventarioContenedores funciona para Inhabilitar o Asignar por contenedor 
        public JsonResult ObtenerNumerosContenedores(int IdInventario, string OrdenSeleccionada)
        {
            List<string> numeroDeContenedores = new List<string>();
            var contenedoresEncontrados = InventarioNegocios.ObtenerContenedoresActivosPorIdInventario(IdInventario, OrdenSeleccionada);

            foreach (var contenedor in contenedoresEncontrados)
            {
                numeroDeContenedores.Add(Convert.ToString(contenedor.NumContenedor));
            }


            if (numeroDeContenedores.Count < 1 || numeroDeContenedores == null)
            {
                numeroDeContenedores.Add("Error al Obtener los contenedores, INTENTELO DE NUEVO");
            }

            return Json(numeroDeContenedores, JsonRequestBehavior.AllowGet);
        }



        //Metodo para Obtener Los numeros de folios del contenedor seleccionado de la tabla InventarioContenedores funciona para Inhabilitar  o Asignar por contenedor 
        public JsonResult ObtenerFoliosDelContenedor(int IdInventario, string OrdenSeleccionada, int ContenedorSeleccionado)
        {
            List<string> foliosContenedor = new List<string>();

            //int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

            var foliosDeContenedorEncontrado = InventarioNegocios.ObtenerFoliosPorContenedor(IdInventario, OrdenSeleccionada, ContenedorSeleccionado);

            foliosContenedor.Add(Convert.ToString(foliosDeContenedorEncontrado.Id));
            foliosContenedor.Add(Convert.ToString(foliosDeContenedorEncontrado.FolioInicial));
            foliosContenedor.Add(foliosDeContenedorEncontrado.FolioFinal);

            return Json(foliosContenedor, JsonRequestBehavior.AllowGet);
        }



        #region

        ////// //Metodos de Inhabilitados
        //public JsonResult ObtenerNumerosContenedores(string banco,string OrdenSeleccionada)
        //{
        //    bool bandera = false;

        //    List<int> NumeroDeContenedores = new List<int>();

        //    int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

        //    var contenedoresEncontrados = InventarioNegocios.ObtenerContenedoresActivosPorNumeroOrden(idBanco, OrdenSeleccionada);


        //    foreach (var contenedor in contenedoresEncontrados) 
        //    {
        //        int nuevoContenedor = contenedor.NumContenedor;

        //        NumeroDeContenedores.Add(nuevoContenedor);
        //    }


        //    return Json(NumeroDeContenedores, JsonRequestBehavior.AllowGet);
        //}





        //public JsonResult GuardarInhabilitaciones(int idInventarioDetalle, string banco, string cuenta, string OrdenSeleccionada, int ContenedorSeleccionado, string FolioInicial, string FolioFinal) 
        //{
        //    bool bandera= false;

        //    int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);



        //     var inventarioAgregado = Negocios.InventarioNegocios.AgregarFoliosInhabilitados(idInventarioDetalle,idBanco, cuenta, OrdenSeleccionada, ContenedorSeleccionado, FolioInicial, FolioFinal );

        //    if (inventarioAgregado.Id > 0)
        //        bandera = true;




        //    return Json(bandera, JsonRequestBehavior.AllowGet);
        //}


        //////Metodo para Ajustar puede recibir 1, 2, o 3 parametros segun el caso
        //public JsonResult ObtenerNumerosOrden(string BancoSeleccionado, string OrdenSeleccionado, string ContenedorSeleccionado)
        //{
        //    int caso = 0;
        //    int idBanco;
        //    List<string> listaTemporal = new List<string>();

        //    if (OrdenSeleccionado == null) 
        //        caso = 1;


        //    if (BancoSeleccionado != null && OrdenSeleccionado != null && ContenedorSeleccionado == null)
        //        caso = 2;

        //    if (BancoSeleccionado != null && OrdenSeleccionado != null && ContenedorSeleccionado != null)
        //        caso = 3;

        //    idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(BancoSeleccionado);
        //    switch (caso)
        //    {
        //        case 1:
        //            //devuelve el numero de ordenes activas por el banco


        //            listaTemporal = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(idBanco);
        //            return Json(listaTemporal, JsonRequestBehavior.AllowGet);
        //            break;
        //        case 2:
        //            //devulve el numero de contenedores por el numero de orden anteriormente obtenido y filtrado en la vista

        //            var contenedoresEncontrados = Negocios.InventarioNegocios.ObtenerContenedoresActivosPorNumeroOrden(idBanco, OrdenSeleccionado);
        //            foreach (var contenedor in contenedoresEncontrados)
        //            {
        //                listaTemporal.Add(Convert.ToString(contenedor.NumContenedor));
        //            }


        //            return Json(listaTemporal, JsonRequestBehavior.AllowGet);
        //            break;
        //        case 3:
        //            //devulve los folios tanto inicial como el final de acuerdo con el contenedor filtrado

        //            var FoliosEncontrados = Negocios.InventarioNegocios.ObtenerFoliosPorContenedor(idBanco, OrdenSeleccionado, Convert.ToInt32(ContenedorSeleccionado));
        //            listaTemporal.Add(Convert.ToString( FoliosEncontrados.Id));
        //            listaTemporal.Add(Convert.ToString((Convert.ToInt32(FoliosEncontrados.FolioFinal) - FoliosEncontrados.FormasDisponiblesActuales) + 1));
        //            listaTemporal.Add(FoliosEncontrados.FolioFinal);

        //            return Json(listaTemporal, JsonRequestBehavior.AllowGet);
        //            break;

        //    }
        //    // int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(BancoSeleccionado);

        //    // List<string> listaTemporal = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(idBanco);

        //    return Json(listaTemporal, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult GuardarAjuste(int IdContenedor, string PersonalSeleccionado, string FolioInicial,string FolioFinal, int TotalFormasAsignadas)
        //{
        //    bool bandera = false;


        //    bandera =  Negocios.InventarioNegocios.AgregarNuevaAsignacion(IdContenedor, PersonalSeleccionado, FolioInicial, FolioFinal, TotalFormasAsignadas);

        //    return Json(bandera, JsonRequestBehavior.AllowGet);
        //}
        #endregion



        //Metodos para Solicitar Formas de pago para uno o mas de un banco
        public JsonResult ObtenerCuentaBancaria(string BancoSeleccionado) 
        {
          // bool bandera = false;


            string cuentaBancaria = Negocios.InventarioNegocios.ObtenerCuentaBancariaPorNombreBanco(BancoSeleccionado);
          

            return Json(cuentaBancaria, JsonRequestBehavior.AllowGet);
        }




        //Metodos para verificar disponibilidad de folios para poder saber si se puede inhabilitar o Asignar 
        public JsonResult VerificarDisponibilidadFolios(int IdInventario,string FolioInicial, string FolioFinal) 
        {
            List<String> foliosNoDisponibles = Negocios.InventarioNegocios.ValidarFoliosDisponibles(IdInventario, FolioInicial.Trim(), FolioFinal.Trim());

            return Json(foliosNoDisponibles, JsonRequestBehavior.AllowGet);
        }




        public JsonResult VerificarDisponibilidadContenedor(int IdContenedor, string FolioInicial, string FolioFinal) 
        {
            List<string> foliosNoDisponiblesContenedor = Negocios.InventarioNegocios.ValidarFoliosPorContenedor(IdContenedor, FolioInicial, FolioFinal);
           
            return Json(foliosNoDisponiblesContenedor, JsonRequestBehavior.AllowGet);
        }




        public JsonResult CrearIncidencias(int IdInventario, string FolioInicial, string FolioFinal, int IdIncidencia, string NombreEmpleado) 
        {//si el NombreEmpleado es null es por que es una inhabilitacion y no se necesita
            int IdEmpleado = 0;
            if (NombreEmpleado != null) 
            {
                IdEmpleado = Negocios.InventarioNegocios.ObtenerIdPersonal(NombreEmpleado);
            }
         
           

           List<string>foliosConProblemas = Negocios.InventarioNegocios.CrearIncidenciasFolios( IdInventario, FolioInicial, FolioFinal, IdIncidencia, IdEmpleado);

            return Json(foliosConProblemas, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CrearIncidenciasContenedor(int IdInventario, string NumeroOrden, int IdContenedor, string FolioInicial, string FolioFinal, int IdIncidencia, string NombreEmpleado)
        {//si el NombreEmpleado es null es por que es una inhabilitacion y no se necesita
            int IdEmpleado = 0;
            if (NombreEmpleado != null)
            {
                IdEmpleado = Negocios.InventarioNegocios.ObtenerIdPersonal(NombreEmpleado);
            }


            List<string> foliosConProblemas = Negocios.InventarioNegocios.CrearIncidenciasFoliosContenedor(IdInventario, NumeroOrden, IdContenedor, FolioInicial, FolioFinal, IdIncidencia, IdEmpleado);

            return Json(foliosConProblemas, JsonRequestBehavior.AllowGet);
        }





        //crear tabla del detalle banco 
        public JsonResult CargarDetalleBanco(int IdInventario) 
        {
            var detallesObtenidos = Negocios.InventarioNegocios.obtenerDetallesTabla(IdInventario);

            List<DetalleBancoModel> listaDetalle = new List<DetalleBancoModel>();

            foreach (var detalle  in detallesObtenidos) 
            {
                DetalleBancoModel nuevoDetalle = new DetalleBancoModel();

                nuevoDetalle.NumeroOrden = detalle.Tbl_InventarioContenedores.NumOrden;
                nuevoDetalle.NumeroContenedor = detalle.Tbl_InventarioContenedores.NumContenedor;
                nuevoDetalle.NumeroFolio = detalle.NumFolio;

                if (detalle.IdIncidencia != null)
                    nuevoDetalle.Incidencia = detalle.Tbl_InventarioTipoIncidencia.Descrip_Incidencia;

                if (detalle.IdEmpleado != null)
                    nuevoDetalle.NombreEmpleado = detalle.Tbl_InventarioAsignacionPersonal.NombrePersonal;

                if (detalle.FechaIncidencia != null)
                    nuevoDetalle.FechaIncidencia = detalle.FechaIncidencia.ToString();

                listaDetalle.Add(nuevoDetalle);
            }


            return Json(listaDetalle, JsonRequestBehavior.AllowGet);
        }








    }







}