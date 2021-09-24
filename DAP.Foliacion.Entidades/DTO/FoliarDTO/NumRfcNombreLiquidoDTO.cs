using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class NumRfcNombreLiquidoDTO
    {
        public string NumeroEmpleado { get; set; }
        public string Rfc { get; set; }
        public string Nombre { get; set; }
        public decimal Liquido { get; set; }
        public string NombreBanco { get; set; }
        public string IdCuentaBancaria { get; set; }




        /*Datos espeficicamente parte*/
        public string NumBeneficiario { get; set; }

    }
}
