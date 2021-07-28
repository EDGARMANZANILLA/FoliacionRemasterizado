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



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaRevicion(string An, int Quincena, string NombreBanco) 
        {
           
            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPagomatico(An, Quincena, NombreBanco);
  
            return DatosNomina;
        }

        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaPENALRevicion(string An, int Quincena, string NombreBanco)
        {

            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPenalPagomatico(An, Quincena, NombreBanco);

            return DatosNomina;
        }


        public static string ObtenerRutaCOmpletaArchivoIdNomina(int IdNomina) 
        {
            return FoliarConsultasDBSinEntity.ObtenerRutaIdNomina(IdNomina);
        }

    }
}
