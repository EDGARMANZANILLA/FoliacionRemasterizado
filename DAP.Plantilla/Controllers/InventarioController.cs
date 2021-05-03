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
                NuevoBanco.UltimoFolioUtilizado = inventarioBanco.UltimoFolioUtilizado;
                NuevoBanco.FormasUsadasQuincena1 = inventarioBanco.FormasUsadasQuincena1;
                NuevoBanco.FormasUsadasQuincena2 = inventarioBanco.FormasUsadasQuincena2;
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



        public ActionResult Incidencias(string NombreBanco) 
        {
            ViewBag.NombreBancoSeleccionado = NombreBanco;

            return View();
        }


        public ActionResult DetalleBanco(string NombreBanco)
        {

            ViewBag.NombreBancoSeleccionado = NombreBanco;

            List<InventarioContenedorModel> motrarContenedores = new List<InventarioContenedorModel>();

            int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);
            int idInventario = Negocios.InventarioNegocios.ObtenerIdInventario(idBanco);
            var inventarioDetallesObtenidos = Negocios.InventarioNegocios.ObtenerContenedoresBanco(idInventario);

            foreach (var detalle in inventarioDetallesObtenidos)
            {
                InventarioContenedorModel nuevoDetalle = new InventarioContenedorModel();

                nuevoDetalle.Banco = detalle.Tbl_Inventario.Tbl_CuentasBancarias.NombreBanco;
                nuevoDetalle.Cuenta = detalle.Tbl_Inventario.Tbl_CuentasBancarias.Cuenta;
                nuevoDetalle.NumeroOrden = detalle.NumOrden;
                nuevoDetalle.NumeroContenedor = detalle.NumContenedor;
                nuevoDetalle.FolioInicial = detalle.FolioInicial;
                nuevoDetalle.FolioFinal = detalle.FolioFinal;
                nuevoDetalle.TotalFormasContenedor = detalle.FormasTotalesContenedor;
                nuevoDetalle.FormasDisponiblesActuales = detalle.FormasDisponiblesActuales;
                nuevoDetalle.FormasInhabilitadas = detalle.FormasInhabilitadas;
                nuevoDetalle.FormasAsignadas = detalle.FormasAsignadas;
                nuevoDetalle.FechaAlta = detalle.FechaAlta;

                motrarContenedores.Add(nuevoDetalle);
            }




            return View(motrarContenedores);
        }





        public ActionResult Ajustar(string NombreBanco)
        {

            ViewBag.NombreBanco = NombreBanco;


            int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);

            string numeroCuenta = Negocios.InventarioNegocios.ObtenerCuentaBancariaIdBanco(idBanco);

            int idInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(idBanco);

            ViewBag.NumeroCuentaBanco = numeroCuenta;
            ViewBag.IdInventario = idInventario;






            //pasar nombre de bancos activos
            //List<string> nombresBancos = new List<string>();

            //var bancosActivos = Negocios.InventarioNegocios.ObtenerBancosConChequera();

            //foreach (var banco in bancosActivos)
            //{

            //    nombresBancos.Add(banco.NombreBanco);
            //}


            //ViewBag.ListaNombreBancos = nombresBancos;


            //List<string> nombrePersonal = new List<string>();
            ViewBag.ListaNombrePersonal = Negocios.InventarioNegocios.ObtenerPersonalActivo();

            ////Datos para mostrar en la tabla
            //var inventarioAsignadoEncontrado = Negocios.InventarioNegocios.ObtenerInventarioAsignado();
            //List<InventarioAsignarAjustarModel> MostrarAsignaciones = new List<InventarioAsignarAjustarModel>();
            //foreach (var inventario in inventarioAsignadoEncontrado)
            //{
            //    InventarioAsignarAjustarModel nuevoInventario = new InventarioAsignarAjustarModel();
            //    nuevoInventario.NombrePersona = inventario.Tbl_Inventario_AsignacionPersonal.NombrePersonal;
            //    nuevoInventario.NombreBanco = inventario.Tbl_Inventario_Detalle.Tbl_Inventario.Tbl_CuentasBancarias.NombreBanco; ;
            //    nuevoInventario.Cuenta = inventario.Tbl_Inventario_Detalle.Tbl_Inventario.Tbl_CuentasBancarias.Cuenta;
            //    nuevoInventario.NumeroOrden = inventario.Tbl_Inventario_Detalle.NumOrden;
            //    nuevoInventario.Contenedor = inventario.Tbl_Inventario_Detalle.NumContenedor;
            //    nuevoInventario.FoliosAsignados = inventario.FoliosAsignados;
            //    nuevoInventario.FolioInicial = inventario.FolioInicial;
            //    nuevoInventario.FolioFinal = inventario.FolioFinal;
            //    nuevoInventario.FechaAsignacion = inventario.FechaAsignacion;

            //    MostrarAsignaciones.Add(nuevoInventario);
            //}




            return View(/*MostrarAsignaciones*/);
        }


        public ActionResult Solicitar()
        {
            //pasar nombre de bancos activos
            List<string> nombresBancos = new List<string>();

            var bancosActivos = Negocios.InventarioNegocios.ObtenerBancosConChequera();

            foreach (var banco in bancosActivos)
            {

                nombresBancos.Add(banco.NombreBanco);
            }


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


            int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(NombreBanco);

            string numeroCuenta = Negocios.InventarioNegocios.ObtenerCuentaBancariaIdBanco(idBanco);

            int idInventario = Negocios.InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(idBanco);

            ViewBag.NumeroCuentaBanco = numeroCuenta;
            ViewBag.IdInventario = idInventario;

            //List<string> ordenesEncontradas = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(idBanco);

            //ViewBag.OrdenesEncontradas = ordenesEncontradas;



            //var InhabilitadosObtenidos = Negocios.InventarioNegocios.ObtenerInhabilitadosBanco(idBanco);

            //List<InventarioInhabilitadoModel> inventarioInhabilitado = new List<InventarioInhabilitadoModel>();

            //foreach (var obtenido in InhabilitadosObtenidos)
            //{
            //    InventarioInhabilitadoModel NuevoContenedor = new InventarioInhabilitadoModel();

            //    NuevoContenedor.banco = obtenido.Tbl_Inventario_Detalle.Tbl_Inventario.Tbl_CuentasBancarias.NombreBanco;
            //    NuevoContenedor.Cuenta = obtenido.Tbl_Inventario_Detalle.Tbl_Inventario.Tbl_CuentasBancarias.Cuenta;
            //    NuevoContenedor.Orden = obtenido.Tbl_Inventario_Detalle.NumOrden;
            //    NuevoContenedor.Contenedor = obtenido.Tbl_Inventario_Detalle.NumContenedor;
            //    NuevoContenedor.FolioInicial = obtenido.FolioInicial;
            //    NuevoContenedor.FolioFinal = obtenido.FolioFinal;
            //    NuevoContenedor.TotalFormas = obtenido.TotalFormas;
            //    NuevoContenedor.FechaDetalle = obtenido.FechaDetalle;

            //    inventarioInhabilitado.Add(NuevoContenedor);
            //}


            return View(/*inventarioInhabilitado*/);
        }


        //metodos por post

        [HttpPost]

        //Metodo Para Agregar contenedores 
        public JsonResult GuardarInventarioAgregado(List<AgregarInventarioModel> listaDeContenedores,string NumOrden, string banco)
        {
            bool bandera = false;

           // var contenedores = listaDeContenedores;
            try
            {
                int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

                int idInventario = Negocios.InventarioNegocios.ObtenerIdInventario(idBanco);

                foreach (AgregarInventarioModel nuevoContenedor in listaDeContenedores) 
                {
                    bandera = Negocios.InventarioNegocios.GuardarInventarioContenedores(idInventario, idBanco, NumOrden, nuevoContenedor.IteradorDeContenedores, nuevoContenedor.FInicial, nuevoContenedor.FFinal, nuevoContenedor.TotalFormas);
                }


              }
            catch (Exception e)
            {
                bandera = false;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
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



        //public JsonResult ObtenerFoliosDelContenedor( string banco, string OrdenSeleccionada, int ContenedorSeleccionado)
        //{


        //    List<string> NumeroDeContenedores = new List<string>();

        //    int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

        //    var foliosDeContenedorEncontrado =  InventarioNegocios.ObtenerFoliosPorContenedor(idBanco, OrdenSeleccionada, ContenedorSeleccionado);

        //    NumeroDeContenedores.Add(Convert.ToString( foliosDeContenedorEncontrado.Id));
        //    NumeroDeContenedores.Add(Convert.ToString( (Convert.ToInt32( foliosDeContenedorEncontrado.FolioFinal) - foliosDeContenedorEncontrado.FormasDisponiblesActuales)+1 ));
        //    NumeroDeContenedores.Add(foliosDeContenedorEncontrado.FolioFinal);

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
           bool bandera = false;
           int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(BancoSeleccionado);
           string cuentaBancaria = Negocios.InventarioNegocios.ObtenerCuentaBancariaIdBanco(idBanco);

            if(cuentaBancaria != "")
                return Json(cuentaBancaria, JsonRequestBehavior.AllowGet);


            return Json(bandera, JsonRequestBehavior.AllowGet);
        }




        //Metodos para verificar disponibilidad de folios para poder saber si se puede inhabilitar o Asignar 
        public JsonResult VerificarDisponibilidadFolios(int IdInventario,string FolioInicial, string FolioFinal) 
        {
            List<String> foliosNoDisponibles = Negocios.InventarioNegocios.ValidarFoliosDisponibles(IdInventario,Convert.ToInt32(FolioInicial), Convert.ToInt32(FolioFinal));



            return Json(foliosNoDisponibles, JsonRequestBehavior.AllowGet);
        }


    }
}