using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Cds.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

[assembly: FunctionsStartup(typeof(FunctionApp3.Startup))]

namespace FunctionApp3
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // setup connection
            var client = GetClient();

            builder.Services.AddSingleton(sp => new CdsServiceClientPool(client, sp.GetRequiredService<ILogger<CdsServiceClientPool>>()));
            builder.Services.AddScoped<CdsServiceClientPool.Lease>();
        }

        private CdsServiceClient GetClient()
        {
            //TraceControlSettings.TraceLevel = System.Diagnostics.SourceLevels.All;
            //TraceControlSettings.AddTraceListener(new ConsoleTraceListener());
            var client = new CdsServiceClient(Environment.GetEnvironmentVariable("CdsServiceConnectionString"));
            if (client.IsReady)
            {
                return client;
            }
            else
                throw client.LastCdsException;
        }
    }
}