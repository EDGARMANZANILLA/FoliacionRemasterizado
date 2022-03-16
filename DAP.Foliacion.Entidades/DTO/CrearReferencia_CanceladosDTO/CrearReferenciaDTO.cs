﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO
{
    public class CrearReferenciaDTO
    {
       
        public int Id { get; set; }
        public int Id_Iterador { get; set; }
        public int Anio { get; set; }
        public int Numero_Referencia { get; set; }
        public string Fecha_Creacion { get; set; }
        public string Creado_Por { get; set; }
        public int FormasPagoCargadas { get; set; }
        public bool Activo { get; set; }
    }
}
