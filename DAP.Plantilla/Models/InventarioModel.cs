﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioModel
    {


        public string NombreBanco { get; set; }
        public string Cuenta { get; set; }

        public int FormasDisponibles { get; set; }

        public string UltimoFolioInventario { get; set; }

        public string UltimoFolioUtilizado { get; set; }


        public Nullable<decimal> EstimadoMeses { get; set; }



    }
}