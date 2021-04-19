﻿using System;
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

        public int UltimoFolioInventario { get; set; }

        public int UltimoFolioQuincena { get; set; }

        public Nullable<int> FormasQuincena1 { get; set; }

        public Nullable<int> FormasQuincena2 { get; set; }

        public Nullable<decimal> EstimadoMeses { get; set; }



    }
}