﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
   public class DatosCompletosBitacoraParaChequesDTO
   {
        public int Id_nom { get; set; }
        public string Nomina { get; set; }
        public string An { get; set; }
        public string Adicional { get; set; }
        public string Quincena { get; set; }
        public int Mes { get; set; }
        public string ReferenciaBitacora { get; set; }
        public bool EsPenA { get; set; }
        public string Coment { get; set; }
        
   }
}
