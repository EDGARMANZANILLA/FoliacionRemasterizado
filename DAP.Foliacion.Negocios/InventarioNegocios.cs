using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Datos;
using System.Data.Objects.SqlClient;
using DAP.Foliacion.Entidades.DTO;

namespace DAP.Foliacion.Negocios
{
    public class InventarioNegocios
    {
        //revisado
        public static IEnumerable<Tbl_CuentasBancarias> ObtenerInventarioActivo()
        {

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var InventariosActivos = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.Tbl_Inventario.Activo == true);


            return InventariosActivos;
        }

        public static int ObtenerIdBanco(string banco)
        {
            int idBanco = 0;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var BancosActivos = repositorio.Obtener(x => x.NombreBanco.Trim() == banco.Trim() && x.Activo == true);

            if (BancosActivos != null)
            {
                idBanco = BancosActivos.Id;
            }


            return idBanco;
        }

        //revisado
        public static int ObtenerIdInventarioPorNombreBanco(string banco)
        {

            int idInventario = 0;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var InventarioActivos = repositorio.Obtener(x => x.NombreBanco.Trim() == banco.Trim() && x.Activo == true);

            if (InventarioActivos != null)
            {
                idInventario = Convert.ToInt32(InventarioActivos.IdInventario);
            }


            return idInventario;
        }

        //revisado
        #region Metodos para agregar contenedores y actualizar inventario general
        public static bool GuardarInventarioContenedores(int idInventario, string numeroOrden, int numeroContenedor, string FInicial, string FFinal, int TotalFormas)
        {

            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);


            var inventarioFiltrado = repositorio.ObtenerPorFiltro(x => x.Id == idInventario && x.Activo == true);



            try
            {
                //Actualiza datos en la tabla inventario
                foreach (Tbl_Inventario inventarioObtenido in inventarioFiltrado)
                {
                    inventarioObtenido.FormasDisponibles += TotalFormas;
                    inventarioObtenido.UltimoFolioInventario = FFinal;

                    if (inventarioObtenido.FormasUsadasQuincena1 > 0 && inventarioObtenido.FormasUsadasQuincena2 > 0)
                    {
                        inventarioObtenido.EstimadoMeses = inventarioObtenido.FormasDisponibles / (inventarioObtenido.FormasUsadasQuincena1 + inventarioObtenido.FormasUsadasQuincena2);
                    }

                }



                //  agrega contenedores con folios para que coincida con el inventario
                Tbl_InventarioContenedores nuevoContenedor = new Tbl_InventarioContenedores();

                nuevoContenedor.IdInventario = idInventario;
                nuevoContenedor.NumOrden = numeroOrden;
                nuevoContenedor.NumContenedor = numeroContenedor;
                nuevoContenedor.FolioInicial = FInicial;
                nuevoContenedor.FolioFinal = FFinal;
                nuevoContenedor.FormasTotalesContenedor = TotalFormas;
                nuevoContenedor.FormasDisponiblesActuales = TotalFormas;
                nuevoContenedor.FormasInhabilitadas = 0;
                nuevoContenedor.FormasAsignadas = 0;
                nuevoContenedor.FechaAlta = DateTime.Now.Date;
                nuevoContenedor.Activo = true;

                var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);

                var contenedorAgregado = repositorioContenedor.Agregar(nuevoContenedor);




                //Agrega el detalle de cada contenedor para saber cada uno de sus folios 
                var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

                for (int i = Convert.ToInt32(FInicial); i <= Convert.ToInt32(FFinal); i++)
                {
                    Tbl_InventarioDetalle nuevoDetalle = new Tbl_InventarioDetalle();

                    nuevoDetalle.IdContenedor = contenedorAgregado.Id;
                    nuevoDetalle.NumFolio = Convert.ToString(i);
                    nuevoDetalle.Activo = true;

                    repositorioDetalle.Agregar(nuevoDetalle);
                }
                // transaccion.Dispose();
                //se transacciona por si algo va mal se pueda hacer un roll over de todo
                transaccion.GuardarCambios();
                bandera = true;

            }
            catch (Exception e)
            {
                transaccion.Dispose();
                bandera = false;
            }






            return bandera;

        }

        #endregion




        #region para la vista DetalleBanco Metodos para obtener contenedores por banco
        public static IEnumerable<Tbl_InventarioContenedores> ObtenerContenedoresBanco(int idBanco)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            return repositorio.ObtenerPorFiltro(x => x.IdInventario == idBanco && x.FormasDisponiblesActuales > 0 && x.Activo == true);
        }

        #endregion

        // pendiente de revision en solicitud
        public static IEnumerable<Tbl_CuentasBancarias> ObtenerBancosConChequera()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var BancosConChequeraActivos = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);


            return BancosConChequeraActivos;
        }


        #region metodos para inhabilitar y solicitar formas de pago
        //revisado
        public static string ObtenerCuentaBancariaPorNombreBanco(string Nombrebanco)
        {

            var transaccion = new Transaccion();


            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var NumeroDeCuenta = repositorio.Obtener(x => x.NombreBanco.Trim() == Nombrebanco.Trim() && x.Activo == true);

            string cuenta = NumeroDeCuenta.Cuenta;
            return cuenta;
        }

        //regresa el id de un registro del inventario filtrado por IdCuentaBancaria
        public static int ObtenerIdInventarioPorIdCuentaBancaria(int Idbanco)
        {
            /////////////////////////verificar5
            var transaccion = new Transaccion();


            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);
            var inventarioObtenido = repositorio.Obtener(x => x.Id == Idbanco && x.Activo == true);

            return inventarioObtenido.Id;
        }





        public static List<InventarioContenedoresDTO> ObtenerInfoContendoresPorBanco(string NombreBanco)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias cuentaEncontrada = repositorio.Obtener(x => x.NombreBanco.Trim() == NombreBanco.Trim());

            int IdInventario = Convert.ToInt32(cuentaEncontrada.IdInventario);

            var repositorio2 = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio2.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo == true);



            List<InventarioContenedoresDTO> listaContenedores = new List<InventarioContenedoresDTO>();
            foreach (var contenedor in contenedoresEncontrados)
            {
                InventarioContenedoresDTO nuevoInhabilitado = new InventarioContenedoresDTO();

                nuevoInhabilitado.Banco = cuentaEncontrada.NombreBanco;
                nuevoInhabilitado.Cuenta = cuentaEncontrada.Cuenta;
                nuevoInhabilitado.Orden = contenedor.NumOrden;
                nuevoInhabilitado.Contenedor = contenedor.NumContenedor;
                nuevoInhabilitado.FolioInicial = contenedor.FolioInicial;
                nuevoInhabilitado.FolioFinal = contenedor.FolioFinal;
                nuevoInhabilitado.FormasTotalesContenedor = contenedor.FormasTotalesContenedor;
                nuevoInhabilitado.FormasDisponiblesActuales = contenedor.FormasDisponiblesActuales;
                nuevoInhabilitado.FormasInhabilitadas = contenedor.FormasInhabilitadas;
                nuevoInhabilitado.FormasAsignadas = contenedor.FormasAsignadas;
                nuevoInhabilitado.FechaAlta = contenedor.FechaAlta.ToString("dd/MM/yyyy");


                listaContenedores.Add(nuevoInhabilitado);

            }



            return listaContenedores;
        }


        /// <summary>
        /// Metodo que devuelve los numeros de ordenes activas por el banco filtrado como parametro el IdInventario funciona para Inhabilitar como para Ajustar
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <returns></returns>
        /// Verisado
        public static List<string> ObtenerNumeroOrdenesBancoActivo(int IdInventario)
        {
            string contenedoresEncontrados = "";

            List<string> numerosOrden = new List<string>();

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            var numerosOrdenEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.Activo == true);

            foreach (var orden in numerosOrdenEncontrados)
            {
                if (!numerosOrden.Contains(orden.NumOrden))
                {
                    contenedoresEncontrados = orden.NumOrden;

                    numerosOrden.Add(contenedoresEncontrados);
                }
            }
            return numerosOrden;
        }


        /// <summary>
        /// Devuelve los contenedores que contiene cada Numero de orden existente donde haya formas actuales y este activo; Funciona para Inhabilitar como para Ajustar
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <param name="OrdenSelecionada"></param>
        /// <returns></returns>
        public static IEnumerable<Tbl_InventarioContenedores> ObtenerContenedoresActivosPorIdInventario(int IdInventario, string OrdenSelecionada)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.NumOrden == OrdenSelecionada && x.FormasDisponiblesActuales > 0 && x.Activo == true);


            return contenedoresEncontrados;
        }


        /// <summary>
        /// Decuelve la entidad que contiene los numeros de folio inicial y final del contenedor y la orden seleccionada
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <param name="OrdenSeleccionada"></param>
        /// <param name="ContenedorSeleccionado"></param>
        /// <returns></returns>
        public static Tbl_InventarioContenedores ObtenerFoliosPorContenedor(int IdInventario, string OrdenSeleccionada, int ContenedorSeleccionado)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            return repositorio.Obtener(x => x.IdInventario == IdInventario && x.NumOrden == OrdenSeleccionada && x.NumContenedor == ContenedorSeleccionado && x.FormasDisponiblesActuales > 0 && x.Activo == true);
        }


        #endregion


        //Valida folios para saber si estan disponibles
        public static List<string> ValidarFoliosDisponibles(int IdInventario, string FolioInicial, string FolioFinal)
        {
            var transaccion = new Transaccion();



            //obtiene el contenedor donde se encuentra el rango de folio inicial y el folio final
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo);
            List<int> idsContenedoresEncontrados = new List<int>();

            foreach (Tbl_InventarioContenedores contenedor in contenedoresEncontrados)
            {
                idsContenedoresEncontrados.Add(contenedor.Id);
            }

            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            List<String> ListafoliosNoDisponiblesp = new List<string>();


            Tbl_InventarioDetalle folioInicialEncontrado = null;
            Tbl_InventarioDetalle folioFinalEncontrado = null;
            foreach (int contenedor in idsContenedoresEncontrados)
            {
                if (folioInicialEncontrado == null)
                    folioInicialEncontrado = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FolioInicial && x.Activo == true);

                if (folioFinalEncontrado == null)
                    folioFinalEncontrado = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FolioFinal && x.Activo == true);

            }


            //obtiene los registros que se tienen por el id para reducir el universo de folios
            if (folioInicialEncontrado != null && folioFinalEncontrado != null)
            {
                var repositorio3p = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                var foliosEntontradosEnContenedores = repositorio3p.ObtenerPorFiltro(x => x.IdContenedor >= folioInicialEncontrado.IdContenedor && x.IdContenedor <= folioFinalEncontrado.IdContenedor && x.Id >= folioInicialEncontrado.Id && x.Id <= folioFinalEncontrado.Id);

                //var resultadopp = foliosEntontradosEnContenedores.FirstOrDefault().Id;
                int resultado = foliosEntontradosEnContenedores.Count();
                if (foliosEntontradosEnContenedores.Count() > 0)
                {
                    // var nuevos =foliosEntontradosEnContenedores.Where(x => x.Id >= folioInicialEncontrado.Id && x.Id <= folioFinalEncontrado.Id);
                    // int resultado2 = nuevos.Count();
                    //Referencia para saber que contiene la lista que vera el usuario
                    ListafoliosNoDisponiblesp.Add("Folios No Disponibles");
                    foreach (Tbl_InventarioDetalle detalle in foliosEntontradosEnContenedores)
                    {
                        if (detalle.IdIncidencia != null)
                        {
                            ListafoliosNoDisponiblesp.Add(Convert.ToString(detalle.NumFolio));
                        }
                    }

                    return ListafoliosNoDisponiblesp;
                }
            }


            if (ListafoliosNoDisponiblesp.Count == 0)
            {
                ListafoliosNoDisponiblesp.Add("No Existe El Folio");
                if (folioInicialEncontrado == null)
                    ListafoliosNoDisponiblesp.Add(FolioInicial);

                if (folioFinalEncontrado == null)
                    ListafoliosNoDisponiblesp.Add(FolioFinal);
            }



            return ListafoliosNoDisponiblesp;
        }


        /// <summary>
        /// Devuelve una lista de los folios que ya fueron ocupados dentro de un contenedor en especifico para que no se puedan usar
        /// </summary>
        /// <param name="IdContenedor"></param>
        /// <param name="FolioInicial"></param>
        /// <param name="FolioFinal"></param>
        /// <returns></returns>
        public static List<string> ValidarFoliosPorContenedor(int IdContenedor, string FolioInicial, string FolioFinal)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);

            int folioInicialEncontrado = repositorio.Obtener(x => x.IdContenedor == IdContenedor && x.NumFolio == FolioInicial && x.Activo == true).Id;
            int folioFinalEncontrado = repositorio.Obtener(x => x.IdContenedor == IdContenedor && x.NumFolio == FolioFinal && x.Activo == true).Id;


            var foliosEntontradosEnContenedores = repositorio.ObtenerPorFiltro(x => x.IdContenedor == IdContenedor && x.Id >= folioInicialEncontrado && x.Id <= folioFinalEncontrado);

            List<string> ListafoliosNoDisponiblesContenedor = new List<string>();

            ListafoliosNoDisponiblesContenedor.Add("Folios No Disponibles");
            foreach (Tbl_InventarioDetalle nuevoDetalle in foliosEntontradosEnContenedores)
            {
                if (nuevoDetalle.IdIncidencia != null)
                {
                    ListafoliosNoDisponiblesContenedor.Add(Convert.ToString(nuevoDetalle.NumFolio));
                }

            }


            return ListafoliosNoDisponiblesContenedor;
        }



        //Metodos para asignar e inhabilitar
        //revisado
        public static List<String> ObtenerPersonalActivo()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var personalEncontrado = repositorio.ObtenerPorFiltro(x => x.Activo == true);

            List<String> nombrePersonalEncontrado = new List<string>();

            foreach (Tbl_InventarioAsignacionPersonal personal in personalEncontrado)
            {
                string nuevonombre = personal.NombrePersonal;

                nombrePersonalEncontrado.Add(nuevonombre);
            }

            return nombrePersonalEncontrado;
        }


        /// <summary>
        /// Obtiene el Id de un nombre registrado, que se pasa como parametro
        /// </summary>
        /// <returns>Int</returns>
        public static int ObtenerIdPersonal(string NombrePersonal)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);

            var personalEncontrado = repositorio.Obtener(x => x.NombrePersonal.Trim() == NombrePersonal.Trim() && x.Activo == true); ;

            return personalEncontrado.Id;
        }




        //metodo para crear una incidencia
        public static List<string> CrearIncidenciasFolios(int IdInventario, string FolioInicial, string FolioFinal, int IdIncidencia, int IdEmpleado)
        {
            var transaccion = new Transaccion();


            //obtiene el contenedor donde se encuentra el rango de folio inicial y el folio final
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo);

            //Obtengo cada una de los contenedores encontrado y se guarda el Id del contenedor 
            List<int> listaContenedoresEncontrados = new List<int>();
            foreach (Tbl_InventarioContenedores contenedor in contenedoresEncontrados)
            {
                listaContenedoresEncontrados.Add(contenedor.Id);
            }


            //se crea el repo de inventaio detalle para ir buscando por el id del contenedor 
            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            List<String> ListafoliosNoDisponibles = new List<string>();

            //se crean variables de Tbl_InventarioDetalle para guardar en donde se encuentran los folios buscados 
            Tbl_InventarioDetalle folioInicialEncontrado = null;
            Tbl_InventarioDetalle folioFinalEncontrado = null;
            foreach (int contenedor in listaContenedoresEncontrados)
            {
                if (folioInicialEncontrado == null)
                    folioInicialEncontrado = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FolioInicial && x.Activo == true);

                if (folioFinalEncontrado == null)
                    folioFinalEncontrado = repositorioDetalle.Obtener(x => x.IdContenedor == contenedor && x.NumFolio == FolioFinal && x.Activo == true);

            }



            //obtiene los registros que se tienen por el id para reducir el universo de folios
            if (folioInicialEncontrado != null && folioFinalEncontrado != null)
            {
                // var repositorio3 = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                //  var transaccionModificar = new Transaccion();
                // var repositorioModificarDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                //se utiliza el mismo repositorio creado de Tbl_InventarioDetalle
                var foliosEntontradosEnContenedores = repositorioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= folioInicialEncontrado.IdContenedor && x.IdContenedor <= folioFinalEncontrado.IdContenedor && x.Id >= folioInicialEncontrado.Id && x.Id <= folioFinalEncontrado.Id).ToList();

                //var resultadopp = foliosEntontradosEnContenedores.FirstOrDefault().Id;
                // int resultado = foliosEntontradosEnContenedores.Count();
                if (foliosEntontradosEnContenedores.Count() > 0)
                {
                    // var nuevos =foliosEntontradosEnContenedores.Where(x => x.Id >= folioInicialEncontrado.Id && x.Id <= folioFinalEncontrado.Id);
                    //
                    //Referencia para saber que contiene la lista que vera el usuario Folios Sin incidencias es por que no era null el campo IdIncidencias
                    ListafoliosNoDisponibles.Add("Folios Sin Guardar Incidencia");

                    // List<Tbl_InventarioDetalle> listafoliosEntontradosEnContenedores = foliosEntontradosEnContenedores.ToList();

                    foreach (var detalle in foliosEntontradosEnContenedores)
                    {
                        if (detalle.IdIncidencia == null)
                        {
                            //se modifica el tipo de incidencia
                            detalle.IdIncidencia = IdIncidencia;
                            detalle.FechaIncidencia = DateTime.Now.Date;

                            if (IdEmpleado > 0)
                                detalle.IdEmpleado = IdEmpleado;

                            //se guarda la modificacion
                            var entidadDevuleta = repositorioDetalle.Modificar(detalle);

                            //si se guardo 
                            if (entidadDevuleta != null)
                            {
                                //entonces obtenemos el contenedor donde se encuentra el registro que se modifico para restarle a sus formas disponibles
                                var contenedorRestar = repositorio.Obtener(x => x.NumContenedor == entidadDevuleta.IdContenedor);

                                //traemos el inventario donde se debe de restar tambien
                                var inventarioRepositorio = new Repositorio<Tbl_Inventario>(transaccion);
                                var inventarioRestar = inventarioRepositorio.Obtener(x => x.Id == contenedorRestar.IdInventario);

                                //Si se inhabilito se le resta uno alas formas disponibles y se le suma 1 en las formas inhabilitadas
                                if (IdIncidencia == 1)
                                {
                                    //se modifica el contenedor con resta y suma 
                                    contenedorRestar.FormasDisponiblesActuales -= 1;
                                    contenedorRestar.FormasInhabilitadas += 1;
                                    repositorio.Modificar(contenedorRestar);

                                    //se modifica el inventario
                                    inventarioRestar.FormasDisponibles -= 1;
                                    inventarioRepositorio.Modificar(inventarioRestar);

                                }

                                //Si se inhabilito se le resta uno alas formas disponibles y se le suma 1 en las formas Asignadas
                                if (IdIncidencia == 2)
                                {
                                    //se modifica el contenedor con resta y suma 
                                    contenedorRestar.FormasDisponiblesActuales -= 1;
                                    contenedorRestar.FormasAsignadas += 1;
                                    repositorio.Modificar(contenedorRestar);

                                    //se modifica el inventario
                                    inventarioRestar.FormasDisponibles -= 1;
                                    inventarioRepositorio.Modificar(inventarioRestar);

                                }
                            }

                        }
                        else
                        {
                            ListafoliosNoDisponibles.Add(detalle.NumFolio);
                        }
                    }


                }
            }



            return ListafoliosNoDisponibles;
        }




        public static List<string> CrearIncidenciasFoliosContenedor(int IdInventario, string NumeroOrden, int IdContenedor, string FolioInicial, string FolioFinal, int IdIncidencia, int IdEmpleado)
        {
            //HAy que verificar este metodo aqui nos quedamos 11/05/2021 2:41 PM
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var inventarioRepositorio = new Repositorio<Tbl_Inventario>(transaccion);

            int folioInicialEncontrado = repositorio.Obtener(x => x.IdContenedor == IdContenedor && x.NumFolio == FolioInicial && x.Activo == true).Id;
            int folioFinalEncontrado = repositorio.Obtener(x => x.IdContenedor == IdContenedor && x.NumFolio == FolioFinal && x.Activo == true).Id;


            var foliosEncontradosEnContenedores = repositorio.ObtenerPorFiltro(x => x.IdContenedor == IdContenedor && x.Id >= folioInicialEncontrado && x.Id <= folioFinalEncontrado).ToList();

            List<string> ListafoliosNoDisponiblesContenedor = new List<string>();

            ListafoliosNoDisponiblesContenedor.Add("Folios No Disponibles");
            foreach (Tbl_InventarioDetalle nuevoDetalle in foliosEncontradosEnContenedores)
            {
                if (nuevoDetalle.IdIncidencia == null)
                {
                    //se modifica el tipo de incidencia
                    nuevoDetalle.IdIncidencia = IdIncidencia;
                    nuevoDetalle.FechaIncidencia = DateTime.Now.Date;

                    if (IdEmpleado > 0)
                        nuevoDetalle.IdEmpleado = IdEmpleado;

                    //se guarda la modificacion
                    var entidadDevuleta = repositorio.Modificar(nuevoDetalle);

                    //si se guardo 
                    if (entidadDevuleta != null)
                    {
                        //entonces obtenemos el contenedor donde se encuentra el registro que se modifico para restarle a sus formas disponibles
                        var contenedorRestar = repositorioContenedor.Obtener(x => x.IdInventario == IdInventario && x.NumOrden.Trim() == NumeroOrden.Trim() && x.NumContenedor == entidadDevuleta.IdContenedor);

                        //traemos el inventario donde se debe de restar tambien
                        var inventarioRestar = inventarioRepositorio.Obtener(x => x.Id == contenedorRestar.IdInventario);

                        //Si se inhabilito se le resta uno alas formas disponibles y se le suma 1 en las formas inhabilitadas
                        if (IdIncidencia == 1)
                        {
                            //se modifica el contenedor con resta y suma 
                            contenedorRestar.FormasDisponiblesActuales -= 1;
                            contenedorRestar.FormasInhabilitadas += 1;
                            repositorioContenedor.Modificar(contenedorRestar);

                            //se modifica el inventario
                            inventarioRestar.FormasDisponibles -= 1;
                            inventarioRepositorio.Modificar(inventarioRestar);

                        }

                        //Si se inhabilito se le resta uno alas formas disponibles y se le suma 1 en las formas Asignadas
                        if (IdIncidencia == 2)
                        {
                            //se modifica el contenedor con resta y suma 
                            contenedorRestar.FormasDisponiblesActuales -= 1;
                            contenedorRestar.FormasAsignadas += 1;
                            repositorioContenedor.Modificar(contenedorRestar);

                            //se modifica el inventario
                            inventarioRestar.FormasDisponibles -= 1;
                            inventarioRepositorio.Modificar(inventarioRestar);

                        }
                    }

                }
                else
                {
                    ListafoliosNoDisponiblesContenedor.Add(nuevoDetalle.NumFolio);
                }
            }

            return ListafoliosNoDisponiblesContenedor;
        }






        //
        public static IEnumerable<Tbl_InventarioDetalle> obtenerDetallesTabla(int IdInventario )
        {
            var transaccion = new Transaccion();
            var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);


            var contenedoresEncontrados = repositorioContenedor.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo == true);

            List<int> IdContenedores = new List<int>();

            foreach (Tbl_InventarioContenedores nuevoContenedor in contenedoresEncontrados)
            {
                IdContenedores.Add(nuevoContenedor.Id);     
            }

           
                int idContenedorInicial = IdContenedores.Min();
                int idContenedorFinal = IdContenedores.Max();

                var detallesEncontradosContenedores = repositorioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= idContenedorInicial && x.IdContenedor <= idContenedorFinal && x.Activo == true);

           


            return detallesEncontradosContenedores;
        }



        /// <summary>
        /// Guarda las solicitud con n cantidad de bancos creadas en la DB y devuelve el numero de memorandum con el que se asigno  
        /// </summary>
        /// <param name="solicitudCreada"></param>
        /// <returns></returns>
        public static int GuardarSolicitudCreada(List<SolicitudCreadaDTO> solicitudCreada /*,string CuentaBanco, string CantidadFormas, string FInicial*/)
        {
            bool bandera = false;

            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var repositorioSolicitud = new Repositorio<Tbl_Solicitudes>(transaccion);




            var solicitudesEncontrada = repositorioSolicitud.ObtenerPorFiltro(x => x.Activo == true).ToList();


            List<int> numMemosEncontrados = new List<int>();
            foreach (Tbl_Solicitudes solicitud in solicitudesEncontrada)
            {
                numMemosEncontrados.Add(solicitud.NumeroMemo);
            }

           // int numMemo;

            int numMemo = solicitudesEncontrada.Count() == 0 ? 0 : numMemosEncontrados.Max();

      //      int numMemoGuardar = numMemo == 0 ? 1 : numMemosEncontrados.Max();

            numMemo += 1;

            int valorADevolver= 0;
            foreach (SolicitudCreadaDTO solicitud in solicitudCreada) 
            {
                Tbl_CuentasBancarias cuentaEncontrada = repositorioCuentaBancaria.Obtener(x => x.Cuenta.Trim() == solicitud.cuentaBanco.Trim());

                Tbl_Solicitudes nuevoBancoSolicitud = new Tbl_Solicitudes();

                nuevoBancoSolicitud.NumeroMemo = numMemo;
                nuevoBancoSolicitud.IdCuentaBancaria = cuentaEncontrada.Id;
                nuevoBancoSolicitud.Cantidad = Convert.ToInt32(solicitud.cantidadFormas);
                nuevoBancoSolicitud.FolioInicial = solicitud.fInicial;
                nuevoBancoSolicitud.FechaSolicitud = DateTime.Now.Date;
                nuevoBancoSolicitud.Activo = true;

               var entidadGuardada = repositorioSolicitud.Agregar(nuevoBancoSolicitud);

                if (entidadGuardada != null)
                   valorADevolver = numMemo;
            }

            return valorADevolver;
        }



        public static IEnumerable<Tbl_Solicitudes> ObtenerSolicitudes() 
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Solicitudes>(transaccion);

            return repositorio.ObtenerPorFiltro(x => x.Activo == true);

        }


    }
}
