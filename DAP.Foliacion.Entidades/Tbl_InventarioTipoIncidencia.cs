//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAP.Foliacion.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_InventarioTipoIncidencia
    {
        public Tbl_InventarioTipoIncidencia()
        {
            this.Tbl_InventarioDetalle = new HashSet<Tbl_InventarioDetalle>();
        }
    
        public int Id { get; set; }
        public string Descrip_Incidencia { get; set; }
        public bool Activo { get; set; }
    
        public virtual ICollection<Tbl_InventarioDetalle> Tbl_InventarioDetalle { get; set; }
    }
}
