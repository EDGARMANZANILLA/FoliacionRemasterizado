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
    
    public partial class Tbl_InventarioDetalle
    {
        public Tbl_InventarioDetalle()
        {
            this.Tbl_Pagos = new HashSet<Tbl_Pagos>();
        }
    
        public int Id { get; set; }
        public int IdContenedor { get; set; }
        public int NumFolio { get; set; }
        public Nullable<int> IdIncidencia { get; set; }
        public Nullable<System.DateTime> FechaIncidencia { get; set; }
        public Nullable<int> IdEmpleado { get; set; }
        public bool Activo { get; set; }
    
        public virtual Tbl_InventarioAsignacionPersonal Tbl_InventarioAsignacionPersonal { get; set; }
        public virtual Tbl_InventarioContenedores Tbl_InventarioContenedores { get; set; }
        public virtual ICollection<Tbl_Pagos> Tbl_Pagos { get; set; }
        public virtual Tbl_InventarioTipoIncidencia Tbl_InventarioTipoIncidencia { get; set; }
    }
}
