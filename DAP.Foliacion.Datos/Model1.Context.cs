﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAP.Foliacion.Datos
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using DAP.Foliacion.Entidades;
    
    public partial class FoliacionEntities : DbContext
    {
        public FoliacionEntities()
            : base("name=FoliacionEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<sysdiagrams> sysdiagrams { get; set; }
        public DbSet<Tbl_CuentaBancarias_TipoPagoCuenta> Tbl_CuentaBancarias_TipoPagoCuenta { get; set; }
        public DbSet<Tbl_CuentasBancarias> Tbl_CuentasBancarias { get; set; }
        public DbSet<Tbl_Inventario> Tbl_Inventario { get; set; }
        public DbSet<Tbl_InventarioAsignacionPersonal> Tbl_InventarioAsignacionPersonal { get; set; }
        public DbSet<Tbl_InventarioContenedores> Tbl_InventarioContenedores { get; set; }
        public DbSet<Tbl_InventarioDetalle> Tbl_InventarioDetalle { get; set; }
        public DbSet<Tbl_InventarioTipoIncidencia> Tbl_InventarioTipoIncidencia { get; set; }
    }
}