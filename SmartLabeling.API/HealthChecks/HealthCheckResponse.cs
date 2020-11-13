using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace SmartLabeling.API.HealthChecks
{
    public static class HealthCheckResponse
    {
        private readonly static DateTime _upTime;
        private readonly static string _serviceName;

        static HealthCheckResponse() 
        {
            _upTime = DateTime.UtcNow;
            _serviceName = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        public static async Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport healthReport)
        {
            httpContext.Response.ContentType = "application/json";

            //var json = JsonConverter.SerializeObject(
            //    new 
            //    {
            //        serviceName = _serviceName,
            //        status = healthReport.Status,
            //        timestamp = DateTime.UtcNow,
            //        upTime = _upTime,
            //        elaspsedTime = healthReport.TotalDuration.Milliseconds,
            //        subSystems = healthReport.Entries
            //    },
            //    Formatting.Indented,
            //    new JsonStringEnumConverter()
            //    );
        }
    }
}
