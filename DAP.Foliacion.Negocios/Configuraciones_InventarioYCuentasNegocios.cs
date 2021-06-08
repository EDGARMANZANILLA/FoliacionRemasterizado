using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects.SqlClient;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO;

namespace DAP.Foliacion.Negocios
{
    public class Configuraciones_InventarioYCuentasNegocios
    {
        //Metodos para las asignaciones
        #region  Metodos para agregar un nuevo personal para la asignacion

        public static string ObtnerNombreNumEmpleado(string NumEmpleado)
        {
            return ConsultasDBSinEntity.BuscarNombreEmpleado(NumEmpleado);
        }


        public static bool GuardarNombreEmpleadoAsignaciones(string NumeroEmpleado, string NombreEmpleado, DateTime FechaHabilitacion)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var nombreDuplicado = repositorio.Obtener(x => x.NombrePersonal.ToUpper().Trim() == NombreEmpleado.ToUpper().Trim() && x.Activo == true);

            if (nombreDuplicado == null) {
                Tbl_InventarioAsignacionPersonal nuevoPersonal = new Tbl_InventarioAsignacionPersonal();
                nuevoPersonal.IdEmpleado = Convert.ToInt32(NumeroEmpleado);
                nuevoPersonal.NombrePersonal = NombreEmpleado;
                nuevoPersonal.FechaHabilitacion = FechaHabilitacion;
                nuevoPersonal.Activo = true;


                var personalAgregado = repositorio.Agregar(nuevoPersonal);

                if (personalAgregado.Id > 0)
                    bandera = true;
            }


            return bandera;
        }

        #endregion



        #region Metodos para la edicion del personal de asignacion

        public static List<Configuracion_EditarPersonalDTO> ObtenerPersonalActivo()
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var personalEncontrado = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(x => new { x.Id, x.NombrePersonal }).ToList();

            List<Configuracion_EditarPersonalDTO> personalAMostrar = new List<Configuracion_EditarPersonalDTO>();

            foreach (var persona in personalEncontrado)
            {
                Configuracion_EditarPersonalDTO nuevo = new Configuracion_EditarPersonalDTO();

                nuevo.Id = persona.Id;
                nuevo.NombreEmpleado = persona.NombrePersonal;

                personalAMostrar.Add(nuevo);
            }


            return personalAMostrar;
        }

        public static Tbl_InventarioAsignacionPersonal ObtenerPersonaActivaPorId(int Id)
        {
            bool bandera = false;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            return repositorio.Obtener(x => x.Id == Id && x.Activo == true);
        }



        public static bool GuardarEdicionNombrePersonalActivo(int Id, string NombreEditado)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            Tbl_InventarioAsignacionPersonal personalEncontrado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);


            personalEncontrado.NombrePersonal = NombreEditado;

            Tbl_InventarioAsignacionPersonal personalModificado = repositorio.Modificar(personalEncontrado);

            if (personalModificado.Id == Id)
                bandera = true;


            return bandera;
        }



        #endregion


        #region Metodos para la inhabilitacion de asignacion
        public static bool InhabilitarPersonaPorID(int Id)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            Tbl_InventarioAsignacionPersonal personalEncontrado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);


            personalEncontrado.Activo = false;

            Tbl_InventarioAsignacionPersonal personalModificado = repositorio.Modificar(personalEncontrado);

            if (personalModificado.Id == Id)
                bandera = true;


            return bandera;
        }



        #endregion



        //Metodos para las cuentas bancarias

        public static List<String> VerificarNuevasCuentasBancarias()
        {
            List<string> cuentasEnAlpha = ConsultasDBSinEntity.ObtenerNumerosCuentasDiferentesAlpha();
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var cuentasEnAppFoliacion = repositorio.ObtenerPorFiltro(x => x.Activo == true).ToList();

            List<string> cuentasDiferentes = new List<string>();

            foreach (string cuenta in cuentasEnAlpha)
            {
                var noEncontrado = repositorio.Obtener(x => x.Cuenta.Trim() == cuenta.Trim() && x.Activo == true);
                // cuentasEnAppFoliacion.Select(y => y.Cuenta.Trim() == cuenta1.Trim());

                if (noEncontrado == null)
                {
                    cuentasDiferentes.Add(cuenta);
                }
            }

            return cuentasDiferentes;
        }





        public static List<DetallesDeCuentaDTO> ObtenerNombreCuenta(string CuentaNombre)
        {
            return ConsultasDBSinEntity.ObtenerNombresCuenta(CuentaNombre);
        }




        public static bool AgregarCuentaBancariaEInventario(string NombreCuenta, string NumeroCuenta, string Abreviatura, int TipoPago, DateTime fechaActual)
        {
            bool bandera = false;

            var transaccion = new Transaccion();


            if (TipoPago == 1)
            {


                var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);

                Tbl_CuentasBancarias NuevaCuenta = new Tbl_CuentasBancarias();
                NuevaCuenta.NombreBanco = NombreCuenta.ToUpper();
                NuevaCuenta.Abreviatura = Abreviatura.ToUpper().Substring(0,5);
                NuevaCuenta.Cuenta = NumeroCuenta;
                NuevaCuenta.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                NuevaCuenta.IdInventario = null;
                NuevaCuenta.FechaCreacion = fechaActual;
                NuevaCuenta.FechaBaja = null;
                NuevaCuenta.Activo = true;

                Tbl_CuentasBancarias cuentaAgregada = repositorioCuentaBancaria.Agregar(NuevaCuenta);


                if (cuentaAgregada.Id > 0)
                    bandera = true;
            }




            if (TipoPago > 1) {
                var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);

                Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                nuevoInventario.FormasDisponibles = 0;
                nuevoInventario.UltimoFolioInventario = null;
                nuevoInventario.Activo = true;

                Tbl_Inventario inventarioAgregado = repositorioInventario.Agregar(nuevoInventario);





                var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);

                Tbl_CuentasBancarias NuevaCuenta = new Tbl_CuentasBancarias();
                NuevaCuenta.NombreBanco = NombreCuenta.ToUpper();
                NuevaCuenta.Abreviatura = Abreviatura.ToUpper().Substring(0, 5);
                NuevaCuenta.Cuenta = NumeroCuenta;
                NuevaCuenta.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                NuevaCuenta.IdInventario = inventarioAgregado.Id;
                NuevaCuenta.FechaCreacion = fechaActual;
                NuevaCuenta.FechaBaja = null;
                nuevoInventario.Activo = true;

                Tbl_CuentasBancarias cuentaAgregada = repositorioCuentaBancaria.Agregar(NuevaCuenta);


                if (cuentaAgregada.Id > 0)
                    bandera = true;
            }







            return bandera;
        }



        public static List<string> ObtenerCuentasBancariasActivas()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            return repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => y.Cuenta).ToList();
        }


        public static Tbl_CuentasBancarias ObtenerDetallesCuentaBancaria(string NumeroCuenta)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            return repositorio.Obtener(x => x.Cuenta.Trim() == NumeroCuenta.Trim() && x.Activo == true);
        }


        public static bool EditarCuentaBancariaActiva(int IdCuenta, string NumeroCuenta, string NombreCuenta, string Abreviatura, int TipoPago)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancoEncontrado = repositorio.Obtener(x => x.Id == IdCuenta && x.Cuenta.Trim() == NumeroCuenta.Trim() && x.Activo == true);
            bool bandera = false;

         

            //depende del tipo de pago que pase como parametro el usuario

            //si el pago es tarjeta 
            if (TipoPago == 1) 
            {
                //y el banco encontrado no ah tenido inventario solo se modifica los nombres 
                if (bancoEncontrado.IdInventario == null)
                {
                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;

                    var entidadAgregada = repositorio.Modificar(bancoEncontrado);
                    if (entidadAgregada.Id > 0)
                        bandera = true;

                    return bandera;
                }

                //y el banco ya tuvo inventario se inactiva su inventario 
                if (bancoEncontrado.IdInventario != null)
                {
                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;

                    var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
                    Tbl_Inventario inventarioEncontrado = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario && x.Activo == true);

                    inventarioEncontrado.Activo = false;

                    var entidadAgregada = repositorioInventario.Modificar(inventarioEncontrado);
                    var entidadAgregada2 = repositorio.Modificar(bancoEncontrado);


                    if (entidadAgregada.Id > 0 && entidadAgregada2.Id > 0)
                        bandera = true;

                    return bandera;
                }



            }

            //si el tipo de pago es cheque o para tarjeta y cheque (ambos)
            if (TipoPago > 1)
            {
                //y nunca a tenido un inventario, se crea su inventario y se modifica y guarda el resgistro del banco encontrado
                if (bancoEncontrado.IdInventario == null)
                {
                    var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);

                    Tbl_Inventario nuevoInventario = new Tbl_Inventario();
                    nuevoInventario.FormasDisponibles = 0;
                    nuevoInventario.UltimoFolioInventario = null;
                    nuevoInventario.Activo = true;

                    var registroInventarioAgregado = repositorioInventario.Agregar(nuevoInventario);

                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                    bancoEncontrado.IdInventario = registroInventarioAgregado.Id;

                    var entidadAgregada = repositorio.Modificar(bancoEncontrado);

                    if (entidadAgregada.Id > 0)
                        bandera = true;

                    return bandera;
                }



                //y si ya tuvo inventario pero se deshabilito y se quiere habilitar despues
                if (bancoEncontrado.IdInventario != null)
                {
                    bancoEncontrado.NombreBanco = NombreCuenta;
                    bancoEncontrado.Abreviatura = Abreviatura;
                    //

                    //si ya tuvo inventario y ahora solo quiere que se pague con cheques  se modifica y se limpia su inventario para su uso futuro
                    if (TipoPago == 2)
                    {
                        //y si proviene en especifico de un pago tanto con tarjetas como cheques en cuenta bancarias se conservan sus registros del inventario por que seguiera usando las formas de pago actuales 
                        if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == 3) 
                        {
                            bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                            var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                            if (entidadAgregadaCuentaBancaria.Id > 0)
                                bandera = true;

                            return bandera;
                        }

                        if (bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta == TipoPago)
                        {
                            //bancoEncontrado.IdCuentaBancaria_TipoPagoCuenta = TipoPago;
                            var entidadAgregadaCuentaBancaria = repositorio.Modificar(bancoEncontrado);
                            if (entidadAgregadaCuentaBancaria.Id > 0)
                                bandera = true;

                            return bandera;
                        }


                        var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
                        var InventarioModificar  = repositorioInventario.Obtener(x => x.Id == bancoEncontrado.IdInventario);

                        InventarioModificar.FormasDisponibles = 0;
                        InventarioModificar.UltimoFolioInventario = null;
                        InventarioModificar.UltimoFolioUtilizado = null;
                        InventarioModificar.EstimadoMeses = null;
                        InventarioModificar.Activo = true;

                        var registroInventarioAgregado = repositorioInventario.Modificar(InventarioModificar);

                    }

                    var entidadAgregada =  repositorio.Modificar(bancoEncontrado);
                    if (entidadAgregada.Id > 0)
                        bandera = true;

                    return bandera;
                }


            }



            return bandera;
        }



        




    }
}
