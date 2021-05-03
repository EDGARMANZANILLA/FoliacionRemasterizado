using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioModel
    {

        public int Id { get; set; }

        public string NombreBanco { get; set; }

        public int FormasDisponibles { get; set; }

        public string UltimoFolioInventario { get; set; }

        public string UltimoFolioUtilizado { get; set; }

        public Nullable<int> FormasUsadasQuincena1 { get; set; }

        public Nullable<int> FormasUsadasQuincena2 { get; set; }

        public Nullable<decimal> EstimadoMeses { get; set; }



    }
}