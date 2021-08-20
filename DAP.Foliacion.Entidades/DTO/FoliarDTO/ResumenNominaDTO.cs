using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ResumenNominaDTO
    {
        public string Delegacion { get; set; }

        public int Sindicalizado { get; set; }
        public int Confianza { get; set; }
        public int Otros { get; set; }
        public bool Foliado { get; set; }
        public int Total { get; set; }
    
    }


}
