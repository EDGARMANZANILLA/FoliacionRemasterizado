using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class AlertasAlFolearPagomaticosDTO
    {
        public int IdAtencion { get; set; }
        public string NumeroNomina { get; set; }
        public string NombreNomina { get; set; }
        public string Detalle { get; set; }
       
        public int RegistrosFoliados { get; set; }

        public string Solucion { get; set; }
        public string Id_Nom { get; set; }


        //Campo extra solo para los cheques foleados
        public int UltimoFolioUsado { get; set; }
    }
}
