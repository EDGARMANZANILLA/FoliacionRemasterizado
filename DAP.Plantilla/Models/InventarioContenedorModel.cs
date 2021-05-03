using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioContenedorModel
    {

        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string NumeroOrden { get; set; }
        public int NumeroContenedor { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public int TotalFormasContenedor { get; set; }
        public int FormasDisponiblesActuales { get; set; }
        public Nullable<int> FormasInhabilitadas { get; set; }
        public Nullable<int> FormasAsignadas { get; set; }
        public System.DateTime FechaAlta { get; set; }
    

    }
}