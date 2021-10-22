using DAP.Foliacion.Datos;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Negocios
{
    public class BuscadorChequeNegocios
    {

        public static List<ResultadoObtenidoParaSelect2> BuscarDatoPorFiltro(int TipoDeBusqueda, string BuscarDato)
        {
            //int UltimaQuincenaObtenida = 0;
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Pagos>(transaccion);


           List<ResultadoObtenidoParaSelect2> registrosEncontrados = new List<ResultadoObtenidoParaSelect2>();
           // ResultadoObtenidoParaSelect2 nuevoResultado = new ResultadoObtenidoParaSelect2();

            switch (TipoDeBusqueda)
            {
                case 1:

                    try
                    {
                        // Bloque de codigo para buscar por CHEQUE
                        int numCheque = Convert.ToInt32(BuscarDato);
                        int numCheFinal = numCheque + 50;
                        //var resultadoChequeObtenido = repositorio.ObtenerPorFiltro(x => x.FolioCheque == numCheque).ToList();
                        ////var resultadoChequeObtenido = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => new { y.FolioCheque, y.EsPenA, y.NumEmpleado ,y.NombreEmpleado , y.BeneficiarioPenA }).Distinct().ToList();
                        ////var resultadoChequeFiltrado = resultadoChequeObtenido.Where(x => x.FolioCheque.ToString().Contains(BuscarDato));
                       
                        
                        var resultadoChequeFiltrado = repositorio.ObtenerPorFiltro(x => x.FolioCheque == numCheque  ).ToList();
                        
                        foreach (var resultado in resultadoChequeFiltrado)
                        {
                            ResultadoObtenidoParaSelect2 nuevoResultadoNumEmpleado = new ResultadoObtenidoParaSelect2();
                            nuevoResultadoNumEmpleado.id = resultado.Id;
                            if (resultado.EsPenA == true)
                            {
                                nuevoResultadoNumEmpleado.text = "Folio : " + resultado.FolioCheque + " "+resultado.Tbl_CuentasBancarias.Abreviatura+" || Num : " + resultado.NumEmpleado + " || Empleado : "+resultado.NombreEmpleado+" || Beneficiario PenA : " + resultado.BeneficiarioPenA;
                            }
                            else 
                            {
                                nuevoResultadoNumEmpleado.text = "Folio : " + resultado.FolioCheque + " "+resultado.Tbl_CuentasBancarias.Abreviatura+" || Num : " + resultado.NumEmpleado + " || Beneficiario : " + resultado.NombreEmpleado;
                            }

                            registrosEncontrados.Add(nuevoResultadoNumEmpleado);
                        }
                    }
                    catch (Exception e)
                    {
                        return registrosEncontrados;
                    }


                    return registrosEncontrados;

                case 2:
                    // Bloque de codigo para buscar por NUMERO DE EMPLEADO 

                    try
                    {
                        int NumEmpleado = Convert.ToInt32(BuscarDato);
                        var datosNumEmpleadoObtenidos = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(y => new { y.NumEmpleado, y.NombreEmpleado }).Distinct().ToList();

                        var empleadoObtenido = datosNumEmpleadoObtenidos.Where(x => x.NumEmpleado == NumEmpleado).ToList();

                        foreach (var resultado in empleadoObtenido)
                        {
                            ResultadoObtenidoParaSelect2 nuevoResultadoGuarda = new ResultadoObtenidoParaSelect2();
                            nuevoResultadoGuarda.id = resultado.NumEmpleado;
                            nuevoResultadoGuarda.text = "Num : " + resultado.NumEmpleado + " || NombreEmpleado : " + resultado.NombreEmpleado;
                            registrosEncontrados.Add(nuevoResultadoGuarda);
                        }
                    }
                    catch (Exception e) 
                    {
                        return registrosEncontrados;
                    }

                    return registrosEncontrados;
                 
                case 3:
                    try
                    {     // Bloque de codigo para buscar por el nombre del BENEFICIARIO DEL CHEQUE
                            var beneficiariosEncontrados = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.NombreEmpleado.Contains(BuscarDato)).Select(y => new { y.NumEmpleado, y.NombreEmpleado }).Distinct().ToList();

                            //var beneficiariosEncontrado2s = repositorio.ObtenerPorFiltro(x => x.Activo == true ).Select(y => new { y.NumEmpleado, y.NombreEmpleado }).Distinct().ToList();
                          //  var beneficiariosEncontrados = repositorio.ObtenerPorFiltro(x => x.NombreEmpleado.Contains( BuscarDato)).ToList();

                           // BuscarDato en los beneficiarios de pension alimenticia
                           var beneficiariosEncontradosPenA2  = repositorio.ObtenerPorFiltro(x => x.BeneficiarioPenA.Contains( BuscarDato)).Distinct().ToList();

                           var beneficiariosEncontradosPenA = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.BeneficiarioPenA.Contains(BuscarDato)).Select(y => new { y.NumEmpleado, y.BeneficiarioPenA }).Distinct().ToList();
                        // var beneficiariosEncontradosPenA  = repositorio.ObtenerPorFiltro(x => x.BeneficiarioPenA.Contains( BuscarDato)).ToList();

                        foreach (var resultado in beneficiariosEncontrados)
                            {
                                ResultadoObtenidoParaSelect2 nuevoResultadoNombre = new ResultadoObtenidoParaSelect2();
                                nuevoResultadoNombre.id = resultado.NumEmpleado;
                                nuevoResultadoNombre.text = "Num : " + resultado.NumEmpleado + " || Beneficiario : " + resultado.NombreEmpleado;
                                registrosEncontrados.Add(nuevoResultadoNombre);
                            }


                            foreach (var resultado in beneficiariosEncontradosPenA)
                            {
                                ResultadoObtenidoParaSelect2 nuevoResultadoNombre = new ResultadoObtenidoParaSelect2();
                                nuevoResultadoNombre.id = resultado.NumEmpleado;
                                nuevoResultadoNombre.text = "Num : "+resultado.NumEmpleado+" || BeneficiarioPenA : "+resultado.BeneficiarioPenA;
                                registrosEncontrados.Add(nuevoResultadoNombre);
                            }



                    }
                            catch (Exception e)
                    {
                        return registrosEncontrados;
                    }






            return registrosEncontrados;

                default:

                    // aun por verificar si el usuario no conoce ningun dato => funcionalidad a futuro
                    // code block
                    return registrosEncontrados;

            }

        }




        public static List<DetallesBusqueda> ObtenerDetallesDeCheque(int IdRegistroDeFolio) 
        {
            List<DetallesBusqueda> detalleEncontrado = new List<DetallesBusqueda>();

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);

            Tbl_Pagos chequeEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroDeFolio);

            DetallesBusqueda nuevoChequeEncontrado = new DetallesBusqueda();
            nuevoChequeEncontrado.IdRegistro = chequeEncontrado.Id;
            nuevoChequeEncontrado.Id_nom = chequeEncontrado.Id_nom ;
            nuevoChequeEncontrado.ReferenciaBitacora = chequeEncontrado.ReferenciaBitacora;
            nuevoChequeEncontrado.Quincena = chequeEncontrado.Quincena;
            nuevoChequeEncontrado.NumEmpleado = chequeEncontrado.NumEmpleado;
            nuevoChequeEncontrado.NombreBeneficiaro = chequeEncontrado.EsPenA == true ?  chequeEncontrado.BeneficiarioPenA : chequeEncontrado.NombreEmpleado;
            nuevoChequeEncontrado.NumBene = chequeEncontrado.EsPenA == true ? chequeEncontrado.NumBeneficiario : "";
            nuevoChequeEncontrado.IdRegistro = chequeEncontrado.Id;
            nuevoChequeEncontrado.FolioCheque = chequeEncontrado.FolioCheque;
            nuevoChequeEncontrado.Liquido = chequeEncontrado.ImporteLiquido;
            nuevoChequeEncontrado.EstadoCheque = chequeEncontrado.Cat_EstadosPago_Pagos.Descrip;

            detalleEncontrado.Add(nuevoChequeEncontrado);

            return detalleEncontrado;
        }



        public static List<DetallesBusqueda> ObtenerDetallesNumEmpleado(int NumEmpleado)
        {
            List<DetallesBusqueda> detalleEncontrado = new List<DetallesBusqueda>();

            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);

            List<Tbl_Pagos> registrosNumeroEmpleadoEncontrados = repositorioTBlPagos.ObtenerPorFiltro(x => x.NumEmpleado == NumEmpleado).ToList();

            foreach (Tbl_Pagos registroEncontrado in registrosNumeroEmpleadoEncontrados) 
            {
                DetallesBusqueda nuevoChequeEncontrado = new DetallesBusqueda();
                nuevoChequeEncontrado.IdRegistro = registroEncontrado.Id;
                nuevoChequeEncontrado.Id_nom = registroEncontrado.Id_nom;
                nuevoChequeEncontrado.ReferenciaBitacora = registroEncontrado.ReferenciaBitacora;
                nuevoChequeEncontrado.Quincena = registroEncontrado.Quincena;
                nuevoChequeEncontrado.NumEmpleado = registroEncontrado.NumEmpleado;
                nuevoChequeEncontrado.NombreBeneficiaro = registroEncontrado.EsPenA == true ?  registroEncontrado.BeneficiarioPenA : registroEncontrado.NombreEmpleado ;
                nuevoChequeEncontrado.NumBene = registroEncontrado.EsPenA == true ? registroEncontrado.NumBeneficiario : "";
                nuevoChequeEncontrado.IdRegistro = registroEncontrado.Id;
                nuevoChequeEncontrado.FolioCheque = registroEncontrado.FolioCheque;
                nuevoChequeEncontrado.Liquido = registroEncontrado.ImporteLiquido;
                nuevoChequeEncontrado.EstadoCheque = registroEncontrado.Cat_EstadosPago_Pagos.Descrip;

                detalleEncontrado.Add(nuevoChequeEncontrado);
            }




            return detalleEncontrado;
        }


        public static DetallesRegistroDTO ObtenerDetallesIdRegistro(int IdRegistroBuscar)
        {
            
            var transaccion = new Transaccion();

            var repositorioTBlPagos = new Repositorio<Tbl_Pagos>(transaccion);


            Tbl_Pagos regitroEncontrado = repositorioTBlPagos.Obtener(x => x.Id == IdRegistroBuscar);

            DetallesRegistroDTO nuevoDetalle = new DetallesRegistroDTO();

            nuevoDetalle.IdRegistro = regitroEncontrado.Id;
            nuevoDetalle.Id_nom = regitroEncontrado.Id_nom;
            nuevoDetalle.ReferenciaBitacora = regitroEncontrado.ReferenciaBitacora;

            nuevoDetalle.Quincena = regitroEncontrado.Quincena;
            nuevoDetalle.NumEmpleado = regitroEncontrado.NumEmpleado;
            nuevoDetalle.NombreEmpleado = regitroEncontrado.NombreEmpleado;

            nuevoDetalle.Delegacion = regitroEncontrado.Delegacion;
            nuevoDetalle.Folio = regitroEncontrado.FolioCheque;
            nuevoDetalle.Liquido = regitroEncontrado.ImporteLiquido;

            nuevoDetalle.EstadoCheque = regitroEncontrado.Cat_EstadosPago_Pagos.Descrip;
            nuevoDetalle.BancoPagador = regitroEncontrado.Tbl_CuentasBancarias.NombreBanco;
            nuevoDetalle.CuentaPagadora = regitroEncontrado.Tbl_CuentasBancarias.Cuenta;

            //nuevoDetalle.EsPenA = regitroEncontrado.EsPenA;
            nuevoDetalle.EsPenA = regitroEncontrado.EsPenA == null ? false : regitroEncontrado.EsPenA;
            nuevoDetalle.NumBeneficiarioPenA = regitroEncontrado.NumBeneficiario;
            nuevoDetalle.NombreBeneficiarioPenA = regitroEncontrado.BeneficiarioPenA;

            //nuevoDetalle.EsRefoliado =;
            nuevoDetalle.EsRefoliado = regitroEncontrado.EsRefoliado == null ? false : regitroEncontrado.EsRefoliado;



            /*****************************************************************************************************************************************/
            //nuevoDetalle.Id_nom = regitrosNombreEmpleadoEncontrados.Id_nom;
            //nuevoDetalle.NumEmpleado = regitrosNombreEmpleadoEncontrados.NumEmpleado;
            //nuevoDetalle.Quincena = regitrosNombreEmpleadoEncontrados.Quincena;

            //nuevoDetalle.Folio = regitrosNombreEmpleadoEncontrados.FolioCheque;
            //nuevoDetalle.Liquido = regitrosNombreEmpleadoEncontrados.ImporteLiquido;
            //nuevoDetalle.NombreBeneficiaro = regitrosNombreEmpleadoEncontrados.NombreEmpleado;

            //nuevoDetalle.EstadoCheque = regitrosNombreEmpleadoEncontrados.Cat_EstadosPago_Pagos.Descrip;
            //nuevoDetalle.ReferenciaBitacora = regitrosNombreEmpleadoEncontrados.ReferenciaBitacora;
            //nuevoDetalle.BancoPagador = regitrosNombreEmpleadoEncontrados.Tbl_CuentasBancarias.NombreBanco;

         
            
            //nuevoDetalle.NombreBeneficiarioPenA = regitrosNombreEmpleadoEncontrados.BeneficiarioPenA;
            

            //nuevoDetalle.EsRefoliado = regitrosNombreEmpleadoEncontrados.EsRefoliado == null? false: regitrosNombreEmpleadoEncontrados.EsRefoliado;

            /**************************************************************************************************************************************/

 





            return nuevoDetalle;
        }

    }
}
