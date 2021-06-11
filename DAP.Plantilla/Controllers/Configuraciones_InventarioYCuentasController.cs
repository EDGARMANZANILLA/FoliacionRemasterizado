using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Entidades.DTO;
using DAP.Plantilla.ObjetosExtras;
using DAP.Plantilla.Models;


namespace DAP.Plantilla.Controllers
{
    public class Configuraciones_InventarioYCuentasController : Controller
    {
        // GET: Configuraciones_InventarioYCuentas
        public ActionResult Index()
        {
            return View();
        }


        #region  Render de vistas parciales para Asignacion
        public ActionResult Inventario_AgregarPersonal()
        {
            return PartialView();
        }

        public ActionResult Inventario_MostrarEdicionPersonal()
        {
            List<Configuracion_EditarPersonalDTO> personalEncontrado = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerPersonalActivo();

            return PartialView(personalEncontrado);
        }

        public ActionResult Inventario_InhabilitarPersonal()
        {
            List<Configuracion_EditarPersonalDTO> personalEncontrado = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerPersonalActivo();

            return PartialView(personalEncontrado);
        }
        #endregion



        #region Render de vistas parciales para cuentas bancarias

        public ActionResult CuentaBancaria_VerificarAgregar()
        {
            List<string> cuentasDiferentes = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.VerificarNuevasCuentasBancarias();

            return PartialView(cuentasDiferentes);
        }

        public ActionResult CuentaBancaria_MostrarEdicionBanco()
        {
            List<string> cuentasEncontradas = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerCuentasBancariasActivas();

            return PartialView(cuentasEncontradas);
        }

        public ActionResult CuentaBancaria_EliminacionCuenta()
        {
            List<string> cuentasEncontradas = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerCuentasBancariasActivas();

            return PartialView(cuentasEncontradas);
        }
        #endregion




        #region  Render de vistas parciales Formas de pago exepcionales

        public ActionResult FormasPagoExcepcionales()
        {
            List<string> cuentasEncontradas = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerCuentasBancariasConPagoTarjetaEInventario();

            return PartialView(cuentasEncontradas);
        }

        public ActionResult FormasPagoExcepcionales_Asignar(string NumeroCuenta)
        {
            var cuentaEncontrada = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerDetallesCuentaConPagoTarjetaEInventario(NumeroCuenta);

            ViewBag.NombreBanco = cuentaEncontrada.NombreBanco.Trim().ToUpper();
            ViewBag.CuentaBanco = cuentaEncontrada.Cuenta.Trim().ToUpper();
            ViewBag.IdInventario = cuentaEncontrada.IdInventario;



            ViewBag.OrdenesEncontradas = Foliacion.Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(cuentaEncontrada.IdInventario.GetValueOrDefault());


            ViewBag.ListaNombrePersonal = Foliacion.Negocios.InventarioNegocios.ObtenerPersonalActivo();

            return PartialView();
        }

        public ActionResult FormasPagoExcepcionales_Inhabilitar(string NumeroCuenta)
        {
            var cuentaEncontrada = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerDetallesCuentaConPagoTarjetaEInventario(NumeroCuenta);

            ViewBag.NombreBanco = cuentaEncontrada.NombreBanco.Trim().ToUpper();
            ViewBag.CuentaBanco = cuentaEncontrada.Cuenta.Trim().ToUpper();
            ViewBag.IdInventario = cuentaEncontrada.IdInventario;
            




            return PartialView();
        }
        #endregion







        [HttpPost]

        #region Metodos para asignacion del inventario
        public ActionResult EncontrarNumeroEmpleado(string NumEmpleado)
        {
            string nombreEncontrado = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtnerNombreNumEmpleado(NumEmpleado);



            return Json(nombreEncontrado, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GuardarNuevoNombreEmpleadoAsignaciones(string NumeroEmpleado, string NombreEmpleado)
        {


            bool bandera = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.GuardarNombreEmpleadoAsignaciones(NumeroEmpleado, NombreEmpleado, ObjetosExtras.ObtenerHoraReal.ObtenerDateTimeFechaReal());

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerNombreEdicion(int Id) 
        {
            var nombreAEditar = DAP.Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerPersonaActivaPorId(Id); 

            return Json(nombreAEditar.NombrePersonal.Trim(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditarNombrePersonal(int Id, string NombreEditado)
        {
            bool bandera = true;

           bandera =  Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.GuardarEdicionNombrePersonalActivo(Id, NombreEditado);



            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult InhabilitarPersonalPorID(int Id)
        {
            bool bandera = true;

            bandera = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.InhabilitarPersonaPorID(Id);



            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        #endregion




        #region Metodos para cuentas bancarias
        //Metodos para cuenta bancaria
        public ActionResult ObtenerNombreCuentaBancaria(string CuentaNombre)
        {
           var detalleCuentaEncontrado = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerNombreCuenta(CuentaNombre);

            return Json(detalleCuentaEncontrado, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AgregarCuentaBancaria(string NombreCuenta, string NumeroCuenta, string Abreviatura,int  TipoPago)
        {
            bool bandera = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.AgregarCuentaBancariaEInventario( NombreCuenta, NumeroCuenta, Abreviatura, TipoPago, ObjetosExtras.ObtenerHoraReal.ObtenerDateTimeFechaReal());

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerInfoCuentaBancaria(string NumeroCuenta)
        {
            var detalleCuentaObtenida = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.ObtenerDetallesCuentaBancaria(NumeroCuenta);

            CuentaBancariaModel detallesCuenta = new CuentaBancariaModel();
            detallesCuenta.Id = detalleCuentaObtenida.Id;
            detallesCuenta.NombreBanco = detalleCuentaObtenida.NombreBanco;
            detallesCuenta.Abreviatura = detalleCuentaObtenida.Abreviatura;
            detallesCuenta.Cuenta = detalleCuentaObtenida.Cuenta;
            detallesCuenta.IdCuentaBancaria_TipoPagoCuenta = detalleCuentaObtenida.IdCuentaBancaria_TipoPagoCuenta;
            detallesCuenta.IdInventario = detalleCuentaObtenida.IdInventario;

           

            return Json(detallesCuenta, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditarCuentaBancaria(int IdCuenta, string NumeroCuenta, string NombreCuenta, string Abreviatura, int TipoPago)
        {
           bool bandera = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.EditarCuentaBancariaActiva(IdCuenta, NumeroCuenta, NombreCuenta, Abreviatura, TipoPago);


            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EliminarCuentaBancaria(string NumeroCuenta)
        {
            bool bandera = Foliacion.Negocios.Configuraciones_InventarioYCuentasNegocios.EliminarCuentaBancariaActiva(NumeroCuenta, ObjetosExtras.ObtenerHoraReal.ObtenerDateTimeFechaReal());




            return Json(bandera, JsonRequestBehavior.AllowGet);
        }

        #endregion



        #region Metodos para formas de pago exepcionales 


        public ActionResult ContinuarCuentaBancaria(string NombreCuenta)
        {
            bool bandera = true;



            return Json(bandera, JsonRequestBehavior.AllowGet);
        }

        #endregion




    }
}