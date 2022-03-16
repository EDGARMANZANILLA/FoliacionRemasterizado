using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.FinalizarReferencia_CanceladosDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios
{
    public class FinalizarReferencia_CanceladosNegocios
    {



        public static Dictionary<int, string> ObtenerReferenciasActivas(int anioActual)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            List<Tbl_Referencias_Cancelaciones> registrosReferenciasObtenidos = repositorio.ObtenerPorFiltro(x => x.Anio == anioActual && x.Activo == true).OrderBy(x => x.Numero_Referencia).ToList();


            Dictionary<int, string> refenciasFiltradas = new Dictionary<int, string>();
            if (registrosReferenciasObtenidos.Count > 0)
            {
                int iterador = 0;
                foreach (Tbl_Referencias_Cancelaciones nuevaReferencia in registrosReferenciasObtenidos)
                {

                    refenciasFiltradas.Add(nuevaReferencia.Id, nuevaReferencia.Anio+""+nuevaReferencia.Numero_Referencia + " || " + " Contiene " + nuevaReferencia.FormasPagoDentroReferencia + " formas de pago ");
               }
            }

            return refenciasFiltradas;
        }


        public static List<DetallePagoDTO> ObtenerDetalleDePagosDentroReferencia(int IdReferencia)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Pagos>(transaccion);
            List<Tbl_Pagos> registrosPagosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == IdReferencia && x.Activo == true).ToList();

            var repositorioBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            List<DetallePagoDTO> ListaDetallePagoFiltrado = new List<DetallePagoDTO>();
            int iterador = 1;
            if (registrosPagosEncontrados.Count > 0)
            {
                foreach (Tbl_Pagos nuevoPago in registrosPagosEncontrados)
                {
                    Tbl_CuentasBancarias banco = repositorioBanco.Obtener(x => x.Id == nuevoPago.IdTbl_CuentaBancaria_BancoPagador);
                    DetallePagoDTO nuevoDetalle = new DetallePagoDTO();

                    nuevoDetalle.Id = iterador++;
                    nuevoDetalle.IdRegistro =nuevoPago.Id;
                    nuevoDetalle.IdNom = nuevoPago.Id_nom;
                    nuevoDetalle.Quicena = nuevoPago.Quincena;
                    nuevoDetalle.ReferenciaOriginal = nuevoPago.ReferenciaBitacora;
                    nuevoDetalle.Nomina = nuevoPago.Nomina;
                    nuevoDetalle.Delegacion = nuevoPago.Delegacion;
                    nuevoDetalle.Partida = nuevoPago.Partida;

                    nuevoDetalle.EsPena = nuevoPago.EsPenA == true ? "TRUE" : "";
                    nuevoDetalle.Nombre = nuevoPago.EsPenA == true ?  nuevoPago.BeneficiarioPenA : nuevoPago.NombreEmpleado;
                    nuevoDetalle.Num = nuevoPago.NumEmpleado;
                    nuevoDetalle.Folio =nuevoPago.FolioCheque;
                    nuevoDetalle.Liquido = nuevoPago.ImporteLiquido;
             
                    nuevoDetalle.CFDI = nuevoPago.FolioCFDI != null ? Convert.ToString( nuevoPago.FolioCFDI) : "";
                    nuevoDetalle.CTA_Bancaria = banco.Cuenta;
                    nuevoDetalle.Banco = banco.NombreBanco;

                    ListaDetallePagoFiltrado.Add(nuevoDetalle);
                }
            }

            return ListaDetallePagoFiltrado;
        }



       
    }
}
