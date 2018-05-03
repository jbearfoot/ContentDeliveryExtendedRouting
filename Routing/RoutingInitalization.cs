using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

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
            Global.RoutesRegistered += WrapContentRoutes;
            GlobalConfiguration.Configuration.Filters.Add(new PropertyResolveFilter(_httpContextAccessor));
        }

        //until CMS-10523 is fixed we need to wrap default routes instead of just listen to IContentRouteEvents.RoutedContent
        private void WrapContentRoutes(object sender, EPiServer.Web.Routing.RouteRegistrationEventArgs e)
        {
            //Find index of content routes
            var indexedRoutes = new Dictionary<int, IContentRoute>();
            for (int i = 0; i < e.Routes.Count; i++)
            {
                if (e.Routes[i] is IContentRoute contentRoute)
                    indexedRoutes.Add(i, contentRoute);
            }

            foreach (var contentRoute in indexedRoutes)
            {
                var wrappedRoute = new ContentRouteWrapper(contentRoute.Value, _httpContextAccessor);
                e.Routes.RemoveAt(contentRoute.Key);
                e.Routes.Insert(contentRoute.Key, wrappedRoute);
            }

            e.Routes.RegisterPartialRouter(ServiceLocator.Current.GetInstance<PropertyPartialRouter>());
        }

        public void Uninitialize(InitializationEngine context)
        {
            _routingEventHandler?.Dispose();
            _routingEventHandler = null;
        }
    }
}
