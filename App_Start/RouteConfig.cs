using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CentralisationV0
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetDataById",
                url: "Data/GetById/{id}",
                defaults: new { controller = "Data", action = "GetById", id = UrlParameter.Optional }
            );

            // Ajouter la route pour la suppression
            routes.MapRoute(
                name: "DeleteData",
                url: "Data/Delete",
                defaults: new { controller = "Data", action = "Delete" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}

