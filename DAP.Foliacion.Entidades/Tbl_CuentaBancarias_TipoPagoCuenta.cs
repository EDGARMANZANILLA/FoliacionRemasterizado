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
    
    public partial class Tbl_CuentaBancarias_TipoPagoCuenta
    {
        public Tbl_CuentaBancarias_TipoPagoCuenta()
        {
            this.Tbl_CuentasBancarias = new HashSet<Tbl_CuentasBancarias>();
        }
    
        public int Id { get; set; }
        public string TipoPago { get; set; }
    
        public virtual ICollection<Tbl_CuentasBancarias> Tbl_CuentasBancarias { get; set; }
    }
}
