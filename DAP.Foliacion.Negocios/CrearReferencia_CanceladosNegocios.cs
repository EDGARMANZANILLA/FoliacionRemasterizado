using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;

namespace DAP.Foliacion.Negocios
{
    public class CrearReferencia_CanceladosNegocios
    {

        public static List<CrearReferenciaDTO> ObtenerReferenciasAnioActual(int anioActual)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);

            List<Tbl_Referencias_Cancelaciones> registrosReferenciasObtenidos = repositorio.ObtenerPorFiltro(x => x.Anio == anioActual).OrderBy(x => x.Numero_Referencia).ToList();

            List<CrearReferenciaDTO> referenciasEncontradas = new List<CrearReferenciaDTO>();

            if (registrosReferenciasObtenidos.Count > 0)
            {
                int iterador = 0;
                foreach (Tbl_Referencias_Cancelaciones nuevaReferencia in registrosReferenciasObtenidos)
                {

                    CrearReferenciaDTO nuevaReferenciaDeCancelado = new CrearReferenciaDTO();

                    nuevaReferenciaDeCancelado.Id = nuevaReferencia.Id;
                    nuevaReferenciaDeCancelado.Id_Iterador = ++iterador;

                    nuevaReferenciaDeCancelado.Anio = nuevaReferencia.Anio;
                    nuevaReferenciaDeCancelado.Numero_Referencia = nuevaReferencia.Numero_Referencia;
                    nuevaReferenciaDeCancelado.Fecha_Creacion = nuevaReferencia.Fecha_Creacion.ToString("MM/dd/yyyy");
                    nuevaReferenciaDeCancelado.Creado_Por = nuevaReferencia.Creado_Por;
                    nuevaReferenciaDeCancelado.FormasPagoCargadas = nuevaReferencia.FormasPagoDentroReferencia;
                    nuevaReferenciaDeCancelado.Activo = nuevaReferencia.Activo;

                    referenciasEncontradas.Add(nuevaReferenciaDeCancelado);
                }
            }

            return referenciasEncontradas;
        }


    


        public static int CrearNuevaReferenciaCancelados(int nuevoNumero)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);


            Tbl_Referencias_Cancelaciones nuevoNumeroRefenciaCanceladoExiste = repositorio.Obtener(x => x.Anio == DateTime.Now.Year && x.Numero_Referencia == nuevoNumero && x.Activo == true);
            int idAgregado=0;
            if (nuevoNumeroRefenciaCanceladoExiste == null)
            {
                Tbl_Referencias_Cancelaciones nuevaReferencia = new Tbl_Referencias_Cancelaciones();
                nuevaReferencia.Anio = DateTime.Now.Year;
                nuevaReferencia.Numero_Referencia = nuevoNumero;
                nuevaReferencia.Fecha_Creacion = DateTime.Now.Date;
                nuevaReferencia.FormasPagoDentroReferencia = 0;
                nuevaReferencia.Creado_Por = "**********";
                nuevaReferencia.Activo = true;
                idAgregado = repositorio.Agregar(nuevaReferencia).Id;
            }

            return idAgregado;
        }




        public static int InactivarReferenciaCancelados(int InactivarIdReferenciaCancelados)
        {
            int cantidadRegistrosModificados = 0;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Referencias_Cancelaciones>(transaccion);
            var repositorioTbl_Pagos = new Repositorio<Tbl_Pagos>(transaccion);

            Tbl_Referencias_Cancelaciones registroReferenciaEncontrado = null;
            
            List<Tbl_Pagos> registrosChequesObtenidos = repositorioTbl_Pagos.ObtenerPorFiltro(x => x.IdTbl_Referencias_Cancelaciones == InactivarIdReferenciaCancelados && x.Activo == true).ToList();
            int numeroVerificacionCancelado = 0;
            if (registrosChequesObtenidos.Count != 0)
            {
                foreach (Tbl_Pagos nuevoChequeObtenido in registrosChequesObtenidos)
                {
                    //deberia devolver 8 si fue removido un cheque de la referencia de cancelacion
                    numeroVerificacionCancelado = BuscadorChequeNegocios.RevocarCheque_ReferenciaCancelado(nuevoChequeObtenido.Id);

                    if (numeroVerificacionCancelado == 8)
                    {
                        cantidadRegistrosModificados += 1;
                    }
                }

                if (registrosChequesObtenidos.Count == cantidadRegistrosModificados)
                {
                    registroReferenciaEncontrado = repositorio.Obtener(x => x.Id == InactivarIdReferenciaCancelados && x.Activo == true);
                    registroReferenciaEncontrado.Activo = false;
                    repositorio.Modificar(registroReferenciaEncontrado);

                }

            }
            else if (registrosChequesObtenidos.Count == 0) 
            {
                registroReferenciaEncontrado = repositorio.Obtener(x => x.Id == InactivarIdReferenciaCancelados && x.Activo == true);
                registroReferenciaEncontrado.Activo = false;
                repositorio.Modificar(registroReferenciaEncontrado);

            }

            return cantidadRegistrosModificados;
        }











    }

}
