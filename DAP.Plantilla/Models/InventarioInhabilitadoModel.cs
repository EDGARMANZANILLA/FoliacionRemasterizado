using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioInhabilitadoModel
    {
        public string banco { get; set; }

        public string Cuenta { get; set; }
        public string Orden { get; set; }
        public int Contenedor { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public int TotalFormas { get; set; }
        public System.DateTime FechaDetalle { get; set; }
       
    }
}