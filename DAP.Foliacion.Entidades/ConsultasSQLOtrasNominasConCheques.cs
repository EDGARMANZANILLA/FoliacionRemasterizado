using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades
{
    public class ConsultasSQLOtrasNominasConCheques
    {
        public List<string> ObtenerConsultasTotalesOtrasNominas(string An ) 
        {
            List<string> consultasPrediseneadas = new List<string>();
            /*Campeche */
            consultasPrediseneadas.Add ("select '', 'Campeche y Mas' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16')");

            /*Champoton*/
            consultasPrediseneadas.Add("select '', 'Champoton' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03");

            /*Escarcega y candelaria*/
            consultasPrediseneadas.Add("select '', 'Escarcega - Candelaria' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg in ('04', '11')");

            /*Calkini*/
            consultasPrediseneadas.Add("select '', 'Calkini' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05");

            /*Hecelchakan*/
            consultasPrediseneadas.Add("select '', 'Hecelchakan' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 06");

            /*Hopelchen*/
            consultasPrediseneadas.Add("select '', 'Hopelchen' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo." + An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 07");

            return consultasPrediseneadas;
        }



        /// <summary>
        /// Obtiene una cadena de una consulta para delegacion de una nomina en espefico filtrado 
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePago(int Delegacion, string An )
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                        /*Campeche*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 3:
                       /*Champoton 03 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 5:
                     /*Calkini 5 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    
                    break;
                case 6:
                    /*Hecelchakan 6 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";
                    
                    break;
                case 7:
                     /*Hopelchen 7 */
                        cadenaConsulta = " select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";
                    
                    break;

            }
            return cadenaConsulta;
        }





        /// <summary>
        /// Obtiene una consulta de delegacion seleccionada de una nomina QUE NO ES GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA 
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta con la que se deben de foliar TODAS LAS NOMINAS EXCEPTO LAS NOMINAS (GENERAL, DESCENTRALIZADA Y PENSION ALIMENTICA) </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoFoliar(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' ) order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = ''  and deleg = 03 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = " select  NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in ('04', '11') order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS ";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = " select NUM 'NUM', RFC  , NOMBRE , LIQUIDO from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 order by  IIF(isnull(NOM_ESP, 0) = 1, '1', '2'), DELEG, SUBSTRING(PARTIDA, 2, 8), NOMBRE collate SQL_Latin1_General_CP1_CI_AS";

                    break;

            }
            return cadenaConsulta;
        }





        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//





        //Consultas para la nomina de PENCION ALIMENTICIA

        public List<string> ObtenerConsultaTotalesPencionAlimenticia(string An) 
        {
            List<string> consultaTotales = new List<string>();


            /*Consultar_TOTALES*/
            /*Campeche*/
            consultaTotales.Add("select '', 'Campeche y Otros' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16')");



            /*Champoton*/
            consultaTotales.Add("select '', 'Champoton' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03");




            /*Escarcega y candelaria*/
            consultaTotales.Add("select '', 'Escarcega y Candelaria' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04', '11')");



            /*Calkini*/
            consultaTotales.Add("select '', 'Calkini' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05");




            /*Hecelchakan*/
            consultaTotales.Add("select '', 'Hecelchakan' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06");



            /*Hopelchen*/
           consultaTotales.Add("select '', 'Hopelchen' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07 ");



            return consultaTotales;
        }




        /// <summary>
        /// Obtiene una cadena de una consulta para delegacion de una nomina con PENSION ALIMENTICIA PARA REVISION (Como quedaria ya foliada)
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticia(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  order by JUZGADO, NOMBRE";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03  order by JUZGADO, NOMBRE";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  order by JUZGADO, NOMBRE";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by JUZGADO, NOMBRE";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06  order by JUZGADO, NOMBRE";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = "select '' 'ID' , Substring(PARTIDA, 1, 6) 'PARTIDA',  num 'NUM'  , NOMBRE, DELEG, '' 'NOMINA' , NUM_CHE, LIQUIDO, '' 'CUENTABANCARIA' from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07  order by JUZGADO, NOMBRE";

                    break;

            }
            return cadenaConsulta;
        }



        /// <summary>
        /// Obtiene una cadena de una consulta para una delegacion de nomina con PENSION ALIMENTICIA PARA FOLIAR
        /// </summary>
        /// <param name="An"> An como se encuentra en la bitacora</param>
        /// <param name="Delegacion"> entero con el numero de delegacion seleccionada</param>
        /// <returns>Regresa una consulta con la que se debe de foliar una NOMINA DE PENSION ALIMENTICIA  </returns>
        public string ObtenerConsultaConOrdenamientoFormasDePagoPensionAlimenticiaFoliar(int Delegacion, string An)
        {
            string cadenaConsulta = null;
            switch (Delegacion)
            {
                case 0:
                    /*Campeche*/
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )  order by JUZGADO, NOMBRE";

                    break;
                case 3:
                    /*Champoton 03 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 03  order by JUZGADO, NOMBRE";

                    break;
                case 4:
                    /*Escarcega y candelaria 04 - 11*/
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg in('04' , '11' )  order by JUZGADO, NOMBRE";

                    break;
                case 5:
                    /*Calkini 5 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 05 order by JUZGADO, NOMBRE";


                    break;
                case 6:
                    /*Hecelchakan 6 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 06  order by JUZGADO, NOMBRE";

                    break;
                case 7:
                    /*Hopelchen 7 */
                    cadenaConsulta = "select NUM 'NUM', RFC  , NOMBRE , LIQUIDO, BENEF 'NumBeneficiario'  from interfaces.dbo."+An+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and deleg = 07  order by JUZGADO, NOMBRE";

                    break;

            }
            return cadenaConsulta;
        }


        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************//





    }
}
