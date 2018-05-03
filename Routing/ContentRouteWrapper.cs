using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace ContentDeliveryExtendedRouting.Routing
{
    public class ContentRouteWrapper : RouteBase, IContentRoute
    {
        private IContentRoute _defaultRoute;
        private ServiceAccessor<HttpContextBase> _httpContextAccessor;

        public ContentRouteWrapper(IContentRoute defaultRoute, ServiceAccessor<HttpContextBase> httpContextAccessor)
        {
            _defaultRoute = defaultRoute;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Name => _defaultRoute.Name;

        public RouteValueDictionary Defaults { get => _defaultRoute.Defaults; set => _defaultRoute.Defaults = value; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = (_defaultRoute as RouteBase).GetRouteData(httpContext);
            //If path has been rewritten to content delivery API do not route
            return httpContext.Request.Path.StartsWith($"/{EPiServer.ContentApi.RouteConstants.BaseContentApiRoute}") ? null : routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return _defaultRoute.GetVirtualPath(requestContext, values);
        }

        public bool MatchConstraints(SegmentContext segmentContext, HttpContextBase context)
        {
            return _defaultRoute.MatchConstraints(segmentContext, context);
        }

        public RouteData RouteSegmentContext(SegmentContext segmentContext)
        {
            return _defaultRoute.RouteSegmentContext(segmentContext);
        }
    }
}
