﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using EventManager.BL.Bootstrap;
using System.Security.Claims;
using System.Web.Helpers;

namespace EventManager.PL
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IWindsorContainer _container;

        public MvcApplication()
        {
           
        }

        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureBoostrap();
        }

        private void ConfigureBoostrap()
        {
            _container = new WindsorContainer();

            // configure DI            
            _container.Install(new BussinessLayerInstaller());
            _container.Install(new MvcInstaller());

            // configure mapping within BL
            AutoMapperConfig.Initialize();

            // initialize default user accounts (admin, ...)
            DataWithAccountsInit.InitializeUserAccounts(_container);

            // set controller factory
            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}
