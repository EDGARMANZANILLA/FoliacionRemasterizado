using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Datos;


namespace DAP.Foliacion.Negocios
{
    public class InventarioNegocios
    {

        public static IEnumerable<Tbl_Inventario> ObtenerInventarioActivo()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);

            var InventariosActivos = repositorio.ObtenerPorFiltro(x => x.Activo == true);


            return InventariosActivos;
        }



        public static int ObtenerIdBanco(string banco)
        {
            int idBanco = 0;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var BancosActivos = repositorio.Obtener(x => x.NombreBanco == banco.Trim() && x.Activo == true);

            if (BancosActivos != null)
            {
                idBanco = BancosActivos.Id;
            }


            return idBanco;
        }



       public static bool  GuardarInventarioContenedores(int idBanco, string numeroOrden, int numeroContenedor,  string FInicial, string FFinal, int TotalFormas)
       {
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);

            
            var inventarioFiltrado = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria == idBanco && x.Activo == true);



            try
            {
                foreach (Tbl_Inventario inventarioObtenido in inventarioFiltrado)
                {
                    inventarioObtenido.FormasDisponibles += TotalFormas;
                    inventarioObtenido.UltimoFolioInventario = Convert.ToInt32(FFinal);

                    if (inventarioObtenido.FormasQuincena1 > 0 && inventarioObtenido.FormasQuincena2 > 0)
                    {
                        inventarioObtenido.EstimadoMeses = inventarioObtenido.FormasDisponibles / (inventarioObtenido.FormasQuincena1 + inventarioObtenido.FormasQuincena2);
                    }

                     
                }


              //  var repositorio2 = new Repositorio<Tbl_Inventario_Detalle>(transaccion);
                Tbl_Inventario_Detalle nuevoDetalle = new Tbl_Inventario_Detalle();

                nuevoDetalle.IdInventario = idBanco;
                nuevoDetalle.NumOrden = numeroOrden;
                nuevoDetalle.NumContenedor = numeroContenedor;
                nuevoDetalle.FolioInicial = FInicial;
                nuevoDetalle.FolioFinal = FFinal;
                nuevoDetalle.TotalFormasContenedor = TotalFormas;
                nuevoDetalle.FormasDisponiblesActuales = TotalFormas;
                nuevoDetalle.FechaAlta = DateTime.Now.Date;
                nuevoDetalle.Activo = true;

                bandera = GuardarRegistroDelDetalle(nuevoDetalle);

                if (bandera) 
                {
                    transaccion.GuardarCambios();
                }
                else 
                {
                    transaccion.Dispose();
                }
                         

              //  bandera = true;

            }
            catch (Exception e)
            {
                bandera  = false;
            }

  




            return bandera;

       }





        public static bool GuardarRegistroDelDetalle(Tbl_Inventario_Detalle NuevoDetalle)
        {
            bool bandera;
            var transaccion = new Transaccion();
            try
            {
                var repositorio = new Repositorio<Tbl_Inventario_Detalle>(transaccion);
                repositorio.Agregar(NuevoDetalle);
                bandera = true;
            }
            catch (Exception ex)
            {
                transaccion.Dispose();
                bandera = false;
            }
            return bandera;
        }











    }
}
