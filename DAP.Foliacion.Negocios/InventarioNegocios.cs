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

        public static IEnumerable<Tbl_Inventario> ObtenerInventarioActivo()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);

            var InventariosActivos = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.Tbl_CuentasBancarias.IdCuentaBancaria_TipoPagoCuenta != 1);


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

        public static int ObtenerIdInventario(int banco)
        {
            int idInventario = 0;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);

            var InventarioActivos = repositorio.Obtener(x => x.IdCuentaBancaria == banco && x.Activo == true);

            if (InventarioActivos != null)
            {
                idInventario = InventarioActivos.Id;
            }


            return idInventario;
        }


        #region Metodos para agregar contenedores y actualizar inventario general
        public static bool GuardarInventarioContenedores(int idInventario,int idBanco, string numeroOrden, int numeroContenedor, string FInicial, string FFinal, int TotalFormas)
        {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);


            var inventarioFiltrado = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria == idBanco && x.Activo == true);



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

                 var contenedorAgregado =repositorioContenedor.Agregar(nuevoContenedor);
               


             
                //Agrega el detalle de cada contenedor para saber cada uno de sus folios 
                var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

                for (int i = Convert.ToInt32(FInicial); i <= Convert.ToInt32(FFinal); i++)
                {
                    Tbl_InventarioDetalle nuevoDetalle =new Tbl_InventarioDetalle();

                    nuevoDetalle.IdContenedor = contenedorAgregado.Id;
                    nuevoDetalle.NumFolio  = Convert.ToString( i );

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


        public static IEnumerable<Tbl_CuentasBancarias> ObtenerBancosConChequera()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var BancosConChequeraActivos = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);


            return BancosConChequeraActivos;
        }


        #region metodos para inhabilitar y solicitar formas de pago
        public static string ObtenerCuentaBancariaIdBanco(int Idbanco)
        {
            //string cuenta = "";
            var transaccion = new Transaccion();


            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var NumeroDeCuenta = repositorio.Obtener(x => x.Id == Idbanco && x.Activo == true);

            
            return NumeroDeCuenta.Cuenta;
        }

        //regresa el id de un registro del inventario filtrado por IdCuentaBancaria
        public static int ObtenerIdInventarioPorIdCuentaBancaria(int Idbanco)
        {
           
            var transaccion = new Transaccion();


            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);
            var inventarioObtenido = repositorio.Obtener(x => x.IdCuentaBancaria == Idbanco && x.Activo == true);

           return inventarioObtenido.Id;
        }





        #endregion


        //Valida folios para saber si estan disponibles
        public static List<String> ValidarFoliosDisponibles(int IdInventario,int FolioInicial, int FolioFinal) 
        {
            var transaccion = new Transaccion();

         
            //obtiene el contenedor donde se encuentra el rango de folio inicial y el folio final
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo) ;


           //el diccionario sirve para guardar la order y el contenedor con folios duplicados
            List<string> contenedoresFoliosDuplicados = new List<string>();

            //Referencia para saber que contiene la lista que vera el usuario
            contenedoresFoliosDuplicados.Add("ErrorContenedores");
            int idContenedor = 0;
            foreach (Tbl_InventarioContenedores contenedor in contenedoresEncontrados) 
            {
                if(FolioInicial >= Convert.ToInt32(contenedor.FolioInicial) &&  FolioFinal <= Convert.ToInt32(contenedor.FolioFinal))
                {
                    if (idContenedor == 0)
                    {
                        idContenedor = contenedor.Id;
                        contenedoresFoliosDuplicados.Add(contenedor.NumOrden);
                        contenedoresFoliosDuplicados.Add(Convert.ToString(contenedor.NumContenedor));
                    }
                    else 
                    {
                        contenedoresFoliosDuplicados.Add(contenedor.NumOrden);
                        contenedoresFoliosDuplicados.Add(Convert.ToString( contenedor.NumContenedor));
                    }

                    

                } 
            }

            //si se encontraron folios duplicados devuelve una lista de la orden y contenedores
            if (contenedoresFoliosDuplicados.Count > 3) 
            {
                return contenedoresFoliosDuplicados;
            }
            

            List<String> ListafoliosNoDisponibles = new List<string>();
            if (idContenedor > 0)
            {
                //obtener el id de donde inicia el numerode folio para una busqueda mas rapida
                string cadenaFolioInicial = Convert.ToString(FolioInicial);
                string cadenaFolioFinal = Convert.ToString(FolioFinal);
                var repositorio1 = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                int idFoliosInicialEntontrado = repositorio1.Obtener(x => x.IdContenedor == idContenedor && x.NumFolio == cadenaFolioInicial  && x.IdIncidencia == null).Id;
                int idFoliosFinalEntontrado = repositorio1.Obtener(x => x.IdContenedor == idContenedor && x.NumFolio == cadenaFolioFinal && x.IdIncidencia == null).Id;

                //obtiene los registros que se tienen por el id para reducir el universo de folios
                var repositorio2 = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                var foliosEntontrados = repositorio2.ObtenerPorFiltro(x => x.IdContenedor == idContenedor && x.Id >= idFoliosInicialEntontrado && x.Id <= idFoliosFinalEntontrado && x.IdIncidencia == null );

                //Referencia para saber que contiene la lista que vera el usuario
                ListafoliosNoDisponibles.Add("FoliosNoDisponibles");
                foreach (Tbl_InventarioDetalle detalle in foliosEntontrados)
                {
                        if (detalle.IdIncidencia != null)
                        {
                            ListafoliosNoDisponibles.Add(Convert.ToString(detalle.NumFolio));
                        }                    
                }
              
            }


            return ListafoliosNoDisponibles;
        }





        //Metodos para Ajustar
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
















        // //metodos para inhabilitar  (ObtenerNumeroOrdenesBancoActivo funciona para AJustar tambien)
        // public static List<string> ObtenerNumeroOrdenesBancoActivo(int Idbanco)
        // {
        //     string contenedoresEncontrados = "";

        //     List<string> numerosOrden = new List<string>();

        //     var transaccion = new Transaccion();



        //     var repositorio = new Repositorio<Tbl_Inventario_Detalle>(transaccion);

        //     var numerosOrdenEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == Idbanco && x.Activo == true);

        //     foreach (var orden in numerosOrdenEncontrados)
        //     {
        //         if (!numerosOrden.Contains(orden.NumOrden))
        //         {
        //             contenedoresEncontrados = orden.NumOrden;

        //             numerosOrden.Add(contenedoresEncontrados);
        //         }


        //     }



        //     return numerosOrden;

        // }




        // public static IEnumerable<Tbl_Inventario_Inhabilitados> ObtenerInhabilitadosBanco(int idBanco)
        // {

        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Inhabilitados>(transaccion);

        //     return repositorio.ObtenerPorFiltro(x => x.Tbl_Inventario_Detalle.IdInventario == idBanco && x.activo == true);
        // }

        // public static IEnumerable<Tbl_Inventario_Detalle> ObtenerContenedoresActivosPorNumeroOrden(int Idbanco, string OrdenSelecionada)
        // {
        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Detalle>(transaccion);


        //     return repositorio.ObtenerPorFiltro(x => x.IdInventario == Idbanco && x.NumOrden == OrdenSelecionada && x.Activo == true);

        // }


        // public static Tbl_Inventario_Detalle ObtenerFoliosPorContenedor(int IdBanco, string OrdenSeleccionada, int ContenedorSeleccionado)
        // {

        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Detalle>(transaccion);


        //     return repositorio.Obtener(x => x.IdInventario == IdBanco && x.NumOrden == OrdenSeleccionada && x.NumContenedor == ContenedorSeleccionado && x.Activo == true);

        // }


        // public static Tbl_Inventario_Inhabilitados AgregarFoliosInhabilitados(int idInventarioDetalle, int idBanco, string cuenta, string OrdenSeleccionada, int ContenedorSeleccionado, string FolioInicial, string FolioFinal)
        // {
        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Inhabilitados>(transaccion);

        //     Tbl_Inventario_Inhabilitados nuevaInhabilitacion = new Tbl_Inventario_Inhabilitados();

        //     nuevaInhabilitacion.IdInhabilitado = idInventarioDetalle;
        //     nuevaInhabilitacion.FolioInicial = FolioInicial;
        //     nuevaInhabilitacion.FolioFinal = FolioFinal;



        //     if (FolioFinal != "")
        //     {
        //         nuevaInhabilitacion.TotalFormas = (Convert.ToInt32(FolioFinal) - Convert.ToInt32(FolioInicial)) + 1;

        //     }
        //     else
        //     {
        //         nuevaInhabilitacion.TotalFormas = 1;
        //     }

        //     nuevaInhabilitacion.FechaDetalle = DateTime.Now.Date ;
        //     nuevaInhabilitacion.activo = true;

        //     return repositorio.Agregar(nuevaInhabilitacion);

        // }


        ////metodos para mostrar Inventario_Detalles

        // public static IEnumerable<Tbl_Inventario_Detalle> ObtenerInventarioDetalles(int idBanco) 
        // {
        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Detalle>(transaccion);

        //     return  repositorio.ObtenerPorFiltro(x => x.IdInventario == idBanco && x.Activo == true);
        // }



        // public static IEnumerable<Tbl_Inventario_Asignacion> ObtenerInventarioAsignado()
        // {
        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_Asignacion>(transaccion);

        //  return  repositorio.ObtenerPorFiltro(x => x.Activo== true);
        // }


        // public static int ObtenerUnicoIdPersonaActivo( string PersonalSeleccionado)
        // {
        //     var transaccion = new Transaccion();
        //     var repositorio = new Repositorio<Tbl_Inventario_AsignacionPersonal>(transaccion);

        //    return  repositorio.Obtener(x => x.NombrePersonal.Trim() == PersonalSeleccionado.Trim() && x.Activo == true).Id;
        // }

        // public static bool AgregarNuevaAsignacion( int IdContenedor, string PersonalSeleccionado, string FolioInicial, string FolioFinal, int TotalFormasAsignadas)
        // {

        //     bool bandera;

        //     var transaccion = new Transaccion();

        //     try
        //     {
        //         var repositorio = new Repositorio<Tbl_Inventario_Asignacion>(transaccion);

        //         Tbl_Inventario_Asignacion nuevaAsignacion = new Tbl_Inventario_Asignacion();
        //         nuevaAsignacion.IdAsignacion = IdContenedor;
        //         nuevaAsignacion.IdNombrePersona = ObtenerUnicoIdPersonaActivo(PersonalSeleccionado);
        //         nuevaAsignacion.FoliosAsignados = TotalFormasAsignadas;
        //         nuevaAsignacion.FolioInicial = FolioInicial;
        //         nuevaAsignacion.FolioFinal = FolioFinal;
        //         nuevaAsignacion.FechaAsignacion = DateTime.Now.Date;
        //         nuevaAsignacion.Activo = true;
        //         repositorio.Agregar(nuevaAsignacion);

        //         var repositorio1 = new Repositorio<Tbl_Inventario_Detalle>(transaccion);
        //         var inventarioDetalleObtenido = repositorio1.Obtener(x => x.Id == IdContenedor && x.Activo == true);
        //         inventarioDetalleObtenido.FormasDisponiblesActuales -= TotalFormasAsignadas;

        //         var repositorio2 = new Repositorio<Tbl_Inventario>(transaccion);
        //         var inventarioGeneralEncontrado = repositorio2.Obtener(x => x.IdCuentaBancaria == inventarioDetalleObtenido.IdInventario && x.Activo == true);
        //         inventarioGeneralEncontrado.FormasDisponibles -= TotalFormasAsignadas;

        //         transaccion.GuardarCambios();

        //         bandera = true;
        //     }
        //     catch (Exception e)
        //     {
        //         transaccion.Dispose();
        //         bandera = false;
        //     }



        //     return bandera;
        // }


    }
}
