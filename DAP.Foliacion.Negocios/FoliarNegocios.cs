using DAP.Foliacion.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades;

namespace DAP.Foliacion.Negocios
{
    public class FoliarNegocios
    { 

        public static Dictionary<int, string> ObtenerNominasXNumeroQuincena(string NumeroQuincena) 
        {
            List<NombreNominasDTO> nombresNomina = FoliarConsultasDBSinEntity.ObtenerNombreNominas(NumeroQuincena);

            Dictionary<int, string> nombresListosNomina = new Dictionary<int, string>();

            int i = 0;
            foreach (NombreNominasDTO NuevaNomina in nombresNomina)
            {
                i += 01;
                if (NuevaNomina.Adicional == "")
                {        
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]  [ " + NuevaNomina.Nomina + " ] " + " -" + "-" + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }
                else 
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]  [ " + NuevaNomina.Nomina + " ] " + " -" + "-" + "- " + "ADICIONAL" +" -" + "-" + "- " + NuevaNomina.Adicional + " -" + "- " + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }


            }

            return nombresListosNomina;
        }

        public static Dictionary<int, string> ObtenerBancosParaPagomatico() 
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true);

            Dictionary<int, string> bancosMostrar= new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados) 
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " "+cuentaPagomatico.NombreBanco+" "+" - "+" [ "+cuentaPagomatico.Cuenta+" ] ");
            }

            return bancosMostrar;
        }

        /// <summary>
        /// Obtiene los bancos con lo que se pueden 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> ObtenerBancoParaFormasPago()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);

            Dictionary<int, string> bancosMostrar = new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados)
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " " + cuentaPagomatico.NombreBanco + " " + " - " + " [ " + cuentaPagomatico.Cuenta + " ] ");
            }

            return bancosMostrar;
        }




        public static string ObtenerBancoPorID(int Id)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            Tbl_CuentasBancarias resultado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);

            return  resultado.NombreBanco+" - [ "+resultado.Cuenta +" ] "; 
        }



        /// <summary>
        /// Devuelve el an de como se encuentra en bitacora de una nomina especifica para revisarla o foliarla
        /// </summary>
        /// <param name="IdNum">numero de la nomina que corresponde la seleccion del usuario</param>
        /// <returns></returns>
        public static List<string> ObtenerAnBitacoraIdNum(int IdNum) 
        {
          return  FoliarConsultasDBSinEntity.ObtenerAnBitacoraPorIdNumConexion(IdNum);
        
        }



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaRevicion( string NumeroNomina, string An, int Quincena) 
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true ).Select(y => y.Id ).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas) 
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }

            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPagomatico(NumeroNomina, An, Quincena, NombresBanco);
  
            return DatosNomina;
        }

        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaPENALRevicion(string NumeroNomina ,string An, int Quincena)
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).Select(y => y.Id).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas)
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }

            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPenalPagomatico(NumeroNomina, An, Quincena, NombresBanco);

            return DatosNomina;
        }


        public static string ObtenerRutaCOmpletaArchivoIdNomina(int IdNomina) 
        {
            return FoliarConsultasDBSinEntity.ObtenerRutaIdNomina(IdNomina);
        }


      
        public static string ObtenerNumeroNominaXIdNumBitacora(int IdNum)
        {
           return FoliarConsultasDBSinEntity.ObtenerNumeroNominaXIdNum(IdNum);
        }



        /// <summary>
        /// Obtine una lista de las nominas que pueden ser Foliadas para que se foleen todas de un solo jalon
        /// recibe como parametro el numero de quincena ejem 2112
        /// </summary>
        /// <param name="Quincena"></param>
        /// <returns></returns>
        public static List<DatosRevicionTodasNominasDTO> ObtenerTodasNominasXQuincena(string NumeroQuincena) {

            return FoliarConsultasDBSinEntity.ObtenerListaDTOTodasNominasXquincena(NumeroQuincena);
        }





        //Metodos para foliar formas de pagos (cheques)
        public static DatosBitacoraParaCheque ObtenerDetallesNominaParaCheques(int IdNomina) 
        {
           DatosBitacoraParaCheque datosEncontrados =  FoliarConsultasDBSinEntity.ObtenerAnBitacoraParaCheques(IdNomina);

            //verifica que no haya ocurrido un problema al traer los datos
            if (datosEncontrados.Comentario != "Sin Datos")
            {
                //verifica que la nomina sea general o desentralizado ya que son las dos unicas que se imprimen en diferentes archivos (Confianza y sindicalizados C-G?2115.tx y S-G?2115.txt)
                if (datosEncontrados.Nomina == "01" || datosEncontrados.Nomina == "02")
                {
                    

                }
            }

        }

    }
}
