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
    
    public partial class Tbl_CuentasBancarias
    {
        public int Id { get; set; }
        public string NombreBanco { get; set; }
        public string Abreviatura { get; set; }
        public string Cuenta { get; set; }
        public int IdCuentaBancaria_TipoPagoCuenta { get; set; }
        public Nullable<int> IdInventario { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaBaja { get; set; }
        public Nullable<bool> Activo { get; set; }
    
        public virtual Tbl_CuentaBancarias_TipoPagoCuenta Tbl_CuentaBancarias_TipoPagoCuenta { get; set; }
        public virtual Tbl_Inventario Tbl_Inventario { get; set; }
    }
}