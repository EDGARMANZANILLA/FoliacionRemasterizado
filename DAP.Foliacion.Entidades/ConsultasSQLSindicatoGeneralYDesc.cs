using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades
{
    public class ConsultasSQLSindicatoGeneralYDesc
    {
        public static string AnObtenido = null;
        ConsultasSQLSindicatoGeneralYDesc( string An) 
        {
            AnObtenido = An;
        }

  

        /*Para los de confianza osea NO SINDICALIZADOS*/
        /*Campeche*/
        string Campeche_Confianza = "select 'Campeche y Mas' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )" ;

        /*Champoton*/
        string Champoton__Confianza = "select 'Champoton' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 03";

        /*Escarcega y candelaria*/
        string EscarcegaYCandelaria_Confianza = "select 'Escarcega - Candelaria' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg in ('04', '11')";

        /*Calkini*/
        string Calkini_Confianza = "select 'Calkini' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 05";

        /*Hecelchakan*/
        string Hecelchakan_Confianza = "select 'Hecelchakan' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 06";

        /*Hopelchen*/
        string Hopelchen_Confianza = "select 'Hopelchen' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 0 and deleg = 07";







        /*Para los de Sindicalizados*/
        /*Campeche*/
        string Campeche_Sindicalizados = "select 'Campeche y Mas' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in('00' , '01', '02', '08', '09', '10', '12', '13', '14', '15', '16' )";

        /*Champoton*/
        string Champoton_Sindicalizados = "select 'Champoton' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 03";
        
        /*Escarcega y candelaria*/
        string EscarcegaYCandelaria_Sindicalizados = "select 'Escarcega - Candelaria' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg in ('04', '11')";
        
        /*Calkini*/
        string Calkini_Sindicalizados = "select 'Calkini' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 05" ;
        
        /*Hecelchakan*/
        string Hecelchakan_Sindicalizados = "select 'Hecelchakan' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 06";

        /*Hopelchen*/
        string Hopelchen_Sindicalizados = "select 'Hopelchen' 'Nom_Deleg' ,count(*) 'Total' from interfaces.dbo."+AnObtenido+" where TARJETA = '' and SERFIN = '' and BANCOMER = '' and BANORTE = '' and HSBC = '' and sindicato = 1 and deleg = 07";




        /// <summary>
        /// Devuelve 6 cadenas de consultas para los de confianza y otras 6 para los sindicalizado  
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerConsultasSindicalizados() 
        {
            //consul
            List<string> consultasPreparadas = new List<string>();

            if(AnObtenido != null)
            {
                consultasPreparadas.Add(Campeche_Confianza);
                consultasPreparadas.Add(Champoton__Confianza);
                consultasPreparadas.Add(EscarcegaYCandelaria_Confianza);
                consultasPreparadas.Add(Calkini_Confianza);
                consultasPreparadas.Add(Hecelchakan_Confianza);
                consultasPreparadas.Add(Hopelchen_Confianza);



                consultasPreparadas.Add(Campeche_Sindicalizados);
                consultasPreparadas.Add(Champoton_Sindicalizados);
                consultasPreparadas.Add(EscarcegaYCandelaria_Sindicalizados);
                consultasPreparadas.Add(Calkini_Sindicalizados);
                consultasPreparadas.Add(Hecelchakan_Sindicalizados);
                consultasPreparadas.Add(Hopelchen_Sindicalizados);
            }
 


            return consultasPreparadas;
        }


    }
}
