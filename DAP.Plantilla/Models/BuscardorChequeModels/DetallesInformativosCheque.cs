using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.BuscardorChequeModels
{
    public class DetallesInformativosCheque
    {
        public int Id_nom { get; set; }
        public string NumEmpleado { get; set; }
        public int Quincena { get; set; }





        public string Folio { get; set; }
        public decimal Liquido { get; set; }
        public string NombreBeneficiaro { get; set; }


        public string EstadoCheque { get; set; }
        public string ReferenciaBitacora { get; set; }
        public string BancoPagador { get; set; }
       



        public bool EsPenA { get; set; }
        public string NombreBeneficiarioPenA { get; set; }
        public string EstadoCancelado { get; set; }




        public bool EsRefoliado { get; set; }


    }
}