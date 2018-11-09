﻿using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Web;
using System.Web.Http;

namespace ContentDeliveryExtendedRouting.Routing
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class RoutingInitalization : IInitializableModule
    {
        private RoutingEventHandler _routingEventHandler;
        private ServiceAccessor<HttpContextBase> _httpContextAccessor;

        public void Initialize(InitializationEngine context)
        {
            _routingEventHandler = context.Locate.Advanced.GetInstance<RoutingEventHandler>();
            _httpContextAccessor = context.Locate.Advanced.GetInstance<ServiceAccessor<HttpContextBase>>();
            Global.RoutesRegistered += (o, e) => e.Routes.RegisterPartialRouter(ServiceLocator.Current.GetInstance<PropertyPartialRouter>());
            GlobalConfiguration.Configuration.Filters.Add(new PropertyResolveFilter(_httpContextAccessor));
        }

        public void Uninitialize(InitializationEngine context)
        {
            _routingEventHandler?.Dispose();
            _routingEventHandler = null;
        }
    }
}
