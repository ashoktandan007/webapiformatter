﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using MessagePack;
using MessagePack.Resolvers;

namespace WEBAPIFORMATTER
{
    public class MessageFormatter:MediaTypeFormatter {
        public MessageFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-msgpack"));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (writeStream == null)
            {
                throw new ArgumentNullException(nameof(writeStream));
            }

            MessagePackSerializer.NonGeneric.Serialize(type, writeStream, value, ContractlessStandardResolver.Instance);
            return Task.FromResult(0);
        }
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (readStream == null)
            {
                throw new ArgumentNullException(nameof(readStream));
            }

            var value = MessagePackSerializer.NonGeneric.Deserialize(type, readStream, ContractlessStandardResolver.Instance);
            return Task.FromResult(value);
        }
    }
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Add(new MessageFormatter());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
