using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class FoliarFormasPagoDTO
    {

        public int IdNomina { get; set; }
        public int Delegacion { get; set; }
        public bool Sindicato { get; set; }
        public bool Confianza { get; set; }
        public int IdBancoPagador { get; set; }
        public int RangoInicial { get; set; }

        //por si se habilita la casilla inhabilitados 
        public bool Inhabilitado { get; set; }
        public int RangoInhabilitadoInicial { get; set; }
        public int RangoInhabilitadoFinal { get; set; }


        // propiedad usada para saber a que grupo de nomina corresponde 
        // 1 = le pertenece a las nominas general y descentralizada
        // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza 
        public int GrupoNomina { get; set; }

    }
}
