﻿using EPiServer.ContentApi.Core.Internal;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Linq;
using System.Web;

namespace ContentDeliveryExtendedRouting.Routing
{
    [ServiceConfiguration(IncludeServiceAccessor = false)]
    public class RoutingEventHandler : IDisposable
    {
        private readonly ServiceAccessor<HttpContextBase> _httpContextAccessor;
        private readonly IContentRouteEvents _contentRouteEvents;

        public RoutingEventHandler(IContentRouteEvents routeEvents, ServiceAccessor<HttpContextBase> httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _contentRouteEvents = routeEvents;
            _contentRouteEvents.RoutedContent += RoutedContent;
        }

        private void RoutedContent(object sender, RoutingEventArgs e)
        {
            var httpContext = _httpContextAccessor();
            var routingContext = e.RoutingSegmentContext;
            var request = httpContext?.Request;
            if (request != null && request.AcceptTypes.Contains(RoutingConstants.JsonContentType))
            {
                //make sure routed language is first in accept language header
                var language = routingContext.Language ?? routingContext.ContentLanguage ?? ContentLanguage.PreferredCulture.Name;
                var acceptLanguageHeader = request.Headers[RoutingConstants.AcceptLanguage];
                request.Headers[RoutingConstants.AcceptLanguage] = string.IsNullOrEmpty(acceptLanguageHeader) ?
                    language :
                    $"{language}, { acceptLanguageHeader}";

                //we need to consider the application path of the request in case the app is hosted in a virtual directory in IIS
                var applicationPath = VirtualPathUtility.AppendTrailingSlash(request.ApplicationPath);

                var property = routingContext.GetCustomRouteData<string>(RoutingConstants.RoutedPropertyKey);
                httpContext.RewritePath(property != null ?
                    $"{applicationPath}{RouteConstants.BaseContentApiRoute}content/{routingContext.RoutedContentLink}?{RoutingConstants.RoutedPropertyKey}={property}" :
                    $"{applicationPath}{RouteConstants.BaseContentApiRoute}content/{routingContext.RoutedContentLink}");

                //Set RouteData to null to pass the request to next routes (WebApi route)
                e.RoutingSegmentContext.RouteData = null;
            }
        }

        public void Dispose()
        {
            if (_contentRouteEvents != null)
                _contentRouteEvents.RoutedContent -= RoutedContent;
        }
    }
}
