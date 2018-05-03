# Extended routing for Episerver Content Delivery API

## Description
Episerver Content Delivery API delivers content through Urls like: http://&lt;your-site-url&gt;/api/episerver/v1.0/content/&lt;contentReference&gt;. 
This module extends the CMS routing so that it is possible to use the same "Friendly URLs" as in CMS to get data from Content Delivery API. 
The module hooks into the CMS routing and after a content has been routed and then look at the Accept header and in case it was "application/json" rewrites the request to the Web API controller in EPiServer Content Delivery. 
So it does not do any Json serialization but rely on Content Delivery API for that (same with access checks) and it also does not do any redirects but stay on the friendly url.
It also adds support to load data for individual properties like http://&lt;your-site-url&gt;/en/alloy-meet/MainBody where property MainBody is loaded for content with url http://&lt;your-site-url&gt;/en/alloy-meet

## Future improvments
An idea is to add Graph QL support to make it possible to query groups of properties.

## Disclamier
This is nothing offically supported by EPiServer, you are free to use it as you like at your own risk.