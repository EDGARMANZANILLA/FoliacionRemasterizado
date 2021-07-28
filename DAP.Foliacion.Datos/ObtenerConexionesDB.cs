using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos
{
    public class ObtenerConexionesDB
    {
        public static string obtnercadenaConexionAlpha()
        {
            return @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
        }


        public static string obtenerCadenaConexionLocalInterfaces()
        {
            return @"Data Source=172.19.62.71; Initial Catalog=Interfaces; User=sa; PassWord=dbadmin";
        }


        public static string obtenerCadenaConexion251Nomina()
        {
            return @"Data Source=172.19.2.251; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
        }
    }
}
