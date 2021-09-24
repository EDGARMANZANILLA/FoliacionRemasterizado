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
    
    public partial class Tbl_Pagos
    {
        public Tbl_Pagos()
        {
            this.Tbl_ChequesRefoliados_Pagos = new HashSet<Tbl_ChequesRefoliados_Pagos>();
        }
    
        public int Id { get; set; }
        public int Id_nom { get; set; }
        public int Nomina { get; set; }
        public string An { get; set; }
        public string Adicional { get; set; }
        public int Mes { get; set; }
        public int Quincena { get; set; }
        public string ReferenciaBitacora { get; set; }
        public string RfcEmpleado { get; set; }
        public string NumEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public Nullable<bool> EsPenA { get; set; }
        public string BeneficiarioPenA { get; set; }
        public string NumBeneficiario { get; set; }
        public Nullable<int> IdTbl_InventarioDetalle { get; set; }
        public int IdTbl_CuentaBancaria_BancoPagador { get; set; }
        public int FolioCheque { get; set; }
        public decimal ImporteLiquido { get; set; }
        public int IdCat_FormaPago_Pagos { get; set; }
        public int IdCat_EstadoPago_Pagos { get; set; }
        public string Integridad_HashMD5 { get; set; }
        public Nullable<int> IdReferenciaCancelados_Pagos { get; set; }
        public Nullable<int> IdCat_EstadoCancelados_Pagos { get; set; }
        public Nullable<bool> EsRefoliado { get; set; }
        public bool Activo { get; set; }
    
        public virtual Cat_EstadosPago_Pagos Cat_EstadosPago_Pagos { get; set; }
        public virtual Cat_FormasPago_Pagos Cat_FormasPago_Pagos { get; set; }
        public virtual ICollection<Tbl_ChequesRefoliados_Pagos> Tbl_ChequesRefoliados_Pagos { get; set; }
        public virtual Tbl_CuentasBancarias Tbl_CuentasBancarias { get; set; }
        public virtual Tbl_InventarioDetalle Tbl_InventarioDetalle { get; set; }
    }
}
