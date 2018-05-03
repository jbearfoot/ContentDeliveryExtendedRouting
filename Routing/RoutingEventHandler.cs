﻿using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                var property = routingContext.GetCustomRouteData<string>(RoutingConstants.RoutedPropertyKey);
                httpContext.RewritePath(property != null ?
                    $"/{EPiServer.ContentApi.RouteConstants.BaseContentApiRoute}content/{routingContext.RoutedContentLink}?{RoutingConstants.RoutedPropertyKey}={property}" :
                    $"/{EPiServer.ContentApi.RouteConstants.BaseContentApiRoute}content/{routingContext.RoutedContentLink}");
            }
        }

        public void Dispose()
        {
            if (_contentRouteEvents != null)
                _contentRouteEvents.RoutedContent -= RoutedContent;
        }
    }
}
