using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Plantilla.Models.BuscardorChequeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DAP.Plantilla
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);



            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<ResultadoObtenidoParaSelect2, ElementosBuscador>();
                cfg.CreateMap<DetallesBusqueda, DetallesBusquedaModels>();
                cfg.CreateMap<DetallesRegistroDTO, DetallesInformativosCheque>();
            
            });

        }
    }
}
